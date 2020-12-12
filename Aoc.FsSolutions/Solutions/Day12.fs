module Aoc.Solutions.Day12

open Aoc.Runner
open System.Collections.Generic
open System.Collections


type Command = { Command: char; Val: int }
type Loc = { Row: int; Col: int }

type Ship = { Direction: char; Location: Loc }

let ParseInput (input: string) =
    let parseLine =
        fun (x: string) -> { Command = x.[0]; Val = int x.[1..] }

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
        | 'E' -> 0
        | 'N' -> 90
        | 'W' -> 180
        | 'S' -> 270
        | _ -> 0
    let normalized = (degrees + currentDegrees + 360) % 360
    match normalized with
    | 0 -> { ship with Direction = 'E' }
    | 90 -> { ship with Direction = 'N' }
    | 180 -> { ship with Direction = 'W' }
    | 270 -> { ship with Direction = 'S' }
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
             Location = { Row = 0; Col = 0 } }



type Day12() =
    inherit Day()

    override this.SolveA(input) =
        let ship = Simulate input
        string (abs(ship.Location.Row) + abs(ship.Location.Col))

    override this.SolveB(input) = "B"

    override this.Tests =
        [ Test("fsharp test", "123", "123", (fun input -> "123")) ]
        |> List<Test>
