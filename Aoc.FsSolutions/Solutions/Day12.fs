module Aoc.Solutions.Day12

open Aoc.Runner
open System.Collections.Generic
open System.Collections


type Command = { Command: char; Val: int64 }
type Loc = { Row: int64; Col: int64 }

type Ship = { Direction: char; Location: Loc }

let ParseInput (input: string) =
    let parseLine =
        fun (x: string) -> { Command = x.[0]; Val = int64 x.[1..] }

    input.Split("\n") |> Seq.map parseLine


let stepDirection ship dir value =
    match dir with
    | 'N' ->
        { Direction = ship.Direction
          Location =
              { Row = ship.Location.Row + value
                Col = ship.Location.Col } }
    | 'S' ->
        { Direction = ship.Direction
          Location =
              { Row = ship.Location.Row - value
                Col = ship.Location.Col } }
    | 'E' ->
        { Direction = ship.Direction
          Location =
              { Row = ship.Location.Row
                Col = ship.Location.Col + value } }
    | 'W' ->
        { Direction = ship.Direction
          Location =
              { Row = ship.Location.Row
                Col = ship.Location.Col - value } }
    | _ -> ship

let turn ship degrees =
    let currentDegrees =
        match ship.Direction with
        | 'E' -> 0L
        | 'N' -> 90L
        | 'W' -> 180L
        | 'S' -> 270L
        | _ -> 0L

    let normalized = (degrees + currentDegrees + 360L) % 360L
    match normalized with
    | 0L -> { ship with Direction = 'E' }
    | 90L -> { ship with Direction = 'N' }
    | 180L -> { ship with Direction = 'W' }
    | 270L -> { ship with Direction = 'S' }
    | _ -> ship


let step ship (command: Command) =
    match command.Command with
    | 'N'
    | 'S'
    | 'E'
    | 'W' -> stepDirection ship command.Command command.Val
    | 'F' -> stepDirection ship ship.Direction command.Val
    | 'R' -> turn ship -command.Val
    | 'L' -> turn ship command.Val
    | _ -> ship


let Simulate input =
    ParseInput input
    |> Seq.fold
        step
           { Direction = 'E'
             Location = { Row = 0L; Col = 0L } }


// ----- PART B

type State = { Waypoint: Loc; Ship: Ship }

let move location direction value =
    match direction with
    | 'E' ->
        { location with
              Col = location.Col + value }
    | 'N' ->
        { location with
              Row = location.Row + value }
    | 'W' ->
        { location with
              Col = location.Col - value }
    | 'S' ->
        { location with
              Row = location.Row - value }
    | _ -> location

let rotate state degrees =
    let newRotation = (degrees + 360L) % 360L
    match newRotation with
    | 90L ->
        { Row = state.Waypoint.Col
          Col = -state.Waypoint.Row }
    | 180L ->
        { Row = -state.Waypoint.Row
          Col = -state.Waypoint.Col }
    | 270L ->
        { Row = -state.Waypoint.Col
          Col = state.Waypoint.Row }
    | _ -> state.Waypoint


let moveToWaypoint waypoint shiploc value =
    let newWaypoint =
        { Row = waypoint.Row * value
          Col = waypoint.Col * value }

    { Row = shiploc.Row + newWaypoint.Row
      Col = shiploc.Col + newWaypoint.Col }


let stepB state (command: Command) =
    match command.Command with
    | 'N'
    | 'S'
    | 'E'
    | 'W' ->
        { state with
              Waypoint = move state.Waypoint command.Command command.Val }
    | 'F' ->
        { state with
              Ship =
                  { state.Ship with
                        Location = moveToWaypoint state.Waypoint state.Ship.Location command.Val } }
    | 'R' ->
        { state with
              Waypoint = rotate state -command.Val }
    | 'L' ->
        { state with
              Waypoint = rotate state command.Val }
    | _ -> state


type Day12() =
    inherit Day()

    override this.SolveA(input) =
        let ship = Simulate input
        string (abs (ship.Location.Row) + abs (ship.Location.Col))

    override this.SolveB(input) =
        let state =
            ParseInput input
            |> Seq.fold
                stepB
                   { Waypoint = { Col = 10L; Row = 1L }
                     Ship =
                         { Direction = 'E'
                           Location = { Row = 0L; Col = 0L } } }

        string
            (abs (state.Ship.Location.Row)
             + abs (state.Ship.Location.Col))



    override this.Tests =
        [ Test("sample 12", "sample", "286", (fun input -> (this.GetInput >> this.SolveB) input)) ]
        |> List<Test>
