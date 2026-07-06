using System.Collections.Immutable;
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
    public GameResult GameResult { get; private set; }
    public ImmutableList<Move> MoveHistory { get; private set; }

    private readonly string _initialPositionKey;

    public int CurrentMoveIndex { get; private set; }

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        CastlingAvailability = new CastlingAvailability("KQkq");
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
        MoveHistory = [];
        _initialPositionKey = BuildPositionKey(Board, Turn, CastlingAvailability, EnPassant);
        CurrentMoveIndex = 0;
        GameResult = GameResult.InProgress;
    }

    public Game(Board board, Color turn, CastlingAvailability castlingAvailability, Square? enPassant, int halfMoveCount, int fullMoveCount)
        : this(board, turn, castlingAvailability, enPassant, halfMoveCount, fullMoveCount, [], 0, BuildPositionKey(board, turn, castlingAvailability, enPassant))
    { }

    public Game(Board board, Color turn, CastlingAvailability castlingAvailability, Square? enPassant, int halfMoveCount, int fullMoveCount, ImmutableList<Move> moveHistory, int currentMoveIndex, string initialPositionKey)
    {
        Board = board;
        Turn = turn;
        CastlingAvailability = castlingAvailability;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
        MoveHistory = moveHistory;
        _initialPositionKey = initialPositionKey;
        CurrentMoveIndex = currentMoveIndex;
        GameResult = EvaluateResult();
    }
    
    public MakeMoveResult MakeMove(string from, string to, [Optional] char promotionPieceChar)
    {
        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    private MakeMoveResult MakeMove(Square from, Square to, [Optional] char promotionPieceChar)
    {
        if (GameResult != GameResult.InProgress)
        {
            return new MakeMoveResult(MoveResult.GameAlreadyOver, this);
        }

        var piece = from.Piece;
        if (piece is null || piece.Color != Turn)
        {
            return new MakeMoveResult(MoveResult.InvalidPiece, this);
        }

        var isEnPassantMove = IsEnPassantMove(from, to);
        var isCastlingMove = IsCastlingMove(from, to);
        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove || isCastlingMove))
        {
            return new MakeMoveResult(MoveResult.IllegalMove, this);
        }

        if (WouldLeaveKingInCheck(from, to, isEnPassantMove, isCastlingMove, piece.Color))
        {
            return new MakeMoveResult(MoveResult.IllegalMove, this);
        }

        if (IsPromotionMove(piece, to) && !IsValidPromotionChar(piece, promotionPieceChar))
        {
            return new MakeMoveResult(MoveResult.InvalidPromotion, this);
        }

        // Move is valid and legal. Move pieces and return new gamestate
        Game newGame = ApplyMove(from, to, isEnPassantMove, isCastlingMove, promotionPieceChar);
        return new MakeMoveResult(MoveResult.Success, newGame);
    }

    private Game ApplyMove(Square from, Square to, bool isEnPassantMove, bool isCastlingMove, [Optional] char promotionPieceChar)
    {
        var piece = from.Piece!;
        var isPieceCaptured = to.Piece is not null || isEnPassantMove;

        var enPassant = CalculateEnPassantSquare(Board, from, to);
        var board = ApplyMoveToBoard(from, to, isEnPassantMove, isCastlingMove, promotionPieceChar);
        var castlingAvailability = UpdateCastlingAvailability(piece, from, isCastlingMove);
        var halfMoveCount = CalculateHalfMoveCount(piece, isPieceCaptured);
        var turn = CalculateNextTurn();
        var newFullMoveCount = CalculateNextFullMoveCount();

        var positionKey = BuildPositionKey(board, turn, castlingAvailability, enPassant);
        var move = new Move(from, to, promotionPieceChar, positionKey);
        var moveHistory = MoveHistory.Add(move);

        return new Game(board, turn, castlingAvailability, enPassant, halfMoveCount, newFullMoveCount, moveHistory, CurrentMoveIndex + 1, _initialPositionKey);
    }

    public Game GoToNextMove() => GoToMove(CurrentMoveIndex + 1);
    public Game GoToPreviousMove() => GoToMove(CurrentMoveIndex - 1);

    public Game GoToMove(int moveIndex)
    {
        if (moveIndex < 0 || moveIndex > MoveHistory.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(moveIndex), $"Move index must be between 0 and {MoveHistory.Count}");
        }

        if (moveIndex == CurrentMoveIndex)
        {
            return this;
        }

        return ReconstructGameAtMove(moveIndex);
    }

    private Game ReconstructGameAtMove(int targetMoveIndex)
    {
        var game = new Game();

        for (int i = 0; i < targetMoveIndex; i++)
        {
            var move = MoveHistory[i];
            var result = game.MakeMove(move.From, move.To, move.PromotionPiece);
            game = result.Game;
        }

        return new Game(game.Board, game.Turn, game.CastlingAvailability, game.EnPassant,
                        game.HalfMoveCount, game.FullMoveCount, MoveHistory, targetMoveIndex, _initialPositionKey);
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

    private static bool IsCastlingAttempt(Square from, Square to)
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

        var king = (from.Piece as King)!;
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

    private static Square? CalculateEnPassantSquare(Board board, Square from, Square to)
    {
        var piece = from.Piece;

        // We already know the move is valid, so to check if it's a pawns first move we only
        // need to check the difference in rank between the start and end square, and can omit
        // checking if the start rank is the second rank (for white) or seventh rank (for black)
        if (piece is Pawn && from.Rank.DistanceTo(to.Rank) == 2)
        {
            var direction = piece.IsWhite ? Direction.Up : Direction.Down;
            return board[from.File, from.Rank + direction];
        }
        return null;
    }

    private Board ApplyMoveToBoard(Square fromSquare, Square toSquare, bool isEnPassantMove, bool isCastlingMove, [Optional] char promotionPieceChar)
    {
        var board = Board.Clone();
        var from = board[fromSquare.File, fromSquare.Rank];
        var to = board[toSquare.File, toSquare.Rank];

        var piece = from.Piece!;
        var isPromotionMove = IsPromotionMove(piece, to);

        if (isCastlingMove)
        {
            MoveCastlingPieces(board, from, to);
        }
        else if (isPromotionMove)
        {
            PromotePawn(from, to, promotionPieceChar);
        }
        else
        {
            board.MovePiece(from, to);
            if (isEnPassantMove)
            {
                RemoveCapturedEnPassantPawn(board, from, to);
            }
        }

        return board;
    }

    private CastlingAvailability UpdateCastlingAvailability(Piece piece, Square from, bool isCastlingMove)
    {
        if (isCastlingMove)
        {
            return CastlingAvailability.AfterCastlingMove(piece.Color);
        }
        return CastlingAvailability.AfterRegularMove(piece, from);
    }

    private int CalculateHalfMoveCount(Piece movedPiece, bool isPieceCaptured)
    {
        return (movedPiece is Pawn || isPieceCaptured) ? 0 : HalfMoveCount + 1;
    }

    private Color CalculateNextTurn()
    {
        return Turn == Color.White ? Color.Black : Color.White;
    }

    private int CalculateNextFullMoveCount()
    {
        return Turn == Color.Black ? FullMoveCount + 1 : FullMoveCount;
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

    private static void RemoveCapturedEnPassantPawn(Board board, Square from, Square to)
    {
        board[to.File, from.Rank].Piece = null;
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

    private GameResult EvaluateResult()
    {
        if (IsDrawBySeventyFiveMoveRule()) return GameResult.DrawBySeventyFiveMoveRule;
        if (IsFivefoldRepetition()) return GameResult.DrawByFivefoldRepetition;
        if (IsInsufficientMaterial()) return GameResult.DrawByInsufficientMaterial;
        if (!GetLegalMoves(Turn).Any()) return Board.IsKingInCheck(Turn) ? GameResult.Checkmate : GameResult.Stalemate;
        return GameResult.InProgress;
    }

    private bool IsDrawBySeventyFiveMoveRule()
    {
        return HalfMoveCount >= 150;
    }

    private bool IsFivefoldRepetition()
    {
        string currentPosition = GetPositionKey();
        int count = MoveHistory
            .Count(m => m.PositionAfterMove == currentPosition);

        if (currentPosition == _initialPositionKey)
        {
            count++;
        }

        return count >= 5;
    }

    private bool IsInsufficientMaterial()
    {
        // Insufficient material scenarios:
        // King vs King
        // King + Bishop vs King
        // King + Knight vs King
        // King + Bishop vs King + Bishop, with bishops on the same color

        var pieces = Board.GetAllPieces().ToList();
        var whitePieces = pieces.Where(p => p.IsWhite).ToList();
        var blackPieces = pieces.Where(p => p.IsBlack).ToList();

        if (whitePieces.Count == 1 && blackPieces.Count == 1)
        {
            return true;
        }

        if (whitePieces.Count == 2 && blackPieces.Count == 1)
        {
            return whitePieces.Any(p => p is Bishop or Knight);
        }
        if (blackPieces.Count == 2 && whitePieces.Count == 1)
        {
            return blackPieces.Any(p => p is Bishop or Knight);
        }

        if (whitePieces.Count == 2 && blackPieces.Count == 2)
        {
            var whiteBishop = whitePieces.FirstOrDefault(p => p is Bishop);
            var blackBishop = blackPieces.FirstOrDefault(p => p is Bishop);

            if (whiteBishop is null || blackBishop is null)
            {
                return false;
            }
           
            Square whiteSquare = GetSquareWithPiece(whiteBishop)!;
            Square blackSquare = GetSquareWithPiece(blackBishop)!;

            bool whiteOnLightSquare = ((int)whiteSquare.File + (int)whiteSquare.Rank) % 2 == 0;
            bool blackOnLightSquare = ((int)blackSquare.File + (int)blackSquare.Rank) % 2 == 0;
            return whiteOnLightSquare == blackOnLightSquare;
        }

        return false;
    }

    private Square? GetSquareWithPiece(Piece piece)
    {
        for (File file = File.A; file <= File.H; file++)
        {
            for (Rank rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square square = Board[file, rank];
                if (square.Piece == piece)
                {
                    return square;
                }
                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
        }
        return null;
    }

    private string GetPositionKey()
    {
        return BuildPositionKey(Board, Turn, CastlingAvailability, EnPassant);
    }

    private static string BuildPositionKey(Board board, Color turn, CastlingAvailability castling, Square? enPassant)
    {
        // Identical position according to FIDE Laws of Chess Article 9.2. (Dresden, 2008)
        string boardFen = board.ToString();
        string turnStr = turn == Color.White ? "w" : "b";
        string castlingStr = castling.ToString();
        string enPassantStr = enPassant?.ToString() ?? "-";
        return $"{boardFen} {turnStr} {castlingStr} {enPassantStr}";
    }
}