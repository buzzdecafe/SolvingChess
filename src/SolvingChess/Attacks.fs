﻿module Attacks

open BoardUnits
open Position
open BoardShifter
open BitOperations

let private ka =    Array.zeroCreate<Bitboard> 64
let private karea = Array.zeroCreate<Bitboard> 64
let private wpa =   Array.zeroCreate<Bitboard> 64

for i = 0 to 63 do
    let s = sqFromIndex i
    let cs = chessShift

    ka.[i] <- cs -1 -1 s ||| cs -1  0 s ||| cs -1 1 s ||| cs  0 -1 s ||| cs  0 1 s |||
                 cs  1 -1 s||| cs  1 0 s ||| cs  1 1 s 

    let a = cs  -1 0 (ka.[i]) ||| cs  1 0 (ka.[i])
    karea.[i] <- cs  0 -1 a ||| cs 0 1 a
    
    wpa.[i] <- (cs  1 -1 s ||| cs 1 1 s) 


let kingAttacks (kingPosition : Bitboard) = 
    ka.[lsb kingPosition]

let kingArea (kingPosition: Bitboard) =
    karea.[lsb kingPosition]

let whitePawnsAttacks (whitePawnsPositions : Bitboard) =
    let cs = whitePawnsPositions.chessShift
    (cs 1 -1 ||| cs 1 1) 

let blackPawnsAttacks (blackPawnsPositions : Bitboard) =
    let cs = blackPawnsPositions.chessShift
    cs -1 -1 ||| cs -1 1

let knightsAttacks (knightsPositions : Bitboard) =
    let cs = knightsPositions.chessShift
    cs 1 2 ||| cs  1 -2 ||| cs  2 1 ||| cs  2 -1 ||| cs -1 2 |||
               cs -1 -2 ||| cs -2 1 ||| cs -2 -1

let rooksAttacks rooksPositions friends enemies =
    let allpieces = friends ||| enemies
    enumerateSquares rooksPositions
    |> Seq.map(fun(sq) -> 
                    rankOfSquare sq ||| fileOfSquare sq 
                    |> except sq
                    |> except (rayToNFromSquare (((rayToNFromSquare sq) &&& allpieces).lsb() |> sqFromIndex))
                    |> except (rayToEFromSquare (((rayToEFromSquare sq) &&& allpieces).lsb() |> sqFromIndex))
                    |> except (rayToSFromSquare (((rayToSFromSquare sq) &&& allpieces).msb() |> sqFromIndex))
                    |> except (rayToWFromSquare (((rayToWFromSquare sq) &&& allpieces).msb() |> sqFromIndex))
              )
    |> Seq.fold (fun acc elem -> acc ||| elem) 0UL

let bishopsAttacks bishopsPositions friends enemies =
    let allpieces = friends ||| enemies
    enumerateSquares bishopsPositions
    |> Seq.map(fun(sq) -> 
                    diagonalNWOfSquare sq ||| diagonalNEOfSquare sq 
                    |> except sq
                    |> except (rayToNEFromSquare (((rayToNEFromSquare sq) &&& allpieces).lsb() |> sqFromIndex))
                    |> except (rayToNWFromSquare (((rayToNWFromSquare sq) &&& allpieces).lsb() |> sqFromIndex))
                    |> except (rayToSEFromSquare (((rayToSEFromSquare sq) &&& allpieces).msb() |> sqFromIndex))
                    |> except (rayToSWFromSquare (((rayToSWFromSquare sq) &&& allpieces).msb() |> sqFromIndex))
              )
    |> Seq.fold (fun acc elem -> acc ||| elem) 0UL

let queensAttacks queensPositions friends enemies = 
    (rooksAttacks queensPositions friends enemies) ||| (bishopsAttacks queensPositions friends enemies)


let whiteAttacks (position:Position) ignoresBlackKing =
    let enemies = if not ignoresBlackKing 
                  then position.BlackPieces 
                  else position.BlackPieces &&& ~~~(position.BlackKing)

    queensAttacks       position.WhiteQueens  position.WhitePieces enemies |||
    rooksAttacks        position.WhiteRooks   position.WhitePieces enemies |||
    bishopsAttacks      position.WhiteBishops position.WhitePieces enemies |||
    knightsAttacks      position.WhiteKnights  |||
    whitePawnsAttacks   position.WhitePawns |||
    kingAttacks         position.WhiteKing        

let blackAttacks (position:Position) ignoresWhiteKing =
    let enemies = if not ignoresWhiteKing
                  then position.WhitePieces 
                  else position.WhitePieces &&& ~~~(position.WhiteKing)

    queensAttacks       position.BlackQueens  position.BlackPieces enemies |||
    rooksAttacks        position.BlackRooks   position.BlackPieces enemies |||
    bishopsAttacks      position.BlackBishops position.BlackPieces enemies |||
    knightsAttacks      position.BlackKnights  |||
    blackPawnsAttacks   position.BlackPawns |||
    kingAttacks         position.BlackKing