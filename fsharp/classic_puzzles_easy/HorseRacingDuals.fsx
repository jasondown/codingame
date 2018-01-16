// https://www.codingame.com/training/easy/horse-racing-duals

open System

let readNIntLines n = List.init n (fun _ -> int(Console.ReadLine()))

let getHorseStrengths num = 
    num
    |> readNIntLines
    
let delta (horsePair : int * int) = Math.Abs((fst horsePair) - (snd horsePair))

let getClosesMatchup strengths = 
    strengths
    |> Seq.sort
    |> Seq.pairwise
    |> Seq.minBy delta

// Main logic
let horseCount = int(Console.In.ReadLine())
horseCount |> getHorseStrengths |> getClosesMatchup |> delta |> printfn "%i"