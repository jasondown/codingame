// https://www.codingame.com/training/easy/mime-type

open System
open System.IO

let readNLines n = List.init n (fun _ -> Console.ReadLine())

let pairs (s : seq<string>) =
    s 
    |> Seq.pairwise 
    |> Seq.mapi (fun i x -> i % 2 = 0, x) 
    |> Seq.filter fst 
    |> Seq.map (fun s -> (s |> snd |> fst).ToLowerInvariant() , (s |> snd |> snd))
    |> List.ofSeq

let getMimeTypes num =
    num
    |> readNLines
    |> List.collect (fun line -> line.Split([|' '|]) |> Array.toList)
    |> pairs
    |> Map.ofList
    
let getFilenames num =
    num
    |> readNLines
    |> List.map (fun f -> new FileInfo(f))

let findMimeTypes numMimes numFiles =
    let mimeTypes = getMimeTypes numMimes
    getFilenames numFiles
    |> List.map (fun f -> 
        let ext = f.Extension.Replace(".", "").ToLowerInvariant()
        if mimeTypes.ContainsKey ext then mimeTypes.[ext]
        else "UNKNOWN")

let numMimeTypes = int(Console.In.ReadLine()) 
let numFiles = int(Console.In.ReadLine())
(numMimeTypes, numFiles) ||> findMimeTypes |> List.iter (fun s -> printfn "%s" s)