type 'a Tree =
    | Empty
    | Branch of 'a * 'a Tree * 'a Tree

let rec bfs = function
    | [] -> []
    | Empty :: trees -> bfs trees
    | Branch (x, left, right) :: trees -> x :: bfs [ yield! trees; yield left; yield right ]

let breadthFirstSearch tr = bfs [tr]

let tree1 = Branch ('a', Branch ('b', Branch ('d', Empty, Empty),
                               Branch ('e', Empty, Empty)),
                         Branch ('c', Empty,
                               Branch ('f', Branch ('g', Empty, Empty),
                                           Empty))) 

printfn "%A" (breadthFirstSearch tree1)