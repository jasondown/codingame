// Dijkstra's Algorithm:
//----------------------
//----------------------
//
// Data -> Data Structure
//----------------------- 
// Distance Table -> 3-column array
// Backtracking -> Stack
// Enqueuing neighbours -> Priority queue
//
// Priority Queue options:
//------------------------
// 
// Binary Heap O(E lg(V))
//
// Array O(E + V^2)
//
//
// Going to convert priority queue from https://algs4.cs.princeton.edu/24pq/MaxPQ.java.html 
// (Java) to F#
//
module PriorityQueue =
    type PriorityQueue<'T when 'T :> System.IComparable<'T>> (capacity : int) =
        let pq : 'T array  = Array.zeroCreate (capacity + 1)
        let mutable N = 0

        let less i j = pq.[i].CompareTo(pq.[j]) < 0

        let exch i j =
            let temp = pq.[i]
            pq.[i] <- pq.[j]
            pq.[j] <- temp
            i

        let rec swim k =
            match (k > 1 && less (k/2) k) with
            | true -> exch (k/2) k |> swim
            | false -> ()

        let rec sink k =
            match 2*k <= N, 2*k with
            | true, j when j < N && less j (j+1) ->
                match less k (j+1), j+1 with
                | true, x -> exch k x |> ignore
                             sink x
                | false, _ -> ()
            | true, j -> exch k j |> ignore
                         sink j
            | false, _ -> ()

        member __.IsEmpty with get() = N = 0
        member __.Size with get() = N
        member __.Insert (item : 'T) = N <- N+1
                                       pq.[N] <- item
                                       swim N

        member __.DelMax () =
            let max = pq.[1]
            exch 1 N |> ignore
            N <- N-1
            pq.[N+1] <- Unchecked.defaultof<'T>
            sink 1
            max                 