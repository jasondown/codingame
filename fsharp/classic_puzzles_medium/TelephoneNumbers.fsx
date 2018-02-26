// https://www.codingame.com/training/medium/telephone-numbers

open System

let readNLines n = Seq.init n (fun _ -> Console.ReadLine())

let totalNums = Console.ReadLine() |> int
let phoneNumbers = 
    readNLines totalNums
    |> Seq.sortBy id

let nodes =
    let numbers = Seq.skip 1 phoneNumbers
    let zipped = Seq.zip numbers Tuple.Create
