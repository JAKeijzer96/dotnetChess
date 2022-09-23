using Core.ChessBoard;
using Core.Shared;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; }
    public string Castling { get; }
    public Square? EnPassant { get; }
    public int HalfMoveCount { get; }
    public int FullMoveCount { get; }

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        Castling = "KQkq";
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
    }

    public Game(Board board, Color turn, string castling, Square? enPassant, int halfMoveCount, int fullMoveCount)
    {
        Board = board;
        Turn = turn;
        Castling = castling;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
    }
}