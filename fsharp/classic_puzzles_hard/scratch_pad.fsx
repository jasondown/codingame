open System

type Vertex =
    | Label of string

type Edge = (Vertex * Vertex) * double

type Graph = Set<Vertex> * Set<Vertex>

type AdjacencyList<'TLabel when 'TLabel : comparison> = 
    L of Map<Vertex, Set<(Vertex * 'TLabel)>>

// May support directed vs undirected later
let (|Graph|_|) graph = 
    match graph with
    |  (v, e)   -> Some (v, e)

let getEdges = function 
    | Graph (_, e) -> e
    | x -> failwithf "Not a graph: %A" x
