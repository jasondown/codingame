// https://www.codingame.com/training/easy/chuck-norris

open System
open System.Text

let buildOutput (stringBytes : string) =
    let unary = 
        let rec build lastNum index output =
            match index < stringBytes.Length with
            | false -> output
            | true ->
                let currNum = string <| stringBytes.[index]
                if currNum = lastNum then 
                    build currNum (index + 1) (String.Concat (output, "0"))
                else 
                    match currNum with
                    | "1" -> build currNum (index + 1) (String.Concat (output, " 0 0"))
                    | _ -> build currNum (index + 1) (String.Concat (output, " 00 0"))
        build String.Empty 0 String.Empty
    unary.TrimStart()
    
//----------main logic
let bytes = Console.ReadLine() |> Encoding.ASCII.GetBytes
String.Join("", bytes |> Array.map (fun b -> Convert.ToString(b, 2).PadLeft(7, '0')))
|> buildOutput
|> printfn "%s"