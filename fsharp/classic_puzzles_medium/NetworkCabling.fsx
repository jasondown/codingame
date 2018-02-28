// https://www.codingame.com/training/medium/network-cabling

open System
let readNLines n = List.init n (fun _ -> Console.In.ReadLine())

type Building = {X : int64; Y: int64}

let median (blds : Building list) =
    let ySorted = 
        blds
        |> List.sortBy (fun b -> b.Y)
    
    ySorted.[int(Math.Round(float(blds.Length - 1) / 2.0))]

let getMainLineLength buildings =
    let minX =
        buildings
        |> List.minBy (fun b -> b.X) 
    let maxX = 
        buildings
        |> List.maxBy (fun b -> b.X)

    Math.Abs(minX.X - maxX.X)

let getMainLineY buildings = (buildings |> median).Y
    
let getTotalYLengths buildings =
    let mainLine = buildings |> getMainLineY
    buildings
    |> List.fold (fun total b -> 
        total + (match b.Y with
                 | y when y > mainLine -> Math.Abs(mainLine - y)
                 | y when y < mainLine -> Math.Abs(y - mainLine)
                 | _ -> 0L)) 
        0L

let getTotalLength buildings =
    (buildings |> getMainLineLength) + (buildings |> getTotalYLengths)

let getBuildings num = 
    num
    |> readNLines 
    |> List.map (fun l ->
        let token = l.Split[|' '|]
        { X = int64(token.[0]); Y = int64(token.[1]) })
    |> List.sortByDescending (fun b -> b.X)
    //|> List.rev

// Main Logic
Console.ReadLine()
|> int
|> getBuildings 
|> getTotalLength 
|> printfn "%i"