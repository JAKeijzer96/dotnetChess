using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; private set; }
    public string Castling { get; }
    public Square? EnPassant { get; private set; }
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
        
        if (piece is null || piece.Color != Turn)
        {
            return false;
        }

        var isEnPassantMove = IsEnPassantMove(from, to);
        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove))
        {
            return false;
        }
        
        // Move is valid. Move pieces and update gamestate
        var pieceCaptured = to.Piece is not null || isEnPassantMove;
        UpdateEnPassantSquare(from, to);
        Board.MovePiece(from, to);
        UpdateHalfMoveCount(piece, pieceCaptured);
        EndTurn();
        
        return true;
    }
    
    private bool IsEnPassantMove(Square from, Square to)
    {
        if (EnPassant is null)
        {
            return false;
        }

        var piece = from.Piece!;

        if (piece is not Pawn)
        {
            return false;
        }

        var direction = piece.IsWhite() ? 1 : -1;

        return to.File == EnPassant.File && from.Rank + direction == EnPassant.Rank;
    }

    private void UpdateEnPassantSquare(Square from, Square to)
    {
        var piece = from.Piece;

        // No need to explicitly check for exact rank as we already know it's a valid move,
        // thus meaning that it has to be a first move from the pawns starting rank
        if (piece is Pawn && Math.Abs(from.Rank - to.Rank) == 2)
        {
            var direction = piece.IsWhite() ? 1 : -1;
            EnPassant = Board.GetSquare(from.File, from.Rank + direction);
        }
        else
        {
            EnPassant = null;
        }
    }

    private void UpdateHalfMoveCount(Piece movedPiece, bool pieceCaptured)
    {
        if (movedPiece is Pawn || pieceCaptured)
        {
            HalfMoveCount = 0;
        }
        else
        {
            HalfMoveCount++;
        }
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
}