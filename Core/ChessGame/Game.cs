using System.Runtime.InteropServices;
using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;
using File = Core.ChessBoard.File;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; private set; }
    public CastlingAvailability CastlingAvailability { get; }
    public Square? EnPassant { get; private set; }
    public int HalfMoveCount { get; private set; }
    public int FullMoveCount { get; private set; }
    public GameResult Result { get; private set; }

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        CastlingAvailability = new CastlingAvailability("KQkq");
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
    }

    public Game(Board board, Color turn, CastlingAvailability castlingAvailability, Square? enPassant, int halfMoveCount, int fullMoveCount)
    {
        Board = board;
        Turn = turn;
        CastlingAvailability = castlingAvailability;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
        Result = EvaluateResult();
    }
    
    public MoveResult MakeMove(string from, string to, [Optional] char promotionPieceChar)
    {
        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    private MoveResult MakeMove(Square from, Square to, [Optional] char promotionPieceChar)
    {
        if (Result != GameResult.InProgress)
        {
            return MoveResult.GameAlreadyOver;
        }

        var piece = from.Piece;
        if (piece is null || piece.Color != Turn)
        {
            return MoveResult.InvalidPiece;
        }

        var isEnPassantMove = IsEnPassantMove(from, to);
        var isCastlingMove = IsCastlingMove(from, to);
        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove || isCastlingMove))
        {
            return MoveResult.IllegalMove;
        }

        if (WouldLeaveKingInCheck(from, to, isEnPassantMove, isCastlingMove, piece.Color))
        {
            return MoveResult.IllegalMove;
        }

        if (IsPromotionMove(piece, to) && !IsValidPromotionChar(piece, promotionPieceChar))
        {
            return MoveResult.InvalidPromotion;
        }

        // Move is valid and legal. Move pieces and update gamestate
        var pieceCaptured = to.Piece is not null || isEnPassantMove;
        UpdateEnPassantSquare(from, to);
        MovePieces(from, to, isEnPassantMove, isCastlingMove, promotionPieceChar);
        UpdateHalfMoveCount(piece, pieceCaptured);
        EndTurn();

        return MoveResult.Success;
    }

    private bool IsLegalMove(Square from, Square to)
    {
        var piece = from.Piece;
        if (piece is null)
        {
            return false;
        }

        var isEnPassantMove = IsEnPassantMove(from, to);
        var isCastlingMove = IsCastlingMove(from, to);

        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove || isCastlingMove))
        {
            return false;
        }

        return !WouldLeaveKingInCheck(from, to, isEnPassantMove, isCastlingMove, piece.Color);
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

    private bool IsCastlingAttempt(Square from, Square to)
    {
        if (from.Piece is not King king)
        {
            return false;
        }

        var isWhiteKingMoveOnFirstRank = king.IsWhite && from.Rank == Rank.First && to.Rank == Rank.First;
        var isBlackKingMoveOnEighthRank = king.IsBlack && from.Rank == Rank.Eighth && to.Rank == Rank.Eighth;
        var isFileDifferenceGreaterThanTwo = from.File.DistanceTo(to.File) >= 2;

        return isFileDifferenceGreaterThanTwo && (isWhiteKingMoveOnFirstRank || isBlackKingMoveOnEighthRank);
    }

    private bool IsCastlingMove(Square from, Square to)
    {
        if (!IsCastlingAttempt(from, to))
        {
            return false;
        }

        var king = (King)from.Piece!;
        var direction = from.File < to.File ? Direction.Right : Direction.Left;

        if (!CanCastleInDirection(king, direction))
        {
            return false;
        }

        var currentFile = from.File;
        var destinationFile = from.File + 2 * direction;
        while (currentFile != destinationFile)
        {
            currentFile += direction;
            if (Board[currentFile, from.Rank].IsOccupied())
            {
                return false;
            }
        }

        // When castling queenside, need to check the b-file for obstructions because the rook needs to move through it
        if (currentFile < from.File && Board[File.B, from.Rank].IsOccupied())
        {
            return false;
        }

        if (IsCastlingPathUnderAttack(from, direction, king.OpposingColor))
        {
            return false;
        }

        if (Board.IsKingInCheck(king.Color))
        {
            return false;
        }

        return true;
    }

    private bool CanCastleInDirection(King king, int direction)
    {
        if (CastlingAvailability.CanNeitherSideCastle())
        {
            return false;
        }

        return (king.IsWhite && direction == Direction.Right && CastlingAvailability.CanWhiteCastleKingside()) ||
               (king.IsWhite && direction == Direction.Left && CastlingAvailability.CanWhiteCastleQueenside()) ||
               (king.IsBlack && direction == Direction.Right && CastlingAvailability.CanBlackCastleKingside()) ||
               (king.IsBlack && direction == Direction.Left && CastlingAvailability.CanBlackCastleQueenside());
    }

    private void UpdateEnPassantSquare(Square from, Square to)
    {
        var piece = from.Piece;

        // We already know the move is valid, so to check if it's a pawns first move we only
        // need to check the difference in rank between the start and end square, and can omit
        // checking if the start rank is the second rank (for white) or seventh rank (for black)
        if (piece is Pawn && from.Rank.DistanceTo(to.Rank) == 2)
        {
            var direction = piece.IsWhite ? Direction.Up : Direction.Down;
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
        var isPromotionMove = IsPromotionMove(piece, to);

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
            CastlingAvailability.UpdateAfterRegularMove(piece, from);
        }
    }
    
    private static bool IsPromotionMove(Piece piece, Square to)
    {
        return piece is Pawn && (to.Rank == Rank.First || to.Rank == Rank.Eighth);
    }

    private static bool IsValidPromotionChar(Piece piece, char promotionPieceChar)
    {
        var whitePromotionPieces = new[] { 'Q', 'R', 'B', 'N' };
        var blackPromotionPieces = new[] { 'q', 'r', 'b', 'n' };

        return (piece.IsWhite && whitePromotionPieces.Contains(promotionPieceChar)) ||
               (piece.IsBlack && blackPromotionPieces.Contains(promotionPieceChar));
    }

    private void PerformCastlingMove(Square from, Square to)
    {
        var color = from.Piece!.Color;
        MoveCastlingPieces(Board, from, to);

        CastlingAvailability.UpdateAfterCastlingMove(color);
    }

    private static void MoveCastlingPieces(Board board, Square from, Square to)
    {
        var direction = from.File < to.File ? Direction.Right : Direction.Left;
        var rookFile = from.File < to.File ? File.H : File.A;

        var kingDestinationSquare = board[from.File + 2 * direction, from.Rank];
        board.MovePiece(from: from, to: kingDestinationSquare);
        var rookSquare = board[rookFile, from.Rank];
        var rookDestinationSquare = board[from.File + direction, from.Rank];
        board.MovePiece(from: rookSquare, to: rookDestinationSquare);
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

    private bool WouldLeaveKingInCheck(Square from, Square to, bool isEnPassantMove, bool isCastlingMove, Color movingColor)
    {
        // Only call after checking IsValidMove to avoid unnecessary expensive clones
        Board clone = Board.Clone();
        Square cloneFrom = clone[from.File, from.Rank];
        Square cloneTo = clone[to.File, to.Rank];
        
        if (isCastlingMove)
        {
            MoveCastlingPieces(clone, cloneFrom, cloneTo);
        }
        else
        {
            clone.MovePiece(cloneFrom, cloneTo);
            if (isEnPassantMove)
            {
                clone[to.File, from.Rank].Piece = null;
            }
        }

        return clone.IsKingInCheck(movingColor);
    }

    private bool IsCastlingPathUnderAttack(Square from, int direction, Color opponent)
    {
        return Board.IsSquareUnderAttack(Board[from.File + direction, from.Rank], opponent);
    }

    public IEnumerable<(Square from, Square to)> GetLegalMoves(Color color)
    {
        for (var file = File.A; file <= File.H; file++)
        {
            for (var rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square from = Board[file, rank];
                if (from.Piece is not null && from.Piece.Color == color)
                {
                    foreach (Square to in GetLegalMovesFromSquare(from))
                    {
                        yield return (from, to);
                    }
                }
                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
        }
    }

    private IEnumerable<Square> GetLegalMovesFromSquare(Square from)
    {
        for (var file = File.A; file <= File.H; file++)
        {
            for (var rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square to = Board[file, rank];
                if (IsLegalMove(from, to))
                {
                    yield return to;
                }
                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
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

        Result = EvaluateResult();
    }

    private GameResult EvaluateResult()
    {
        if (GetLegalMoves(Turn).Any())
        {
            return GameResult.InProgress;
        }

        return Board.IsKingInCheck(Turn) ? GameResult.Checkmate : GameResult.Stalemate;
    }
}