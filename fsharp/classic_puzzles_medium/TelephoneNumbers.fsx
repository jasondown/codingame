// https://www.codingame.com/training/medium/telephone-numbers

open System

let readNLines n = Array.init n (fun _ -> Console.ReadLine())
let phoneNumbers = 
    readNLines (int <| Console.ReadLine())
    |> Array.sortBy id

let commonLeadingSubstringLength (s : string * string) =
    let s1 = (fst s).ToCharArray()
    let s2 = (snd s).ToCharArray()
    s2.Length - (Seq.zip s1 s2
                 |> Seq.takeWhile (fun x -> fst x = snd x)
                 |> Seq.length)

let nodes =
    (Array.head phoneNumbers).Length + 
    (Array.pairwise phoneNumbers
    |> Array.sumBy commonLeadingSubstringLength)
    
printfn "%i" nodes