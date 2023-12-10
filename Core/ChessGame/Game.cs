using System.Runtime.InteropServices;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using File = Core.ChessBoard.File;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; private set; }
    public CastlingAvailability Castling { get; }
    public Square? EnPassant { get; private set; }
    public int HalfMoveCount { get; private set; }
    public int FullMoveCount { get; private set; }

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        Castling = new CastlingAvailability("KQkq");
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
    }

    public Game(Board board, Color turn, string castling, Square? enPassant, int halfMoveCount, int fullMoveCount)
    {
        Board = board;
        Turn = turn;
        Castling = new CastlingAvailability(castling);
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
        var isCastlingMove = IsCastlingMove(from, to);
        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove || isCastlingMove))
        {
            return false;
        }
        
        // Move is valid. Move pieces and update gamestate
        var pieceCaptured = to.Piece is not null || isEnPassantMove;
        UpdateEnPassantSquare(from, to);
        MovePieces(from, to, isEnPassantMove, isCastlingMove, promotionPieceChar);
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

    private bool IsCastlingMove(Square from, Square to)
    {
        if (from.Piece is not King)
        {
            return false;
        }

        var king = from.Piece;
        var isWhiteKingMoveOnFirstRank = king.IsWhite() && from.Rank == Rank.First && to.Rank == Rank.First;
        var isBlackKingMoveOnEighthRank = king.IsBlack() && from.Rank == Rank.Eighth && to.Rank == Rank.Eighth;
        var isFileDifferenceGreaterThanTwo = from.File.DistanceTo(to.File) >= 2;

        if (!(isFileDifferenceGreaterThanTwo && (isWhiteKingMoveOnFirstRank || isBlackKingMoveOnEighthRank)))
        {
            return false;
        }
        
        var direction = from.File < to.File ? Direction.Right : Direction.Left;
        VerifyKingCanCastleInGivenDirection(from, to, king, direction);
        
        var currentFile = from.File;
        var destinationFile = from.File + 2 * direction;
        while (currentFile != destinationFile)
        {
            currentFile += direction;
            if (Board[currentFile, from.Rank].IsOccupied())
            {
                throw new InvalidCastlingException(from, to, blockedFile: currentFile);
            }
        }

        // When castling queenside, need to check the b-file for obstructions because the rook needs to move through it
        if (currentFile < from.File && Board[File.B, from.Rank].IsOccupied())
        {
            throw new InvalidCastlingException(from, to, blockedFile: File.B);
        }

        return true;
    }

    private void VerifyKingCanCastleInGivenDirection(Square from, Square to, Piece king, int direction)
    {
        if (Castling.CanNeitherSideCastle() ||
            (king.IsWhite() && direction == Direction.Right && !Castling.CanWhiteCastleKingside()) ||
            (king.IsWhite() && direction == Direction.Left && !Castling.CanWhiteCastleQueenside()) ||
            (king.IsBlack() && direction == Direction.Right && !Castling.CanBlackCastleKingside()) ||
            (king.IsBlack() && direction == Direction.Left && !Castling.CanBlackCastleQueenside()))
        {
            throw new InvalidCastlingException($"Cannot castle from {from} to {to} because the " +
                                               $"king and/or rook have moved (Castling string: {Castling}).");
        }
    }

    private void UpdateEnPassantSquare(Square from, Square to)
    {
        var piece = from.Piece;

        // We already know the move is valid, so to check if it's a pawns first move we only
        // need to check the difference in rank between the start and end square, and can omit
        // checking if the start rank is the second rank (for white) or seventh rank (for black)
        if (piece is Pawn && from.Rank.DistanceTo(to.Rank) == 2)
        {
            var direction = piece.IsWhite() ? Direction.Up : Direction.Down;
            EnPassant = Board[from.File, from.Rank + direction];
        }
        else
        {
            EnPassant = null;
        }
    }

    private void MovePieces(Square from, Square to, bool isEnPassantMove, bool isCastlingMove, [Optional] char promotionPieceChar)
    {
        var piece = from.Piece!;
        var isPromotionMove = IsPromotionMove(piece, to, promotionPieceChar);

        if (isCastlingMove)
        {
            PerformCastlingMove(from, to);
        }
        else if (isPromotionMove)
        {
            PromotePawn(from, to, promotionPieceChar);
        } 
        else
        {
            Board.MovePiece(from, to);
            if (isEnPassantMove)
            {
                RemoveCapturedEnPassantPawn(from, to);
            }
            Castling.UpdateAfterRegularMove(piece, from);
        }
    }
    
    private bool IsPromotionMove(Piece piece, Square to, char promotionPieceChar)
    {
        if (piece is not Pawn || !(to.Rank == Rank.First || to.Rank == Rank.Eighth))
        {
            return false;
        }
        
        var whitePromotionPieces = new[] {'Q', 'R', 'B', 'N'};
        var blackPromotionPieces = new[] {'q', 'r', 'b', 'n'};

        if ((piece.IsWhite() && !whitePromotionPieces.Contains(promotionPieceChar)) ||
            (piece.IsBlack() && !blackPromotionPieces.Contains(promotionPieceChar)))
        {
            throw new InvalidPromotionException($"Invalid promotion character {promotionPieceChar} for color {Turn}");
        }

        return true;
    }

    private void PerformCastlingMove(Square from, Square to)
    {
        var direction = from.File < to.File ? Direction.Right : Direction.Left;
        var rookFile = from.File < to.File ? File.H : File.A;
            
        // Move king and rook
        var kingDestinationSquare = Board[from.File + 2 * direction, from.Rank];
        Board.MovePiece(from: from, to: kingDestinationSquare);
        var rookSquare = Board[rookFile, from.Rank]; 
        var rookDestinationSquare = Board[from.File + direction, from.Rank];
        Board.MovePiece(from: rookSquare, to: rookDestinationSquare);
        
        // Update Castling property
        var isWhite = kingDestinationSquare.Piece!.IsWhite();
        Castling.UpdateAfterCastlingMove(isWhite);
    }

    private static void PromotePawn(Square from, Square to, char promotionPieceChar)
    {
        from.Piece = null;
        to.Piece = PieceFactory.CreatePiece(promotionPieceChar);
    }

    private void RemoveCapturedEnPassantPawn(Square from, Square to)
    {
        Board[to.File, from.Rank].Piece = null;
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