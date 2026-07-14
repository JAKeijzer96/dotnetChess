# .NET Chess
_.NET Chess_ is a chess library written in C# using .NET 10.  
The core goal of this project is to write a library that is comprehensible and easy to read. Performance will always be secondary to readability.

## Features
- A fully working chess engine:
    - Legal move generation with full rule enformcement
    - Castling, promotion, en passant
    - Check and checkmate detection
    - Automatic draw detection (stalemate, insufficient material, fifefold repetition, seventy-five move rule
    - Claim draw (threefold repetition, fifty-move rule or by agreement)
- Branching move tree: navigate forwards and backwards through the game, make new moves and create variations
- FEN and PGN parsing and serialization, including variations
- Support for UCI notation
- Thoroughly tested with TUnit

## Getting started

```csharp
// Start a new game from the default position
var game = new Game();

// Make moves using coordinate notation
var result = game.MakeMove("e2", "e4");

// MakeMove returns a MakeMoveResult: A combination of the result of the move and
// the new game state. If the move was illegal, the game state is unchanged.
// MoveResult can be Success, IllegalMove, InvalidPromotion, ...
if (result.MoveResult == MoveResult.Success)
    game = result.Game;

// Or use UCI notation
game = game.MakeMove("e7e5").Game;

// Inspect the board
Piece? piece = game.Board["e4"].Piece; // returns a white Pawn

// Check game result
GameResult gameResult = game.GameResult; // InProgress, Checkmate, Stalemate, Draw, ...
```

### Navigation
Navigate through a game using `GoToMove`, `GoToNextMove`, `GoToPreviousMove`, and `GoToVariation`. You can create variations by stepping back and making different moves.

```csharp
// 1. e4 d5 (1... e5) 2. exd5 *
var game = new Game();
game = game.Makemove("e2", "e4").Game;
game = game.MakeMove("d7", "d5").Game;
game = game.GoToPreviousMove();        // Step back one move
game = game.Makemove("e7", "e5").Game; // Make a different move, creating a variation
game = game.GoToPreviousMove();        // Step back
game = game.GoToVariation(0);          // Switch back to mainline variation
game = game.MakeMove("e4", "d5");      // Make move on mainline variation
```

### FEN and PGN
Start a game from a non-standard position using FEN, or parse a full game from PGN. You can also serialize a game back to FEN or PGN, including variations.
Note that PGN tags are not preserved when serializing back to PGN.

```csharp
// Parse a position from FEN
Game game = FenParser.Parse("r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4");

// Serialize back to FEN
string fen = FenParser.Serialize(game);

// Parse a full game from PGN
var pgn = """
[Event "Paris"]
[Site "Paris FRA"]
[Date "1858.??.??"]
[Round "?"]
[White "Paul Morphy"]
[Black "Duke Karl / Count Isouard"]
[Result "1-0"]

1. e4 e5 2. Nf3 d6 3. d4 Bg4 4. dxe5 Bxf3 5. Qxf3 {5. gxf3
was played in Breyer vs R Binder, 1921 (1-0)} dxe5 6. Bc4 Nf6
7. Qb3 Qe7 8. Nc3 c6 9. Bg5 b5 {? 9... Na6 10. Bxa6 bxa6 11.
Rd1 Qb4 12. Ke2 a5 13. Rd3 Be7 +/- +1.54 (41 ply)} 10. Nxb5
cxb5 11. Bxb5+ Nbd7 12. O-O-O Rd8 13. Rxd7 Rxd7 14. Rd1 Qe6
15. Bxd7+ Nxd7 16. Qb8+ Nxb8 17. Rd8# 1-0
"""
Game game = PgnParser.Parse(pgn);

// Serialize to PGN
string pgn = PgnParser.Serialize(game);
```

### Pawn promotion

```csharp
// Promote using the third argument
var result = game.MakeMove("a7", "a8", 'Q');

// Or via UCI notation
var result = game.MakeMove("a7a8q");
```

### Draw claims

```csharp
// Applies threefold repetition or fifty-move rule if the position qualifies
game = game.ClaimDraw();
```

## Design

`Game` is immutable. Every `MakeMove` and navigation call returns a new instance. Nothing is mutated in place.
`File`, `Rank`, `Square`, and `Piece` are proper value objects rather than raw primitives. Each piece type has its own move validator.
The full game tree, including variations, is stored immutably, so any position can be reconstructed by replaying from the initial state.

## Requirements

- .NET 10
