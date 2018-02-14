// https://www.codingame.com/training/hard/don't-panic-episode-2

open System

type Direction =
    | Left
    | Right
    | Build
    | Unknown
    override x.ToString() =
        match x with
        | Left -> "LEFT"
        | Right -> "RIGHT"
        | Build -> "ELEVATOR"
        | Unknown -> ""

let getDirection dir =
    match dir with
    | "LEFT" -> Left
    | "RIGHT" -> Right
    | _ -> Unknown

type Clone =
    { Floor : int
      Position: int
      Direction : Direction }
    override x.ToString() =
        String.Format("Clone Floor: {0} - POS: {1} - Direction: {2}", x.Floor, x.Position, x.Direction)

type Elevator =
    { Floor : int
      Position : int }
    override x.ToString() =
        String.Format("Floor: {0} - Position: {1}", x.Floor, x.Position)

type LevelDetails =
    { Floors : int
      Width : int
      Rounds : int
      ExitFloor: int
      ExitPos : int
      TotalClones : int
      AdditionalElevators : int
      TotalElevators : int
      Elevators : Elevator list}
    override x.ToString() =
        String.Format("Floors: {1}{0}Width: {2}{0}Rounds: {3}{0}Exit Floor: {4}{0}Exit Pos: {5}{0}Total Clones: {6}{0}Additional Elevators: {7}{0}Total Elevators: {8}{0}",
            Environment.NewLine,
            x.Floors,
            x.Width,
            x.Rounds,
            x.ExitFloor,
            x.ExitPos,
            x.TotalClones,
            x.AdditionalElevators,
            x.TotalElevators
            )

let mutable elevatorsBuilt = 0

let readNLines n = List.init n (fun _ -> Console.ReadLine())

let getElevators n =
    let lines = n |> readNLines 
    let elevators = lines |> List.map (fun l -> 
        let token = l.Split[|' '|] 
        { Floor = int(token.[0]); Position = int(token.[1])})
    elevators |> List.sortBy (fun e -> e.Floor)

let (|ExitAbove|ExitBelow|ExitSame|) ((clone : Clone), level) =
    match clone.Floor, level.ExitFloor with
    | c, e when e > c -> ExitAbove
    | c, e when e < c -> ExitBelow
    | c, e when e = c -> ExitSame
    | _ -> failwith "Unable to determine exit vs clone floor."

let (|TargetLeft|TargetRight|TargetSame|) (targetPos, (clone : Clone)) =
    match targetPos, clone.Position with
    | tp, cp when tp > cp -> TargetRight
    | tp, cp when tp < cp -> TargetLeft
    | _ -> TargetSame

let getTargetDirection targetPos clone =
    match targetPos, clone with
    | TargetLeft -> Direction.Left
    | TargetRight -> Direction.Right
    | TargetSame -> Direction.Unknown

let getElevatorDirection (clone : Clone) level =
    //Console.Error.WriteLine(sprintf "%A" level.Elevators)
    //Console.Error.WriteLine(sprintf "%i" elevatorsBuilt)
    let currentFloorElevator =
        level.Elevators 
        |> List.filter (fun e -> e.Floor = clone.Floor)
        |> List.sortBy (fun e -> abs (e.Position - clone.Position))
        |> List.tryHead

    match currentFloorElevator with
    | Some e -> getTargetDirection e.Position clone
    | None   -> if elevatorsBuilt < level.AdditionalElevators 
                then 
                    elevatorsBuilt <- elevatorsBuilt + 1 
                    Direction.Build
                else Direction.Unknown
    
let getExitDirection (clone : Clone) level =
    getTargetDirection level.ExitPos clone
    
let getMovementDirection (clone : Clone) level =
    if 
        clone.Floor < 0 then Direction.Unknown
    else
        match clone.Floor, level.ExitFloor with
        | cf, ef when cf = ef -> getExitDirection clone level
        | _ -> getElevatorDirection clone level

type MoveAction =
    | Wait
    | Block
    | Build

let getMoveAction (clone : Clone) direction =
    match clone.Direction, direction with
    | _, d when d = Direction.Unknown -> Wait
    | _, d when d = Direction.Build -> Build
    | c, d when c = d -> Wait
    | _ -> Block

let writeMoveAction move =
    match move with
    | Wait -> printfn "WAIT"
    | Block -> printfn "BLOCK"
    | Build -> printfn "ELEVATOR"

(* nbFloors: number of floors *)
(* width: width of the area *)
(* nbRounds: maximum number of rounds *)
(* exitFloor: floor on which the exit is found *)
(* exitPos: position of the exit on its floor *)
(* nbTotalClones: number of generated clones *)
(* nbAdditionalElevators: *)
(* nbElevators: number of elevators *)
let levelInput = (Console.In.ReadLine()).Split [|' '|]

let mutable level =
    { Floors = int(levelInput.[0])
      Width = int(levelInput.[1])
      Rounds = int(levelInput.[2])
      ExitFloor = int(levelInput.[3])
      ExitPos = int(levelInput.[4])
      TotalClones = int(levelInput.[5])
      AdditionalElevators = int(levelInput.[6])
      TotalElevators = int(levelInput.[7])
      Elevators = getElevators (int(levelInput.[7]))}

(* game loop *)
while true do
    (* cloneFloor: floor of the leading clone *)
    (* clonePos: position of the leading clone on its floor *)
    (* direction: direction of the leading clone: LEFT or RIGHT *)
    let turnInput = (Console.In.ReadLine()).Split [|' '|]
    
    let leadClone = { Floor = int(turnInput.[0]); Position = int(turnInput.[1]); Direction = getDirection turnInput.[2] }
    
    Console.Error.WriteLine(sprintf "%A" level)

    let moveAction = getMovementDirection leadClone >> getMoveAction leadClone

    let ma = level |> moveAction
    writeMoveAction ma

    // So ugly
    match ma with
    | MoveAction.Build -> 
        level <- 
            ({ level with Elevators = level.Elevators @ [{ Floor = leadClone.Floor; Position = leadClone.Position }] })
    | _ -> ()

    ()