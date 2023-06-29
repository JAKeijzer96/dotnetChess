using System.Runtime.InteropServices;
using Core.ChessBoard;
using Core.Exceptions;
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
    
    public bool MakeMove(string from, string to, [Optional] char promotionPieceChar)
    {
        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    private bool MakeMove(Square from, Square to, [Optional] char promotionPieceChar)
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
        CheckForPawnPromotion(piece, to, promotionPieceChar);
        if (isEnPassantMove)
        {
            Board.GetSquare(to.File, from.Rank).Piece = null; // Remove captured piece
        }
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

        return to.File == EnPassant.File && to.Rank == EnPassant.Rank;
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

    private void CheckForPawnPromotion(Piece piece, Square to, char promotionPieceChar)
    {
        if (piece is not Pawn || to.Rank is not (0 or 7)) return;
        
        var whitePromotionPieces = new[] {'Q', 'R', 'B', 'N'};
        var blackPromotionPieces = new[] {'q', 'r', 'b', 'n'};

        if ((piece.IsWhite() && !whitePromotionPieces.Contains(promotionPieceChar)) ||
            (piece.IsBlack() && !blackPromotionPieces.Contains(promotionPieceChar)))
        {
            throw new InvalidPromotionException($"Invalid promotion character {promotionPieceChar} for color {Turn}");
        }
        
        to.Piece = PieceFactory.CreatePiece(promotionPieceChar);
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