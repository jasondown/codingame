// https://www.codingame.com/training/easy/the-descent
//----------------------------------------------------

open System

[<Literal>]
let AllMountains = 8

type Mountain = { Index : int; Height : int}

let readNLines n = List.init n (fun _ -> Console.ReadLine())

let getMountains totalLines =
    totalLines
    |> readNLines
    |> List.mapi (fun i line -> {Index = i; Height = int line})

let highestMountain = getMountains >> List.maxBy (fun m -> m.Height)

(* game loop *)
while true do  
    let target = AllMountains |> highestMountain  
    printfn "%i" target.Index
    ()