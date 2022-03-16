open System
open System.Reflection

type HealthPoints = int
type Id = int
type Rank = int
type Token = string array

type Player = 
    | Me | Enemy | Cloud
    static member Create id =
        match id with
        | 0  -> Player.Me
        | 1  -> Player.Enemy
        | -1 -> Player.Cloud
        | i -> failwithf "Unknown player id: %i" i

type MoleculeType = 
    | A | B | C | D | E
    override x.ToString () =
        match x with
        | A -> "A"
        | B -> "B"
        | C -> "C"
        | D -> "D"
        | E -> "E"

type MoleculeStorage =
    { Counts : Map<MoleculeType, int> }
    static member Create (token : Token) =
        { Counts = 
            Map [ (MoleculeType.A, (int <| token.[0]))
                  (MoleculeType.B, (int <| token.[1])) 
                  (MoleculeType.C, (int <| token.[2]))
                  (MoleculeType.D, (int <| token.[3]))
                  (MoleculeType.E, (int <| token.[4])) ]
        }

type Module =
    | StartPos
    | Samples
    | Diagnosis
    | Molecules
    | Laboratory
    static member Create moduleName =
        match moduleName with
        | "START_POS"  -> Module.StartPos
        | "SAMPLES"    -> Module.Samples
        | "DIAGNOSIS"  -> Module.Diagnosis
        | "MOLECULES"  -> Module.Molecules
        | "LABORATORY" -> Module.Laboratory
        | name         -> failwithf "Unknown module type: %s" name
    override x.ToString () =
        match x with
        | StartPos   -> "START_POS"
        | Samples    -> "SAMPLES"
        | Diagnosis  -> "DIAGNOSIS"
        | Molecules  -> "MOLECULES"
        | Laboratory -> "LABORATORY"

type Robot =
    { Player : Player
      Location : Module
      HealthPoints : HealthPoints
      Molecules : MoleculeStorage }
    static member Create (token : Token) (player : Player) =
        { Player = player
          Location = Module.Create token.[0]
          // eta : ignore (token.[1]) for wood 2
          HealthPoints = int <| token.[2]
          Molecules = MoleculeStorage.Create token.[3 .. 7]
          // expertise : ignore (token.[8 .. 12]) for wood 2 
        }

type DiagnosedSampleData =
    { Id : Id
      CarriedBy : Player
      Rank : Rank
      HealthPoints : HealthPoints
      Molecules : MoleculeStorage }
    static member Create (token : Token) =
        { Id = (int <| token.[0])
          CarriedBy = Player.Create (int <| token.[1])
          Rank = (int <| token.[2])
          // gain : ignore (token.[3]) for wood 2
          HealthPoints = (int <| token.[4])
          Molecules = MoleculeStorage.Create token.[5 .. 9] 
        }

type UndiagnosedSampleData =
    { Id : Id
      CarriedBy : Player 
      Rank : Rank }
    static member Create (token : Token) =
        { Id = (int <| token.[0])
          CarriedBy = Player.Create (int <| token.[1]) 
          Rank = (int <| token.[2]) 
        }

type SampleData =
    | Diagnosed of DiagnosedSampleData
    | Undiagnosed of UndiagnosedSampleData
    member x.HasBeenDiagnosed =
        match x with
        | SampleData.Diagnosed d -> Some d
        | _ -> None
    member x.NeedsToBeDiagnosed =
        match x with
        | SampleData.Undiagnosed u -> Some u
        | _ -> None
    member x.CarriedBy =
        match x with
        | SampleData.Diagnosed d -> d.CarriedBy
        | SampleData.Undiagnosed u -> u.CarriedBy
    static member Create (token : Token) =
        if token.[4] |> int = -1 then
            SampleData.Undiagnosed <| UndiagnosedSampleData.Create token
        else
            SampleData.Diagnosed <| DiagnosedSampleData.Create token

type GameState =
    { Robots : Robot list
      Samples : SampleData list }

type CollectNewSample = Rank -> string
type CollectCloudSample = DiagnosedSampleData -> string
type Analyze = UndiagnosedSampleData -> string
type Gather = MoleculeType -> string
type Produce = DiagnosedSampleData -> string
type Goto = Module -> string

let collectNewSample : CollectNewSample = fun r -> sprintf "CONNECT %i" <| r
let collectCloudSample : CollectCloudSample = fun s -> sprintf "CONNECT %i" <| s.Id
let anaylze : Analyze = fun s -> sprintf "CONNECT %i" <| s.Id
let gather : Gather = fun m -> sprintf "CONNECT %s" <| m.ToString()
let produce : Produce = fun s -> sprintf "CONNECT %i" s.Id
let goto : Goto = fun m -> sprintf "GOTO %s" <| m.ToString()

// Helper functions
let readInput () = Console.ReadLine()
let readInt = readInput >> int
let tokenize (line : string) = line.Split ' '
let tokenizeInput = readInput >> tokenize
let readNLines n = Array.init n (fun _ -> readInput())

// Turn-based functions
let canMakeSample (robot : Robot) (sample : DiagnosedSampleData) =
    sample.Molecules.Counts
    |> Map.forall (fun mt count -> robot.Molecules.Counts.[mt] >= count)

let getRequiredMolecule (robot : Robot) (sample : DiagnosedSampleData) =
    let ms =
        sample.Molecules.Counts
        |> Map.filter (fun mt count -> robot.Molecules.Counts.[mt] < count)
        |> Seq.head
    ms.Key

let getMove (gs : GameState) =
    let me = gs.Robots |> List.find (fun r -> r.Player = Player.Me)
    let mySamples = gs.Samples |> List.filter (fun s -> s.CarriedBy = Player.Me)
    let myUndiagnosedSamples = mySamples |> List.choose (fun s -> s.NeedsToBeDiagnosed)
    let myDiagnosedSamples = mySamples |> List.choose (fun s -> s.HasBeenDiagnosed)
    let samplesReady = myDiagnosedSamples |> List.filter (canMakeSample me)

    let cloudSamples = 
        gs.Samples
        |> List.filter (fun s -> s.CarriedBy = Player.Cloud)
        |> List.choose (fun s -> s.HasBeenDiagnosed)
        |> List.sortByDescending (fun s -> s.HealthPoints)
    
    match myUndiagnosedSamples.Length, myDiagnosedSamples.Length, samplesReady.Length, cloudSamples.Length, me.Location with
    | _, _, sr, _, Module.Laboratory when sr > 0 ->
        produce samplesReady.Head
    | _, _, sr, _, _ when sr > 0 -> 
        goto Module.Laboratory

    | us, ds, _, cs, Module.Diagnosis when us = 0 && ds = 0 && cs > 0 -> 
        collectCloudSample cloudSamples.Head
    | us, ds, _, cs, _ when us = 0 && ds = 0 && cs > 0 -> 
        goto Module.Diagnosis

    | us, ds, _, _, Module.Diagnosis when us > 0 && ds = 0 -> 
        anaylze myUndiagnosedSamples.Head
    | us, ds, _, _, _ when us > 0 && ds = 0 -> 
        goto Module.Diagnosis

    | us, ds, _, _, Module.Samples when us = 0 && ds = 0 -> 
        collectNewSample 3 // just grab highest rank
    | us, ds, _, _, _ when us = 0 && ds = 0 -> 
        goto Module.Samples

    | _, _, _, _, Module.Molecules -> 
        gather (getRequiredMolecule me myDiagnosedSamples.Head)
    | _, _, _, _, _ -> 
        goto Module.Molecules
    
// ignore project count stuff for wood 2
let projectCount = readInt()
for i in 0 .. projectCount - 1 do
    let _ = readInput()
    ()

(* game loop *)
while true do
    let me = Robot.Create (tokenizeInput()) Player.Me
    let enemy = Robot.Create (tokenizeInput()) Player.Enemy
    
    // ignore input for available molecules for wood 2
    let _ = readInput()

    let samples = 
        readInt()
        |> readNLines
        |> Array.map (tokenize >> SampleData.Create)
        |> Array.toList

    //eprintf "%A" samples

    let gameState = 
        { Robots = [me; enemy]
          Samples = samples }
    
    gameState |> getMove |> printfn "%s"
