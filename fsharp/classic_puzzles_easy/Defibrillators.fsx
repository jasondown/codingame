// https://www.codingame.com/training/easy/defibrillators

open System

type Defibrillator = {
    Name : string
    Longitute : float
    Latitude : float   
}

let fr = new System.Globalization.CultureInfo("fr-FR")
let lonLatFormat text = Double.Parse(text, fr)

let getDist (lon, lat) defib =
    let earthRadius = 6371.0
    let x = (lon - defib.Longitute) * cos ((lat + defib.Latitude) / 2.0)
    let y = lat - defib.Latitude
    sqrt (x*x + y*y) * earthRadius

let getDefibs num =
    Seq.init num (fun _ -> Console.ReadLine())
    |> Seq.map (fun l -> 
        let token = l.Split ';'
        { Name      = token.[1]
          Longitute = token.[4] |> lonLatFormat
          Latitude  = token.[5] |> lonLatFormat })
        
//----------main logic
let defib = 
    let userLoc = Console.ReadLine() |> lonLatFormat, 
                  Console.ReadLine() |> lonLatFormat
    let numDefibs = Console.ReadLine() |> int

    numDefibs |> getDefibs |> Seq.minBy (getDist userLoc)

printfn "%s" defib.Name