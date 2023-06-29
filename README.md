# dotnetChess
dotnetChess is a chess game/engine written in C# using .NET Core 6.  
The core goal of this project is to write an engine that is comprehensible and easy to read. Computation speed is not or very rarely taken into account when writing code.  

## To do list / Backlog
- [x] Setup project (June 2022)
- [ ] Implement chess rules in backend
  - [x] Parse and generate basic FEN (September 2022)
  - [x] Basic piece movement (October 2022)
  - [ ] More advanced movement
    - [x] En passant (June 2023)
    - [x] Pawn promotion (June 2023)
    - [ ] Castling
    - [ ] Check
    - [ ] Checkmate
  - [ ] Parse and generate full FEN
  - [ ] Rules
    - [ ] Check
    - [ ] Checkmate
    - [ ] Draw
      - [ ] Stalemate
      - [ ] Repetition
        - [ ] Threefold repetition
          - [ ] Fivefold repetition (forced draw)
      - [ ] 50 move rule
        - [ ] 75 move rule (forced draw)
      - [ ] Insufficient material
      - [ ] Agreement
  - [ ] Parse and generate PGN
- [ ] Engine
  - [ ] Generate possible moves
  - [ ] Analyse position
  - [ ] Predict 
- [ ] Celebrate

### Undecided as of yet
- [ ] API
- [ ] Frontend
- [ ] Authorization
- [ ] Database
- [ ] Time control
- [ ] ...
