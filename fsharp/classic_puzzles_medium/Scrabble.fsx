// https://www.codingame.com/training/medium/scrabble

open System
open System.Collections.Generic

let pointValues = 
    dict ['e',1; 'a',1; 'i',1; 'o',1; 'n',1; 'r',1; 't',1; 'l',1; 's',1; 'u',1;
          'd',2; 'g',2;
          'b',3; 'c',3; 'm',3; 'p',3;
          'f',4; 'h',4; 'v',4; 'w',4; 'y',4;
          'k',5;
          'j',8; 'x',8;
          'q',10; 'z',10; 
    ]

let getHash (word : string) =
    let count c = Seq.filter ((=) c) >> Seq.length
    word |> Seq.distinct |> Seq.map (fun c -> c, (word |> count c)) |> dict

let getScore (points : IDictionary<char, int>) (letters : IDictionary<char, int>) word =
    let mutable total = 0
    let wordHash = getHash word
    for kvp in wordHash do
        let c = kvp.Key
        if (total = -1 || not <| letters.ContainsKey c || letters.[c] < wordHash.[c])
        then total <- -1
        else total <- total + wordHash.[c] * points.[c]
    total

let getBestWord points letters words =
    words |> List.maxBy (fun w -> getScore points letters w)

let getLettersAndWords wordCount =
    let validWords = 
        List.init wordCount (fun _ -> Console.ReadLine())
        |> List.filter (fun w -> w.Length <= 7)
    let letters = Console.ReadLine() |> getHash
    (letters, validWords)

//----------- main logic     
Console.ReadLine() |> int |> getLettersAndWords ||> getBestWord pointValues |> printfn "%s" 