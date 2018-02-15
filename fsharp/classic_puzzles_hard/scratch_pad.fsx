open System

type Vertex =
    | Label of string

type Edge = (Vertex * Vertex) * double

type Graph = Set<Vertex> * Set<Vertex>