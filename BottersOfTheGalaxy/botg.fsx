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

type SkillType =
    | DeadPoolCounter
    | DeadPoolWire of Point
    | DeadPoolStealth of Point
    | HulkCharge of Unit
    | HulkExplosiveShield
    | HulkBash of Unit
    | ValkyrieSpearFlip of Unit
    | ValkyrieJump of Point
    | ValkyriePowerup
    | IronmanBlink of Point
    | IronmanFireball of Point
    | IronmanBurning of Point
    | DrStrangeAoeHeal of Point
    | DrStrangeShield of Unit
    | DrStrangePull of Unit

type Move = 
    | Move of Point 
    | MoveAttack of Point * int
    | Attack of int
    | AttackNearest of UnitType
    | Buy of Item
    | Wait 
    | PickHero of HeroType
    | Skill of SkillType
    override x.ToString() =
        match x with
        | Move p -> sprintf "MOVE %.0f %.0f" p.X p.Y
        | MoveAttack (p, id) -> sprintf "MOVE_ATTACK %.0f %.0f %i" p.X p.Y id
        | Attack id -> sprintf "ATTACK %i" id
        | AttackNearest ut -> sprintf "ATTACK_NEAREST %s" (ut.ToString())
        | Buy i -> sprintf "BUY %s" i.Name
        | Skill (IronmanBlink p) -> sprintf "BLINK %.0f %.0f" p.X p.Y
        | Skill (IronmanFireball p) -> sprintf "FIREBALL %.0f %.0f" p.X p.Y
        | Skill (IronmanBurning p) -> sprintf "BURNING %.0f %.0f" p.X p.Y
        | Skill (ValkyrieSpearFlip u) -> sprintf "SPEARFLIP %i" u.UnitId
        | Skill (ValkyrieJump p) -> sprintf "JUMP %.0f %.0f" p.X p.Y
        | Skill ValkyriePowerup -> sprintf "POWERUP"
        | Skill DeadPoolCounter -> sprintf "COUNTER"
        | Skill (DeadPoolWire p) -> sprintf "WIRE %.0f %.0f" p.X p.Y
        | Skill (DeadPoolStealth p) -> sprintf "STEALTH %.0f %.0f" p.X p.Y
        | Skill (HulkCharge u) -> sprintf "CHARGE %i" u.UnitId
        | Skill HulkExplosiveShield -> sprintf "EXPLOSIVESHIELD"
        | Skill (HulkBash u) -> sprintf "BASH %i" u.UnitId
        | Skill (DrStrangeAoeHeal p) -> sprintf "AOEHEAL %.0f %.0f" p.X p.Y
        | Skill (DrStrangeShield u) -> sprintf "SHIELD %i" u.UnitId
        | Skill (DrStrangePull u) -> sprintf "PULL %i" u.UnitId
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
    let myHeroes = units |> List.filter (fun u -> u.Team = myTeam && u.UnitType = Hero)
    let enemyHeroes = units |> List.filter (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Hero)

    let retreat () =
        myMessages.[roundNum%2] <- "Retreat!"
        Move.Move myTower.Point 

    let getNonSkillMove (hero : Unit) =
        let isSafeDist = isSafeDistance enemyTower
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
            myMessages.[roundNum%2] <- sprintf "Deny: %i" lu.UnitId
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
                    myMessages.[roundNum%2] <- sprintf "Attack: %i" le.UnitId
                    Move.MoveAttack (potentialMove, le.UnitId)
                | _, Some ce -> 
                    myMessages.[roundNum%2] <- sprintf "Attack: %i" ce.UnitId
                    Move.MoveAttack (potentialMove, ce.UnitId)
                | _ -> 
                    myMessages.[roundNum%2] <- "Form a line"
                    Move.Move potentialMove
            else 
                retreat ()

    let getMove (hero : Unit) =
        let isSafeDist = isSafeDistance enemyTower
        match enemyUnits.Length, enemyHeroes.Length, hero.HeroType with
        | eu, _, ht when eu > 0 && ht = HeroType.Ironman ->
            let burnRange = enemyUnits |> List.filter (fun u -> getDist hero.Point u.Point <= 250.)
            let fireballRange = enemyUnits |> List.filter (fun u -> getDist hero.Point u.Point <= 900.)
            match hero.Mana, hero.CoolDown1, hero.CoolDown2, fireballRange.Length, hero.CoolDown3, burnRange.Length with
            | m, _, _, _, cd3, br when m >= 50 && cd3 = 0 && br >= 2 ->
                let target = burnRange.Head.Point
                myMessages.[roundNum%2] <- sprintf "Burn: %.0f %.0f" target.X target.Y
                Move.Skill (SkillType.IronmanBurning target)
            | m, _, cd2, fr, _, _ when m >= 60 && cd2 = 0 && fr >= 2 ->
                let target = fireballRange.Head.Point
                myMessages.[roundNum%2] <- sprintf "FB: %.0f %.0f" target.X target.Y
                Move.Skill (SkillType.IronmanFireball fireballRange.Head.Point)
            // | m, cd1, _, _, _, _ when m >= 16 && m <= 50 && cd1 = 0 ->
            //     let target = { hero.Point with X = backup myTeam hero.Point.X 200. }
            //     myMessages.[roundNum%2] <- sprintf "Blink: %.0f %.0f" target.X target.Y
            //     Move.Skill (SkillType.IronmanBlink target)
            | _ ->
                if myUnits.Length > 0 then
                    getNonSkillMove hero
                else 
                   retreat ()

        | _, _, ht when ht = HeroType.Valkyrie ->
            if myUnits.Length > 0 then
                getNonSkillMove hero
            else
               retreat () 
        | _ -> 
            retreat ()

    let moves =
        match roundType with
        | CommandHeroes _ -> 
            myHeroes 
            |> List.map (fun hero ->
                roundNum <- roundNum + 1    
                let affordableItems = items |> List.filter (fun i -> i.Cost < gold && not i.IsPotion)
                let healthPotions = items |> List.filter (fun i -> i.Cost <= gold && i.IsPotion && i.Health > 0.)
                let manaPotions = items |> List.filter(fun i -> i.Cost <= gold && i.IsPotion && i.Mana > 0)
                let isSafeDist = isSafeDistance enemyTower
                match hero.Health, healthPotions.Length, hero.Mana, manaPotions.Length, affordableItems.Length with
                | h, hp, _, _, _ when h < hero.MaxHealth * 0.3 && hp > 0 ->
                    myMessages.[roundNum%2] <- sprintf "Health Potion"
                    healthPotions |> buyPotion PotionType.Health

                | _, _, m, mp, _ when m < hero.MaxMana / 4 && mp > 0 ->
                    myMessages.[roundNum%2] <- sprintf "Mana Potion"
                    manaPotions |> buyPotion PotionType.Mana

                | _, _, _, _, ai when ai > 0 && hero.ItemsOwned < 3 ->
                    myMessages.[roundNum%2] <- sprintf "New Shiny"
                    affordableItems |> buyItem hero

                | _ ->
                    getMove hero)

        | SelectHero -> 
            if myHeroTypes.Count = 0 then 
                myHeroTypes.Add(HeroType.Ironman)
                [ Move.PickHero HeroType.Ironman ]
            else
                myHeroTypes.Add(HeroType.Valkyrie)
                [ Move.PickHero HeroType.Valkyrie ]        

    match roundType with
    | CommandHeroes _ ->
        moves 
        |> List.iteri (fun i m -> printfn "%s;%s" (string m) myMessages.[i])
    | SelectHero ->
        moves 
        |> List.iter (string >> printfn "%s")    
    ()