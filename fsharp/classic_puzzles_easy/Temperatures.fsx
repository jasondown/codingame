// https://www.codingame.com/training/easy/temperatures

open System

let N = int(Console.In.ReadLine())
let TEMPS = Console.In.ReadLine()

let tempsArray = 
    if N = 0 then []
    else TEMPS.Split(' ') |> Array.toList |> List.map (fun s -> int (s))

let mutable currentClosest = Int32.MaxValue

let rec findClosestTo0 l =
    match l with
    | [] -> currentClosest
    | t::ts ->
        let res = abs(t)
        if abs(currentClosest) > res then currentClosest <- t
        elif res = abs(currentClosest) && t > 0 then currentClosest <- t
        else ()
        findClosestTo0 ts

let printClosest l =
    match l with
        | [] -> printfn "0"
        | _ -> printfn "%i" (findClosestTo0 l)

printClosest tempsArray