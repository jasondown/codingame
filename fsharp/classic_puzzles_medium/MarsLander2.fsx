open System

type Token = string array

type Point =
    { X: int
      Y: int }
    static member Create(token: Token) =
        { X = token.[0] |> int
          Y = token.[1] |> int }

type FlatArea =
    { LeftX: int
      MiddleX: int
      RightX: int }

type Direction =
    | Left = -1
    | Right = 1
    | None = 0

type Lander =
    { Pos: Point
      HorizontalVelocity: int
      VerticalVelocity: int
      CurrentDirection: Direction }
    static member Create(token: Token) =
        { Pos =
            { X = token.[0] |> int
              Y = token.[1] |> int }
          HorizontalVelocity = token.[2] |> int
          VerticalVelocity = token.[3] |> int
          CurrentDirection = token.[2] |> int |> Math.Sign |> enum<Direction> }

[<Literal>]
let maxHorizontalSpeed = 100

[<Literal>]
let desiredSpeedWhenNear = 20

[<Literal>]
let farDistance = 2000

let getLandingArea points =
    let rec flat (points': Point list) (acc: Point list) =
        match points' with
        | []
        | [ _ ] -> acc
        | p1 :: p2 :: ps ->
            if p1.Y = p2.Y then
                flat [] (p1 :: p2 :: acc)
            else
                flat (p2 :: ps) acc

    let landingRange = flat points []

    { LeftX = landingRange.[0].X
      MiddleX = (landingRange.[0].X + landingRange.[1].X) / 2
      RightX = landingRange.[1].X }

let getDistanceX (p1: int) (p2: int) = Math.Abs(p1 - p2)

let isFar (distance: int) (farThreshold: int) = distance > farThreshold

let isOverLandingArea (landingArea: FlatArea) (pos: Point) =
    pos.X >= landingArea.LeftX
    && pos.X <= landingArea.RightX

let isMovingUp lander = lander.VerticalVelocity > 0

let getDesiredDirection landingArea lander =
    Math.Sign(landingArea - lander.Pos.X)
    |> enum<Direction>

let isMovingToDesiredDirection landingArea lander =
    (getDesiredDirection landingArea lander) = lander.CurrentDirection

let getIdealAngle (desiredAcceleration: int) (thrust: int) =
        let thrust' = float thrust
        let desiredAcceleration' = float desiredAcceleration
        let accel = Math.Clamp(desiredAcceleration', -thrust', thrust'); // acceleration possible between -90 to +90 degrees
        let desiredSine =  accel / thrust'; // -1 to +1
        let angleInRadians = Math.Asin(desiredSine); // ArcSine is the inverse function of Sine
        let angleInDegrees = angleInRadians / Math.PI * 180.0;
        (int)angleInDegrees;

let getDesiredAngle desiredSpeedX thrust maxAngle landingX lander =
    let desiredVelocity =
        desiredSpeedX
        * ((getDesiredDirection landingX lander) |> int)

    let desiredAcceleration = desiredVelocity - lander.HorizontalVelocity

    let idealAngle = getIdealAngle desiredAcceleration thrust

    - Math.Clamp(idealAngle, -maxAngle, +maxAngle)

//----------------------------------------------

let readNLines n =
    List.init n (fun _ -> Console.ReadLine())

let tokenize (line: string) = line.Split ' '

let getMove desiredInitialSpeed landingArea lander =
    if isOverLandingArea landingArea lander.Pos then
        let landingSpeedIsOk = Math.Abs(lander.VerticalVelocity) < 40

        let outputThrust =
            if (landingSpeedIsOk && lander.HorizontalVelocity = 0) then
                3
            else
                4

        let outputAngle = getDesiredAngle 0 outputThrust 33 landingArea.MiddleX lander
        outputAngle, outputThrust
    else
        let outputThrust = if isMovingUp lander then 3 else 4

        let outputAngle =
            if isFar (getDistanceX landingArea.MiddleX lander.Pos.X) farDistance then
                getDesiredAngle desiredInitialSpeed outputThrust 60 landingArea.MiddleX lander
            else
                getDesiredAngle desiredSpeedWhenNear outputThrust 45 landingArea.MiddleX lander

        outputAngle, outputThrust

let surfacePoints =
    int (Console.ReadLine())
    |> readNLines
    |> List.map (tokenize >> Point.Create)

let landingArea = getLandingArea surfacePoints

let lander = Console.ReadLine() |> (tokenize >> Lander.Create)

let desiredInitialSpeed =
    if isMovingToDesiredDirection landingArea.MiddleX lander then
        Math.Abs(lander.HorizontalVelocity)
    else
        maxHorizontalSpeed

let initialAngle, initialThrust = 0, 0 //getMove desiredInitialSpeed landingArea lander
printfn "%i %i" initialAngle initialThrust

(* game loop *)
while true do
    let lander = Console.ReadLine() |> (tokenize >> Lander.Create)
    let angle, thrust = getMove desiredInitialSpeed landingArea lander (* To debug: eprintfn "Debug message" *)

    (* rotate power. rotate is the desired rotation angle. power is the desired thrust power. *)
    printfn "%i %i" angle thrust
    ()
