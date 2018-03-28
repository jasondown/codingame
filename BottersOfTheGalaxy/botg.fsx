open System

type RoundType = SelectHero | CommandHeroes of int

type EntityType = Bush | Spawn

type Point = { X : float; Y : float }

type Entity = 
    { EntityType : EntityType
      Point      : Point
      Radius     : int }

type Item =
    { Name      : string
      Cost      : int
      Damage    : float
      Health    : float
      MaxHealth : float
      Mana      : int
      MaxMana   : int
      MoveSpeed : int
      ManaRegen : int
      IsPotion  : bool }

type PotionType =
    | Health
    | Mana

type UnitType = 
    Unit | Hero | Tower | Groot
    override x.ToString() =
        match x with
        | Unit -> "UNIT"
        | Hero -> "HERO"
        | Tower -> "TOWER"
        | Groot -> "GROOT"

type HeroType = 
    Deadpool | DoctorStrange | Hulk | Ironman | Valkyrie | NotHero
    override x.ToString() =
        match x with
        | Deadpool -> "DEADPOOL"
        | DoctorStrange -> "DOCTOR_STRANGE"
        | Hulk -> "HULK"
        | Ironman -> "IRONMAN"
        | Valkyrie -> "VALKYRIE"
        | NotHero -> ""

type Unit =
    { UnitId : int
      Team          : int
      UnitType      : UnitType
      Point         : Point
      AttackRange   : float
      Health        : float
      MaxHealth     : float
      Shield        : int
      AttackDamage  : float
      MovementSpeed : float
      StunDuration  : int
      GoldValue     : int
      CoolDown1     : int
      CoolDown2     : int
      CoolDown3     : int
      Mana          : int
      MaxMana       : int
      ManaRegen     : int
      HeroType      : HeroType
      IsVisible     : bool
      ItemsOwned    : int }

type Move = 
    | Move of Point 
    | MoveAttack of Point * int
    | Attack of int
    | AttackNearest of UnitType
    | Buy of Item
    | Wait 
    | PickHero of HeroType
    override x.ToString() =
        match x with
        | Move p -> sprintf "MOVE %f %f" p.X p.Y
        | MoveAttack (p, id) -> sprintf "MOVE_ATTACK %f %f %i" p.X p.Y id
        | Attack id -> sprintf "ATTACK %i" id
        | AttackNearest ut -> sprintf "ATTACK_NEAREST %s" (ut.ToString())
        | Buy i -> sprintf "BUY %s" i.Name
        | Wait -> "WAIT"
        | PickHero h -> h |> string

let myItemsHero1 = new System.Collections.Generic.List<Item>(4)
let myItemsHero2 = new System.Collections.Generic.List<Item>(4)
let myHeroTypes = new System.Collections.Generic.List<HeroType>(2)
let myMessages = new System.Collections.Generic.List<string>(["";""])
let mutable gold = 0
let mutable roundNum = 1
//----------Helper Functions
let readNLines n = List.init n (fun _ -> Console.ReadLine())

let getUnitType s =
    match s with
    | "UNIT" -> Unit
    | "HERO" -> Hero
    | "TOWER" -> Tower
    | "GROOT" -> Groot
    | s -> failwithf "Unknown unit type %s" s

let getHeroType s =
        match s with
        | "DEADPOOL" -> Deadpool
        | "DOCTOR_STRANGE" -> DoctorStrange
        | "HULK" -> Hulk
        | "IRONMAN" -> Ironman
        | "VALKYRIE" -> Valkyrie
        | _ -> NotHero

let getDist p1 p2 =
    let x = (p1.X - p2.X) * (p1.X - p2.X)
    let y = (p1.Y - p2.Y) * (p1.Y - p2.Y)
    let dist = (x + y) |> sqrt
    dist

let isSafeDistance (enemyTower : Unit) (hero : Unit) (target : Point) =
    let distFromTower = getDist target enemyTower.Point
    let rangeAndSpeed =  enemyTower.AttackRange + hero.MovementSpeed
    distFromTower > rangeAndSpeed

let backup myTeam (x:float) (x1:float) =
    if myTeam = 0 then x-x1 else x+x1

let inFrontOf myTeam x1 x2 dist =
    if myTeam = 0 
    then x1 > x2-dist
    else x1 < x2+dist

let buyItem hero (items : Item list) =
    let item = items |> List.maxBy (fun i -> i.Damage * 7. + i.Health * 4. + float i.Mana * 2.)    
    if hero.HeroType = myHeroTypes.[0]
    then myItemsHero1.Add(item)
    else myItemsHero2.Add(item)
    gold <- gold - item.Cost
    Move.Buy item

let buyPotion potionType (potions : Item list) =
    let potion =
        match potionType with
        | PotionType.Health -> 
            potions |> List.maxBy (fun p -> p.Health)
        | PotionType.Mana ->
            potions |> List.maxBy (fun p -> p.Mana)
    gold <- gold - potion.Cost
    Move.Buy potion

//----------

let myTeam = Console.ReadLine() |> int
let entities =
    Console.ReadLine() |> int |> readNLines
    |> List.map (fun l -> 
        let token = l.Split ' '
        let entityType = 
            match token.[0] with
            | "BUSH" -> Bush
            | "SPAWN" -> Spawn
            | x -> failwithf "Unknown span type %s" x
        { EntityType = entityType
          Point      = { X = float token.[1]; Y = float token.[2] }
          Radius     = int token.[3] })
    
let items =
    Console.ReadLine() |> int |> readNLines
    |> List.map (fun l ->
        let token = l.Split ' '
        { Name      = token.[0]
          Cost      = int token.[1]
          Damage    = float token.[2]
          Health    = float token.[3]
          MaxHealth = float token.[4]
          Mana      = int token.[5]
          MaxMana   = int token.[6]
          MoveSpeed = int token.[7]
          ManaRegen = int token.[8]
          IsPotion  = (int token.[9]) = 1 })

(* game loop *)
while true do
    gold <- Console.ReadLine() |> int
    let enemyGold = Console.ReadLine() |> int
    let roundType = 
        match Console.ReadLine() |> int with
        | n when n < 0 -> SelectHero
        | n            -> CommandHeroes n

    let units =
        Console.ReadLine() |> int |> readNLines
        |> List.map (fun l -> 
            let token = l.Split ' '
            { UnitId        = int token.[0]
              Team          = int token.[1]
              UnitType      = getUnitType token.[2]
              Point         = { X = float token.[3]; Y = float token.[4] }
              AttackRange   = float token.[5]
              Health        = float token.[6]
              MaxHealth     = float token.[7]
              Shield        = int token.[8]
              AttackDamage  = float token.[9]
              MovementSpeed = float token.[10]
              StunDuration  = int token.[11]
              GoldValue     = int token.[12]
              CoolDown1     = int token.[13]
              CoolDown2     = int token.[14]
              CoolDown3     = int token.[15]
              Mana          = int token.[16]
              MaxMana       = int token.[17]
              ManaRegen     = int token.[18]
              HeroType      = getHeroType token.[19]
              IsVisible     = int token.[20] <> 0
              ItemsOwned    = int token.[21] })        

    let myTower = units |> List.find (fun u -> u.Team = myTeam && u.UnitType = UnitType.Tower)
    let enemyTower = units |> List.find (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Tower)
    let myUnits = units |> List.filter (fun u -> u.Team = myTeam && u.UnitType = UnitType.Unit)
    let enemyUnits = units |> List.filter (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Unit)
    
    let getMove (hero : Unit) =
        roundNum <- roundNum + 1    
        let affordableItems = items |> List.filter (fun i -> i.Cost < gold && not i.IsPotion)
        let healthPotions = items |> List.filter (fun i -> i.Cost <= gold && i.IsPotion && i.Health > 0.)
        let manaPotions = items |> List.filter(fun i -> i.Cost <= gold && i.IsPotion && i.Mana > 0)
        let isSafeDist = isSafeDistance enemyTower
        match hero.Health, healthPotions.Length, hero.Mana, manaPotions.Length, myUnits.Length, enemyUnits.Length, affordableItems.Length with

        | h, hp, _, _, _, _, _ when h < hero.MaxHealth * 0.3 && hp > 0 ->
            myMessages.[roundNum%2] <- sprintf "Health Potion"
            healthPotions |> buyPotion PotionType.Health

        | _, _, m, mp, _, _, _ when m < hero.MaxMana / 4 && mp > 0 ->
            myMessages.[roundNum%2] <- sprintf "Mana Potion"
            manaPotions |> buyPotion PotionType.Mana

        | _, _, _, _, _, _, ai when ai > 0 && hero.ItemsOwned < 4 ->
            myMessages.[roundNum%2] <- sprintf "New Shiny!"
            buyItem hero affordableItems

        | _, _, _, _, mu, _, _ when mu > 0 ->
            let heroRange = hero.AttackRange + hero.MovementSpeed * 0.3
            let myFrontUnit = 
                let safeUnits = myUnits |> List.filter (fun u -> isSafeDist hero u.Point)
                if safeUnits.Length > 0 then
                    safeUnits |> List.maxBy (fun u -> if hero.Team = 0 then u.Point.X else -u.Point.X)
                else 
                    myUnits |> List.maxBy (fun u -> if hero.Team = 0 then u.Point.X else -u.Point.X)              
            let myLowUnit = myUnits |> List.tryFind (fun u -> u <> myFrontUnit && u.Health < u.MaxHealth * 0.4 && u.Health <= hero.AttackDamage && getDist hero.Point u.Point < heroRange)
            match myLowUnit with
            | Some lu -> 
                myMessages.[roundNum%2] <- sprintf "Denied: %i!" lu.UnitId
                if getDist hero.Point lu.Point < hero.AttackRange then
                    Move.Attack lu.UnitId
                else 
                    Move.MoveAttack (lu.Point, lu.UnitId)
            | None ->
                let potentialMove = { X = backup hero.Team myFrontUnit.Point.X 25.; Y = hero.Point.Y }
                let lowestEnemy = enemyUnits |> List.tryFind (fun eu -> eu.Health < hero.AttackDamage && getDist potentialMove eu.Point < heroRange)
                let closestEnemy = enemyUnits |> List.tryFind (fun eu -> getDist potentialMove eu.Point < heroRange)
                if isSafeDist hero potentialMove then
                    match lowestEnemy, closestEnemy with
                    | Some le, _ -> 
                        myMessages.[roundNum%2] <- sprintf "Attack: %i!" le.UnitId
                        Move.MoveAttack (potentialMove, le.UnitId)
                    | _, Some ce -> 
                        myMessages.[roundNum%2] <- sprintf "Attack: %i!" ce.UnitId
                        Move.MoveAttack (potentialMove, ce.UnitId)
                    | _ -> 
                        myMessages.[roundNum%2] <- "Form a line!"
                        Move.Move potentialMove
                else 
                    myMessages.[roundNum%2] <- "Retreat!"
                    Move.Move myTower.Point
        | _ -> 
            myMessages.[roundNum%2] <- "Retreat!"
            Move.Move myTower.Point

    let moves =
        match roundType with
        | CommandHeroes _ -> 
            let myHeroes = units |> List.filter (fun u -> u.Team = myTeam && u.UnitType = Hero)
            myHeroes 
            |> List.map getMove
        | SelectHero -> 
            if myHeroTypes.Count = 0 then 
                myHeroTypes.Add(HeroType.Ironman)
                [ Move.PickHero HeroType.Ironman ]
            else
                myHeroTypes.Add(HeroType.Valkyrie)
                [ Move.PickHero HeroType.Valkyrie ]        

    match roundType with
    | CommandHeroes _ ->
        moves |> List.iteri (fun i m -> printfn "%s;%s" (string m) myMessages.[i])
    | SelectHero ->
        moves |> List.iter (string >> printfn "%s")    
    ()