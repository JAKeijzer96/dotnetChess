using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; private set; }
    public string Castling { get; }
    public Square? EnPassant { get; }
    public int HalfMoveCount { get; private set; }
    public int FullMoveCount { get; private set; }

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
    
    public bool MakeMove(string from, string to)
    {
        return MakeMove(Board.GetSquare(from), Board.GetSquare(to));
    }

    private bool MakeMove(Square from, Square to)
    {
        var piece = from.Piece;
        var pieceToCapture = to.Piece;
        
        if (piece is null || piece.Color != Turn)
        {
            return false;
        }

        if (!piece.IsValidMove(Board, from, to))
        {
            return false;
        }
        
        // Move is valid. Move pieces and update gamestate
        Board.MovePiece(from, to);
        UpdateHalfMoveCount(piece, pieceToCapture);
        EndTurn();
        
        return true;
    }

    private void EndTurn()
    {
        if (Turn == Color.Black)
        {
            Turn = Color.White;
            FullMoveCount++;
        }
        else
        {
            Turn = Color.Black;
        }
    }
    
    private void UpdateHalfMoveCount(Piece movedPiece, Piece? capturedPiece)
    {
        if (movedPiece is Pawn || capturedPiece is not null)
        {
            HalfMoveCount = 0;
        }
        else
        {
            HalfMoveCount++;
        }
    }
}