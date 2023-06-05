open System
open System.Collections.Generic

type Token = string array

type graph = Dictionary<int, int array>

let newGraph = graph()

let addNode index neighbours (g: graph) =
    let ns = Array.filter (fun n -> n >= 0) neighbours
    g.Add(index, ns)

let getDistanceTableFrom (start: int) (g: graph) =
    let distance : int array = Array.create g.Count -1
    distance[start] <- 0
    let searchQueue = Queue<int>()
    searchQueue.Enqueue start
    while searchQueue.Count > 0 do
        let mutable size = searchQueue.Count
        
        while (size > 0) do
            size <- size - 1
            let vertex = searchQueue.Dequeue()
            let adjVertices = g[vertex]
            
            for adjVertex in adjVertices do
                if (distance[adjVertex] = -1) then
                    distance[adjVertex] <- distance[vertex] + 1
                    searchQueue.Enqueue adjVertex
    distance

type Resource = 
    | Empty
    | Egg of int
    | Crystal of int
    static member Create (t, v) =
        match t, v with
        | 0, _ -> Empty
        | 1, v -> Egg v
        | 2, v -> Crystal v
        | i, _ -> failwithf "Unknown value when parsing Resource: %i" i
    member x.Update amount =
        match x with
        | Empty -> x
        | Egg _ -> Egg amount
        | Crystal _ -> Crystal amount

type InitialInputToken = 
    {
        Resource: Resource
        Neighbours: int array
    }
    static member Create (token: Token) =
        {
            Resource = Resource.Create (token[0] |> int, token[1] |> int)
            Neighbours = (token[2..7]) |> Array.map int            
        }

type GameTurnToken =
    {
        ResourceAmount: int
        MyAnts: int
        EnemyAnts: int
    }
    static member Create (token: Token) =
        {
            ResourceAmount = token[0] |> int
            MyAnts = token[1] |> int
            EnemyAnts = token[2] |> int
        }

type AntBase = 
    | Mine
    | Enemy

type Cell = 
    {
        Index: int
        Resource: Resource
        AntBase: AntBase option
        Neighbours: int array
    }
    static member Create (index: int) (token: InitialInputToken) =
        {
            Index = index
            Resource = token.Resource
            AntBase = None
            Neighbours = token.Neighbours
        }

type Beacon = 
    {
        Strength: int
        Index: int
    }

type BeaconLine = 
    { 
        Start: Cell
        End: Cell
        Strength: int
    }

type Action = 
    | PlaceBeacon of Beacon
    | PlaceBeaconLine of BeaconLine
    | Wait
    | Message of string
    override x.ToString() =
        match x with
        | PlaceBeacon b -> sprintf "BEACON %i %i" b.Index b.Strength
        | PlaceBeaconLine bl -> sprintf "LINE %i %i %i" bl.Start.Index bl.End.Index bl.Strength
        | Wait -> failwith "NotImplemented"
        | Message(_) -> failwith "NotImplemented"


// helper functions
let readInput () = Console.ReadLine()
let readInt = readInput >> int
let tokenize (line : string) = line.Split ' '
let tokenizeInput = readInput >> tokenize
let readIndices = tokenizeInput >> Array.map int
let readNLines n = Array.init n (fun _ -> readInput())
let tokenizeNLines n : (Token array) = Array.init n (fun _ -> tokenizeInput())

// Main functions
let updateAntBases antbase (cells: Cell array) indices =
    indices 
    |> Array.iter (fun i -> cells[i] <- { cells[i] with AntBase = Some antbase} )

let updateMap (tokens: GameTurnToken array) (cells: Cell array) =
    cells
    |> Array.iteri (fun idx cell -> cells[idx] <- { cell with Resource = cell.Resource.Update tokens[idx].ResourceAmount })

let getResourceCells (cells: Cell array) =
    cells |> Array.filter(fun cell -> 
        match cell.Resource with
        | Crystal amount when amount > 0 -> true
        | Egg amount when amount > 0 -> true
        | _ -> false)

let getActions (myBases: Cell array) (targetCells: Cell array) (g:graph) =

    let distanceTable1 = getDistanceTableFrom myBases[0].Index g

    let eggTargets1 =
        targetCells
        |> Array.filter (fun c -> 
            match c.Resource with
            | Egg amount when amount > 0 -> true
            | _ -> false)
        |> Array.map (fun c -> c, distanceTable1[c.Index])
            |> Array.sortBy (fun (c, d) -> 
                let a = 
                    match c.Resource with
                    | Crystal amount -> amount
                    | _ -> 0
                d, -a)
        |> Array.map fst
        //|> Array.take 3
        |> Array.map (fun cell -> PlaceBeaconLine {Start = myBases[0]; End = cell; Strength = 2})

    let crystalTargets1 =
        targetCells
        |> Array.filter (fun c -> 
            match c.Resource with
            | Crystal amount when amount > 0 -> true
            | _ -> false)
        |> Array.map (fun c -> c, distanceTable1[c.Index])
        |> Array.sortBy (fun (c, d) -> 
            let a = 
                match c.Resource with
                | Crystal amount -> amount
                | _ -> 0
            d, -a)
        |> Array.map fst
        //|> Array.take 3
        |> Array.map (fun cell ->  PlaceBeaconLine {Start = myBases[0]; End = cell; Strength = 1})

    let mutable targets = [| eggTargets1; crystalTargets1 |]

    if myBases.Length > 1 then
        let distanceTable2 = getDistanceTableFrom myBases[1].Index g

        let eggTargets2=
            targetCells
            |> Array.filter (fun c -> 
                match c.Resource with
                | Egg amount when amount > 0 -> true
                | _ -> false)
            |> Array.map (fun c -> c, distanceTable2[c.Index])
                |> Array.sortBy (fun (c, d) -> 
                    let a = 
                        match c.Resource with
                        | Crystal amount -> amount
                        | _ -> 0
                    d, -a)
            |> Array.map fst
            //|> Array.take 2
            |> Array.map (fun cell -> PlaceBeaconLine {Start = myBases[1]; End = cell; Strength = 2})

        let crystalTargets2 =
            targetCells
            |> Array.filter (fun c -> 
                match c.Resource with
                | Crystal amount when amount > 0 -> true
                | _ -> false)
            |> Array.map (fun c -> c, distanceTable2[c.Index])
            |> Array.sortBy (fun (c, d) -> 
                let a = 
                    match c.Resource with
                    | Crystal amount -> amount
                    | _ -> 0
                d, -a)
            |> Array.map fst
            //|> Array.take 3
            |> Array.map (fun cell ->  PlaceBeaconLine {Start = myBases[1]; End = cell; Strength = 1})

        targets <- [|eggTargets1; eggTargets2; crystalTargets1; crystalTargets2|]

    targets |> Array.collect id |> Array.toList

let formatOutput (actions: Action list) =
    actions
    |> List.map (fun a -> a.ToString())
    |> String.concat ";"

// Initial game input
let numberOfCells = readInt ()
let cells = 
    numberOfCells
    |> tokenizeNLines 
    |> Array.mapi (fun idx token -> Cell.Create idx (InitialInputToken.Create token))
    
let graph = newGraph
cells |> Array.iter (fun c -> addNode c.Index c.Neighbours graph)

let _numberOfBases = readInt () // number of bases for each team... not using at the moment

readIndices() |> updateAntBases Mine cells
readIndices() |> updateAntBases Enemy cells

let myBases = cells |> Array.filter (fun cell -> cell.AntBase = Some Mine) // single base for now

// Game loop
while true do

    let gameturnTokens =
        numberOfCells
        |> tokenizeNLines
        |> Array.map (GameTurnToken.Create)

    updateMap gameturnTokens cells

    let actions = getActions myBases cells graph

    //eprintfn "%A" actions

    actions |> formatOutput |> printfn "%s"
    