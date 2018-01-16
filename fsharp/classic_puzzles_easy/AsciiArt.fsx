// https://www.codingame.com/training/easy/ascii-art

open System

let width = int(Console.In.ReadLine())
let height = int(Console.In.ReadLine())
let text = Console.In.ReadLine().ToUpperInvariant()

let rows = Array.init height (fun _ -> String.Empty)

for i in 0 .. height - 1 do 
    rows.[i] <- Console.ReadLine()

let validLetters = [ 'A' .. 'Z' ] @ ['?']

let letterStart letter width =
    let pos = 
        if List.exists ((=) letter) validLetters then
            List.findIndex (fun c -> c = letter) validLetters
        else 
            List.findIndex (fun c -> c = '?') validLetters
    pos * width

let getLetter letter row width =
    rows.[row].Substring((letterStart letter width), width)

for h in 0 .. height - 1 do
    for c in text do
        Console.Write (getLetter c h width)
    Console.WriteLine("")