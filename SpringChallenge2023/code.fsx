open System

type Token = string array

type Resource = 
    | Empty // 0 - does not contain a resource
    | Egg // 1 - ignored for wood league 2
    | Crystal // 2 - contains crystal resource
    static member Create =
        function
        | 0 -> Empty
        | 1 -> Egg
        | 2 -> Crystal
        | i -> failwithf "Unknown value when parsing Resource: %i" i

type InitialInputToken = 
    {
        Resource: Resource
        Amount: int
        // 6 x Neigh - Ignored for wood league 2
    }
    static member Create (token: Token) =
        {
            Resource = token[0] |> int |> Resource.Create
            Amount = token[1] |> int
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
        Amount: int
        AntBase: AntBase option
    }
    static member Create (index: int) (token: InitialInputToken) =
        {
            Index = index
            Resource = token.Resource
            Amount = token.Amount
            AntBase = None
        }


//type Ant = ???

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

// Main functions
let updateAntBases antbase (cells: Cell array) indices =
    indices 
    |> Array.iter (fun i -> cells[i] <- { cells[i] with AntBase = Some antbase} )

let updateMap (tokens: GameTurnToken array) (cells: Cell array) =
    cells
    |> Array.iteri (fun idx cell -> cells[idx] <- { cell with Amount = tokens[idx].ResourceAmount })

let getResourceCells (cells: Cell array) =
    cells |> Array.filter(fun cell -> cell.Resource = Crystal && cell.Amount > 0)

let getActions myBase (targetCells: Cell array) =
    // Simple strategy at beginning:
    // Put a beacon on each cell with a resource, with highest strength on highest resource amount
    targetCells
    |> Array.sortBy (fun cell -> cell.Amount)
    |> Array.mapi (fun i cell -> PlaceBeaconLine { Start = myBase; End = cell; Strength = i } )

let formatOutput (actions: Action array) =
    actions
    |> Array.map (fun a -> a.ToString())
    |> String.concat ";"

// Initial game input
let numberOfCells = readInt ()
let cells = 
    numberOfCells
    |> readNLines
    |> Array.mapi (fun idx text -> 
        let token = (tokenize text) |> InitialInputToken.Create
        Cell.Create idx token)
    
let _numberOfBases = readInt () // number of bases for each team... not using at the moment

readIndices() |> updateAntBases Mine cells
readIndices() |> updateAntBases Enemy cells

//eprintfn "%A" cells

// Game loop
while true do

    let gameturnTokens =
        numberOfCells
        |> readNLines
        |> Array.map (tokenize >> GameTurnToken.Create)

    updateMap gameturnTokens cells

    let myBase = cells |> Array.find (fun cell -> cell.AntBase = Some Mine) // single base for now

    let actions =
        cells
        |> getResourceCells
        |> getActions myBase

    //eprintfn "%A" actions

    actions |> formatOutput |> printfn "%s"
    