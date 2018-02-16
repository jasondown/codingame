// https://www.codingame.com/training/hard/don't-panic-episode-2

open System

type Vertex = 
    | Label of string

type Edge = (Vertex * Vertex) * double
    
type Graph =
    | Directed of Set<Vertex> * Set<Edge>
    | Undirected of Set<Vertex> * Set<Edge>

type AdjacencyList<'TLabel when 'TLabel : comparison> = 
    L of Map<Vertex, Set<(Vertex * 'TLabel)>>

type AdjacencyMatrix<'TLabel when 'TLabel : comparison> = 
    M of Map<Vertex * Vertex, 'TLabel>

type ResultLookup<'TLookupInfo> (map : Map<Vertex, 'TLookupInfo>, empty : 'TLookupInfo) = 
    let update vertex f map =
        match map |> Map.tryFind vertex with
        | Some info -> map |> Map.add vertex (f info)
        | None      -> map |> Map.add vertex (f empty)

    member this.Unwrap = map

    member this.Find key = 
        map |> Map.find key

    member this.TryFind key = 
        map |> Map.tryFind key

    member this.FindOrEmpty key = 
        match this.TryFind key with
        | Some info -> info
        | None      -> empty        

    member this.Update vertex f =   
        ResultLookup(map |> update vertex f, empty)

module Graphs = 
    // Take an edge and switch the start and end vertices.
    let invert = function ((v1, v2), weight) -> ((v2, v1), weight)

    // Recognizer for deconstructing graphs in a transparent manner.
    let (|Graph|_|)  graph = 
        match graph with
        | Directed (v, e)   -> Some (v, e)
        | Undirected (v, e) -> Some (v, Set.union e (Set.map invert e))

    // Get the graph edges; uses the recognizer so for each undirected edge will get a pair of directed edges.
    let getEdges = function 
        | Graph (v, e) -> e
        | other -> failwithf "Not a graph: %A" other

    // Builds a weighted adjacency list from a set of edges.
    let buildAdjacencyList =
        getEdges
        >> Seq.map (fun ((v1, v2), weight) -> (v1, (v2, weight)))
        >> Seq.groupBy fst
        >> Seq.map (fun (key, vertex) ->
            key, vertex |> Seq.map snd |> Set.ofSeq)
        >> Map.ofSeq
        >> AdjacencyList.L

    // Builds an adjacency matrix from a set of edges.
    let buildAdjacencyMatrix = 
        getEdges 
        >> Map.ofSeq
        >> AdjacencyMatrix.M

    // Gets the neighbour vertices using an adjacency list.
    let neighbours (L list) vertex = 
        match list |> Map.tryFind vertex with
        | Some vertices -> vertices |> Set.toList |> List.map fst
        | None -> List.empty

type Color = 
    | White
    | Gray
    | Black

module Search =
    module BreadthFirst = 

        type VertexLookupInfo = 
            {
                predecessor: Vertex option
                distance: int
            }
            static member Empty = 
                {
                    predecessor = None
                    distance = 0
                }

        let buildLookup graph source = 
            match graph with
            | Graphs.Graph (vertices, edges) -> 
                // start with the vertices colored white
                let initialColoring = 
                    vertices
                    |> Seq.map (fun v -> v, Color.White)
                    |> Map.ofSeq

                // function that gets the neighbouring nodes reachable from a vertex
                let neighbours = Graphs.neighbours (Graphs.buildAdjacencyList graph)

                // mutable queue for storing vertices
                let queue = System.Collections.Generic.Queue<Vertex>()
                
                let rec visit (queue: System.Collections.Generic.Queue<_>) vertexColoring (resultLookup: ResultLookup<_>) = 
                    // while queue is not empty...
                    if queue.Count = 0
                        then resultLookup.Unwrap
                        else
                            // ...grab a vertex and its info
                            let current = queue.Dequeue()
                            let currentInfo = resultLookup.FindOrEmpty(current)
                            
                            // fold over its neighbours, updating results lookup
                            let updatedColored, updatedLookup =
                                neighbours current
                                |> List.fold (fun (coloring : Map<Vertex, _>, lookup : ResultLookup<_>) vertex -> 
                                    match coloring.[vertex] with
                                    | Color.White ->
                                        // we haven't seen the vertex, put it in the queue and update its info
                                        queue.Enqueue vertex
                                        let updatedLookup =
                                            lookup.Update vertex <| fun info -> 
                                                { info with predecessor = Some current; distance = currentInfo.distance + 1 }
                                        let updatedColored = 
                                            coloring |> Map.add vertex Color.Gray
                                        (updatedColored, updatedLookup)
                                    | _ -> (coloring, lookup)) (vertexColoring, resultLookup)
                            
                            // try processing another vertex from the queue
                            visit queue updatedColored updatedLookup

                // put the starting vertex in the queue...
                queue.Enqueue source

                // ...and kick-off
                visit queue initialColoring (ResultLookup(Map.empty, VertexLookupInfo.Empty))

            | other -> failwithf "Not a graph: %A" other

module Collections = 
    /// Leftist heap (from Okasaki).
    /// Supports lazy deletion, makes it easier to implement Dijkstra's algorithm this way.
    type Heap<'T> =
        | Empty
        | Node of int (*rank*) * bool (*deleted*) * 'T (*element*) * Heap<'T> (*left child*) * Heap<'T> (*right child*)

    /// Operations on a leftist heap.
    module Heap =
        /// Returns an empty heap of type 'T.
        let empty<'T> () =
            Heap.Empty : Heap<'T>

        /// Gets the rank of a node.
        let rank = function
            | Empty -> 0
            | Node (rank, _, _, _, _) -> rank            

        /// Creates a node with element t and children a and b.
        let makeNode t a b =
            let ra, rb = rank a, rank b
            if ra >= rb 
                then Node (rb + 1, false, t, a, b)
                else Node (ra + 1, false, t, b, a)

        /// Merges two heaps together.
        let rec merge (heap1:Heap<'T>) (heap2:Heap<'T>) =
            match heap1, heap2 with
            | Empty, h
            | h, Empty -> h
            | (Node (_, _, t1, a1, b1) as h1), (Node (_, _, t2, a2, b2) as h2) ->
                if t1 <= t2
                    then makeNode t1 a1 (merge b1 h2)
                    else makeNode t2 a2 (merge h1 b2)

        /// Inserts element t into the heap.
        let insert t heap =
            let single = Node (1, false, t, Empty, Empty)
            merge single heap

        /// Generic fold over a heap.
        let fold func emptyState heap =
            let rec inner heap cont = 
                match heap with  
                | Empty -> cont emptyState
                | Node (rank, deleted, t, a, b) ->
                    inner a (fun aacc -> 
                        inner b (fun bacc ->
                            cont (func rank deleted t aacc bacc)))
            inner heap id

        /// Finds the minimum element, None for empty heap.
        /// Deletes marked elements from the heap.
        let findMin heap = 
            fold (fun rank deleted t (amin, a) (bmin, b) ->
                let heap =
                    if deleted
                        then (merge a b)
                        else Node (rank, deleted, t, a, b)
                let min =
                    [ amin; bmin; (if deleted then None else Some t)]
                    |> List.choose id
                    |> fun coll ->
                        if List.isEmpty coll
                            then None
                            else Some <| List.min coll
                                        
                (min, heap)) (None, Empty) heap

        // Checks if the heap is empty; implemented in terms of fold due to lazy deletion
        let isEmpty heap =
            fold (fun _ deleted _ isLeftEmpty isRightEmpty -> deleted && isLeftEmpty && isRightEmpty) true heap

        /// Marks all occurences of an element td for deletion.
        /// Since the heap implements lazy deletion, the elements are only marked for deletion.
        let rec delete td heap =
            match heap with
            | Node (rank, deleted, t, a, b) when not deleted ->
                Node (rank, (t = td), t, delete td a, delete td b) 
            | Node (rank, deleted, t, a, b) -> 
                Node (rank, deleted, t, delete td a, delete td b)
            | Empty ->
                Empty

        /// Removes the minimum element from the heap.
        /// Deletes marked elements from the heap.
        let deleteMin heap =
            findMin heap
            |> fun (min, heap) ->
                match min with
                | Some m -> delete m heap
                | None   -> heap

        /// Converts a sequence into a leftist heap.
        let fromSeq coll =
            let singletons =
                coll 
                |> Seq.map(fun elem -> Node (1, false, elem, Empty, Empty))
                |> List.ofSeq
            let rec inner heaps acc = 
                match heaps, acc with
                | [],   []  -> Empty
                | [],   [a] -> a
                | [],   acc -> inner acc [] 
                | [s],  acc     -> inner [] (s::acc)
                | a::b::t, acc  -> inner t ((merge a b)::acc)
            inner singletons []                

        /// Converts a heap into a sorted sequence.
        let toSeq heap =
            let rec inner heap acc = 
                let min, _ = findMin heap
                match min with
                | None -> List.rev acc
                | Some m -> inner (deleteMin heap) (m::acc)
            inner heap []
            |> Seq.ofList

module ShortestPath =
    module Dijkstra =
        type VertexLookupInfo = 
            {
                predecessor: Vertex option
                distance: double
            }
            static member Empty = 
                {
                    predecessor = None
                    distance = 0.0
                }

        open Collections

        let buildLookup graph source = 
            match graph with
            | Graphs.Graph (vertices, edges) -> 
                // adjacency list for the graph
                let adjacencyList = Graphs.buildAdjacencyList graph

                // function that gets the neighbouring nodes reachable from a vertex
                let neighbours = Graphs.neighbours (Graphs.buildAdjacencyList graph)

                // set up the initial state of priority queue and result lookup
                let resultLookup, queue =
                    vertices 
                    |> Set.map (fun vertex ->
                        if vertex = source
                            then vertex, { predecessor = None; distance = 0.0 }
                            else vertex, { predecessor = None; distance = infinity })
                    |> fun infos ->
                        ResultLookup (infos |> Map.ofSeq, VertexLookupInfo.Empty), 
                        infos |> Set.fold (fun acc (vertex, info) -> acc |> Heap.insert (info.distance, vertex)) Heap.Empty 
                
                let rec visit queue (resultLookup : ResultLookup<_>) = 
                    // while queue is not empty...
                    if Heap.isEmpty queue
                        then resultLookup.Unwrap
                        else
                            // ...grab a vertex and its info
                            let current, queue = 
                                queue 
                                |> (Heap.findMin >> fun (min, heap) -> 
                                    min 
                                    // we have checked if the queue is empty before, so findMin has to return Some
                                    |> Option.get 
                                    |> fun (_, vertex) -> vertex, Heap.deleteMin heap)
                            
                            // get a set of edges going out from current vertex together with weights
                            let edges = 
                                match adjacencyList with 
                                | L map -> map |> (Map.find current >> Map.ofSeq)

                            // fold over its neighbours, updating results lookup
                            let updatedLookup, updatedQueue =
                                neighbours current
                                |> List.fold (fun (lookup : ResultLookup<_>, queue) vertex -> 
                                    let getDistance v = lookup.Find v |> fun info -> info.distance

                                    let distance        = getDistance current + Map.find vertex edges
                                    let currentDistance = getDistance vertex

                                    if distance < currentDistance
                                        then 
                                            let updatedQueue = 
                                                queue
                                                |> Heap.delete (currentDistance, vertex)
                                                |> Heap.insert (distance, vertex)

                                            let updatedLookup =
                                                lookup.Update vertex <| fun info -> 
                                                    { info with predecessor = Some current; distance = distance}

                                            (updatedLookup, updatedQueue)                                            
                                        else 
                                            (lookup, queue)) (resultLookup, queue)
                            
                            // try processing another vertex from the queue
                            visit updatedQueue updatedLookup

                visit queue resultLookup

            | other -> failwithf "Not a graph: %A" other

//--------------------------------------------------------------------------------------
type Direction =
    | Left
    | Right
    | Build
    | Unknown
    override x.ToString() =
        match x with
        | Left -> "LEFT"
        | Right -> "RIGHT"
        | Build -> "ELEVATOR"
        | Unknown -> ""

let getDirection dir =
    match dir with
    | "LEFT" -> Left
    | "RIGHT" -> Right
    | _ -> Unknown

type Clone =
    { Floor : int
      Position: int
      Direction : Direction }
    override x.ToString() =
        String.Format("Clone Floor: {0} - POS: {1} - Direction: {2}", x.Floor, x.Position, x.Direction)

type Elevator =
    { Floor : int
      Position : int }
    member x.Name = sprintf "%i-%i" x.Floor x.Position
    override x.ToString() =
        String.Format("Floor: {0} - Position: {1}", x.Floor, x.Position)

type LevelDetails =
    { Floors : int
      Width : int
      Rounds : int
      ExitFloor: int
      ExitPos : int
      TotalClones : int
      AdditionalElevators : int
      TotalElevators : int
      Elevators : Elevator list}
    override x.ToString() =
        String.Format("Floors: {1}{0}Width: {2}{0}Rounds: {3}{0}Exit Floor: {4}{0}Exit Pos: {5}{0}Total Clones: {6}{0}Additional Elevators: {7}{0}Total Elevators: {8}{0}",
            Environment.NewLine,
            x.Floors,
            x.Width,
            x.Rounds,
            x.ExitFloor,
            x.ExitPos,
            x.TotalClones,
            x.AdditionalElevators,
            x.TotalElevators
            )

let mutable elevatorsBuilt = 0

let readNLines n = List.init n (fun _ -> Console.ReadLine())

let getElevators n =
    let lines = n |> readNLines 
    let elevators = lines |> List.map (fun l -> 
        let token = l.Split[|' '|] 
        { Floor = int(token.[0]); Position = int(token.[1])})
    elevators |> List.sortBy (fun e -> e.Floor)

let (|ExitAbove|ExitBelow|ExitSame|) ((clone : Clone), level) =
    match clone.Floor, level.ExitFloor with
    | c, e when e > c -> ExitAbove
    | c, e when e < c -> ExitBelow
    | c, e when e = c -> ExitSame
    | _ -> failwith "Unable to determine exit vs clone floor."

let (|TargetLeft|TargetRight|TargetSame|) (targetPos, (clone : Clone)) =
    match targetPos, clone.Position with
    | tp, cp when tp > cp -> TargetRight
    | tp, cp when tp < cp -> TargetLeft
    | _ -> TargetSame

let getTargetDirection targetPos clone =
    match targetPos, clone with
    | TargetLeft -> Direction.Left
    | TargetRight -> Direction.Right
    | TargetSame -> Direction.Unknown

let getElevatorDirection (clone : Clone) level =
    //Console.Error.WriteLine(sprintf "%A" level.Elevators)
    //Console.Error.WriteLine(sprintf "%i" elevatorsBuilt)
    let currentFloorElevator =
        level.Elevators 
        |> List.filter (fun e -> e.Floor = clone.Floor)
        |> List.sortBy (fun e -> abs (e.Position - clone.Position))
        |> List.tryHead

    match currentFloorElevator with
    | Some e -> getTargetDirection e.Position clone
    | None   -> if elevatorsBuilt < level.AdditionalElevators 
                then 
                    elevatorsBuilt <- elevatorsBuilt + 1 
                    Direction.Build
                else Direction.Unknown
    
let getExitDirection (clone : Clone) level =
    getTargetDirection level.ExitPos clone
    
let getMovementDirection (clone : Clone) level =
    if 
        clone.Floor < 0 then Direction.Unknown
    else
        match clone.Floor, level.ExitFloor with
        | cf, ef when cf = ef -> getExitDirection clone level
        | _ -> getElevatorDirection clone level

type MoveAction =
    | Wait
    | Block
    | Build

let getMoveAction (clone : Clone) direction =
    match clone.Direction, direction with
    | _, d when d = Direction.Unknown -> Wait
    | _, d when d = Direction.Build -> Build
    | c, d when c = d -> Wait
    | _ -> Block

let writeMoveAction move =
    match move with
    | Wait -> printfn "WAIT"
    | Block -> printfn "BLOCK"
    | Build -> printfn "ELEVATOR"

(* nbFloors: number of floors *)
(* width: width of the area *)
(* nbRounds: maximum number of rounds *)
(* exitFloor: floor on which the exit is found *)
(* exitPos: position of the exit on its floor *)
(* nbTotalClones: number of generated clones *)
(* nbAdditionalElevators: *)
(* nbElevators: number of elevators *)
let levelInput = (Console.In.ReadLine()).Split [|' '|]

let mutable level =
    { Floors = int(levelInput.[0])
      Width = int(levelInput.[1])
      Rounds = int(levelInput.[2])
      ExitFloor = int(levelInput.[3])
      ExitPos = int(levelInput.[4])
      TotalClones = int(levelInput.[5])
      AdditionalElevators = int(levelInput.[6])
      TotalElevators = int(levelInput.[7])
      Elevators = getElevators (int(levelInput.[7]))}

let graph =
    let vertices =
        level.Elevators
        |> List.map (fun e -> e.Name |> Vertex.Label)
        |> Set.ofList
    let edges = 
        level.Elevators
        |> List.collect (fun e -> 
            [ for x in level.Elevators do
                if x.Floor = e.Floor 
                then yield (Label e.Name, Label x.Name), double <| abs(e.Position - x.Position) ])
        |> Set.ofList
    (vertices, edges)
    |> Graph.Directed

let exit = level.Elevators |> List.find (fun e -> e.Floor = level.ExitFloor && e.Position = level.ExitFloor)

let lookup = ShortestPath.Dijkstra.buildLookup graph (Label exit.Name)

(* game loop *)
while true do
    (* cloneFloor: floor of the leading clone *)
    (* clonePos: position of the leading clone on its floor *)
    (* direction: direction of the leading clone: LEFT or RIGHT *)
    let turnInput = (Console.In.ReadLine()).Split [|' '|]
    
    let leadClone = { Floor = int(turnInput.[0]); Position = int(turnInput.[1]); Direction = getDirection turnInput.[2] }
    
    Console.Error.WriteLine(sprintf "%A" level)

    let moveAction = getMovementDirection leadClone >> getMoveAction leadClone

    let ma = level |> moveAction
    writeMoveAction ma

    // So ugly
    match ma with
    | MoveAction.Build -> 
        level <- 
            ({ level with Elevators = level.Elevators @ [{ Floor = leadClone.Floor; Position = leadClone.Position }] })
    | _ -> ()

    ()