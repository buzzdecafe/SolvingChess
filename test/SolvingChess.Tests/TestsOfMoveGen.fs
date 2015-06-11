﻿module TestsOfMoveGen

open Xunit
open BoardUnits
open Position
open MoveGen

[<Fact>]
let ``Detects white pawns advances``() =
    let position = { EmptyBoard with WhitePawns= (sq 'b' 2) ||| (sq 'c' 3); BlackPawns = (sq 'b' 3) }
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=(sq 'c' 3);To=(sq 'c' 4); Promotion=Undefined}, move)

[<Fact>]
let ``Advancing Pawn from d7 promotes it to queen``() =
    let position = { EmptyBoard with WhitePawns= D7 }
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=D7;To=D8; Promotion=Queen}, move)


[<Fact>]
let ``Detects white pawns captures``() =
    let position = { EmptyBoard with WhitePawns= (sq 'b' 2); BlackPawns = (sq 'b' 3)  ||| (sq 'c' 3)}
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=(sq 'b' 2);To=(sq 'c' 3); Promotion=Undefined}, move)
    
[<Fact>]
let ``Detects black pawns advances``() =
    let position = { EmptyBoard with BlackPawns= B7 ||| C6; WhitePawns = B6; SideToMove=Black }
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=C6;To=C5; Promotion=Undefined}, move)


[<Fact>]
let ``Detects black pawns captures``() =
    let position = { EmptyBoard with BlackPawns= (sq 'b' 7) ; WhitePawns = (sq 'b' 6) ||| (sq 'c' 6); SideToMove=Black }
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=(sq 'b' 7);To=(sq 'c' 6); Promotion=Undefined}, move)
 
[<Fact>]
let ``Detects white pawn advance and promotion``() =
    let position = { EmptyBoard with WhiteKing=F7; WhitePawns=F5 ||| G7; BlackKing=H7; BlackPawns=F6 ||| H6 }
    let move = moves position |> Seq.head
    Assert.Equal({Piece=Pawn; From=G7; To=G8; Promotion=Queen}, move)