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
    public Color Turn { get; }
    public CastlingAvailability CastlingAvailability { get; }
    public Square? EnPassant { get; }
    public int HalfMoveCount { get; }
    public int FullMoveCount { get; }
    public GameResult GameResult { get; }

    /* 
     * Structural context belongs to Game, not to nodes. MoveNode.Parent is not used because
     * path-copying an immutable tree creates new node instances up the spine, which would leave
     * any child's Parent pointing at a stale ancestor. Instead, RootContinuations holds the
     * full move tree and CurrentPath holds the ordered sequence of nodes from the root to the
     * current position, giving each Game instance a self-consistent view of the tree.
     */
    internal readonly ImmutableList<MoveNode> RootContinuations;
    internal readonly ImmutableList<MoveNode> CurrentPath;

    internal readonly Game InitialGame;

    private ImmutableList<MoveNode> CurrentContinuations => CurrentPath.IsEmpty ? RootContinuations : CurrentPath[^1].Continuations;
    public int CurrentMoveIndex => CurrentPath.Count;
    public int CurrentBranchLength => CountCurrentBranchLength();

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        CastlingAvailability = new CastlingAvailability("KQkq");
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
        RootContinuations = [];
        CurrentPath = [];
        InitialGame = this;
        GameResult = GameResult.InProgress;
    }

    public Game(Board board, Color turn, CastlingAvailability castlingAvailability, Square? enPassant, int halfMoveCount, int fullMoveCount)
        : this(board, turn, castlingAvailability, enPassant, halfMoveCount, fullMoveCount, [], [], null)
    { }

    internal Game(Board board, Color turn, CastlingAvailability castlingAvailability, Square? enPassant, int halfMoveCount, int fullMoveCount, ImmutableList<MoveNode> rootContinuations, ImmutableList<MoveNode> currentPath, Game? initialGame)
    {
        Board = board;
        Turn = turn;
        CastlingAvailability = castlingAvailability;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
        RootContinuations = rootContinuations;
        CurrentPath = currentPath;
        InitialGame = initialGame ?? this;
        GameResult = EvaluateResult();
    }

    internal Game WithResult(GameResult result) => new(this, result);

    private Game(Game source, GameResult result)
    {
        Board = source.Board;
        Turn = source.Turn;
        CastlingAvailability = source.CastlingAvailability;
        EnPassant = source.EnPassant;
        HalfMoveCount = source.HalfMoveCount;
        FullMoveCount = source.FullMoveCount;
        RootContinuations = source.RootContinuations;
        CurrentPath = source.CurrentPath;
        InitialGame = source.InitialGame;
        GameResult = result;
    }

    public MakeMoveResult MakeMove(string from, string to, [Optional] char promotionPieceChar)
    {
        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    public MakeMoveResult MakeMove(string uci)
    {
        if (uci is not { Length: 4 or 5 })
            return new MakeMoveResult(MoveResult.IllegalMove, this);

        string from = uci[..2];
        string to = uci[2..4];
        char promotionPieceChar = uci.Length == 5
            ? (Turn == Color.White ? char.ToUpper(uci[4]) : char.ToLower(uci[4]))
            : default;

        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    public Game ClaimDraw()
    {
        if (GameResult != GameResult.InProgress) return this;
        if (IsThreefoldRepetition()) return new Game(this, GameResult.DrawByThreefoldRepetition);
        if (IsFiftyMoveRule()) return new Game(this, GameResult.DrawByFiftyMoveRule);
        return new Game(this, GameResult.DrawByAgreement);
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

        ImmutableList<MoveNode> newRootContinuations;
        ImmutableList<MoveNode> newCurrentPath;

        MoveNode? existingNode = FindExistingContinuation(from, to, promotionPieceChar);
        if (existingNode is not null)
        {
            newRootContinuations = RootContinuations;
            newCurrentPath = CurrentPath.Add(existingNode);
        }
        else
        {
            var positionKey = BuildPositionKey(board, turn, castlingAvailability, enPassant);
            var move = new Move(from, to, positionKey, isPieceCaptured, isCastlingMove, promotionPieceChar);
            var newNode = MoveNode.Create(move);
            (newRootContinuations, newCurrentPath) = AddNodeToTree(newNode);
        }

        return new Game(board, turn, castlingAvailability, enPassant, halfMoveCount, newFullMoveCount, newRootContinuations, newCurrentPath, InitialGame);
    }

    public Game GoToNextMove() => GoToMove(CurrentMoveIndex + 1);
    public Game GoToPreviousMove() => GoToMove(CurrentMoveIndex - 1);

    public Game GoToMove(int moveIndex)
    {
        if (moveIndex < 0 || moveIndex > CurrentBranchLength)
        {
            throw new ArgumentOutOfRangeException(nameof(moveIndex), $"Move index must be between 0 and {CurrentBranchLength}");
        }

        if (moveIndex == CurrentMoveIndex)
        {
            return this;
        }

        return ReconstructGameFromPath(CollectPathToIndex(moveIndex));
    }

    public Game GoToVariation(int variationIndex)
    {
        if (variationIndex < 0 || variationIndex >= CurrentContinuations.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(variationIndex), $"Variation index must be between 0 and {CurrentContinuations.Count - 1}");
        }

        return ReconstructGameFromPath(CurrentPath.Add(CurrentContinuations[variationIndex]));
    }

    private Game ReconstructGameFromPath(ImmutableList<MoveNode> targetPath)
    {
        var game = InitialGame;
        foreach (MoveNode node in targetPath)
        {
            var result = game.MakeMove(node.Move.From, node.Move.To, node.Move.PromotionPiece);
            game = result.Game;
        }

        return new Game(game.Board, game.Turn, game.CastlingAvailability, game.EnPassant,
                        game.HalfMoveCount, game.FullMoveCount, RootContinuations, targetPath, InitialGame);
    }

    private ImmutableList<MoveNode> CollectPathToIndex(int targetIndex)
    {
        int pathNodesNeeded = Math.Min(targetIndex, CurrentPath.Count);
        var builder = ImmutableList.CreateBuilder<MoveNode>();

        for (var i = 0; i < pathNodesNeeded; i++)
        {
            builder.Add(CurrentPath[i]);
        }

        if (targetIndex > CurrentPath.Count)
        {
            int stepsForward = targetIndex - CurrentPath.Count;
            MoveNode? current = CurrentContinuations.FirstOrDefault();
            for (var i = 0; i < stepsForward; i++)
            {
                if (current is null) throw new InvalidOperationException("Not enough moves in the branch.");
                builder.Add(current);
                current = current.Continuations.FirstOrDefault();
            }
        }

        return builder.ToImmutable();
    }

    /*
     * Inserting a node into an immutable tree requires rebuilding every ancestor because nodes
     * cannot be mutated. Only the spine from the current leaf up to the root is recreated. All
     * unaffected branches are shared with the previous tree. Cost is O(depth) allocations
     * regardless of total tree size.
     */
    private (ImmutableList<MoveNode> newRootContinuations, ImmutableList<MoveNode> newCurrentPath) AddNodeToTree(MoveNode newNode)
    {
        if (CurrentPath.IsEmpty)
        {
            return (RootContinuations.Add(newNode), CurrentPath.Add(newNode));
        }

        // Attach newNode to the current leaf, then rebuild the spine bottom-up
        var updatedNodes = new MoveNode[CurrentPath.Count];
        updatedNodes[^1] = CurrentPath[^1].WithContinuation(newNode);

        for (var i = CurrentPath.Count - 2; i >= 0; i--)
        {
            // Find where the next path node sits in this node's continuations, then replace it with the updated version.
            int childIndex = FindContinuationIndex(CurrentPath[i].Continuations, CurrentPath[i + 1].Move.Id);
            updatedNodes[i] = CurrentPath[i].ReplaceContinuation(childIndex, updatedNodes[i + 1]);
        }

        // Splice the rebuilt spine back into the root list and advance the current path to newNode.
        int rootIndex = FindContinuationIndex(RootContinuations, CurrentPath[0].Move.Id);
        ImmutableList<MoveNode> newRootContinuations = RootContinuations.SetItem(rootIndex, updatedNodes[0]);
        ImmutableList<MoveNode> newCurrentPath = updatedNodes.ToImmutableList().Add(newNode);

        return (newRootContinuations, newCurrentPath);
    }

    private MoveNode? FindExistingContinuation(Square from, Square to, char promotionPieceChar)
        => CurrentContinuations.FirstOrDefault(n =>
            n.Move.From.File == from.File &&
            n.Move.From.Rank == from.Rank &&
            n.Move.To.File == to.File &&
            n.Move.To.Rank == to.Rank &&
            n.Move.PromotionPiece == promotionPieceChar);

    private static int FindContinuationIndex(ImmutableList<MoveNode> continuations, Guid moveId)
    {
        for (var i = 0; i < continuations.Count; i++)
        {
            if (continuations[i].Move.Id == moveId) return i;
        }
        throw new InvalidOperationException($"Node with move ID {moveId} not found in continuations.");
    }

    private int CountCurrentBranchLength()
    {
        int length = CurrentPath.Count;
        MoveNode? current = CurrentContinuations.FirstOrDefault();
        while (current is not null)
        {
            length++;
            current = current.Continuations.FirstOrDefault();
        }
        return length;
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

        var direction = piece.IsWhite ? Direction.Up : Direction.Down;
        if (from.File.DistanceTo(to.File) != 1) return false;
        if (from.Rank + direction != to.Rank) return false;
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
        var isFileDifferenceExactlyTwo = from.File.DistanceTo(to.File) == 2;

        return isFileDifferenceExactlyTwo && (isWhiteKingMoveOnFirstRank || isBlackKingMoveOnEighthRank);
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

    private bool IsDrawBySeventyFiveMoveRule() => HalfMoveCount >= 150;

    private bool IsFiftyMoveRule() => HalfMoveCount >= 100;

    private bool IsThreefoldRepetition() => CountRepetitions() >= 3;

    private bool IsFivefoldRepetition() => CountRepetitions() >= 5;

    private int CountRepetitions()
    {
        string currentPosition = GetPositionKey();
        int count = CurrentPath.Count(node => node.Move.PositionAfterMove == currentPosition);

        if (currentPosition == InitialGame.GetPositionKey())
        {
            count++;
        }

        return count;
    }

    private bool IsInsufficientMaterial()
    {
        /*
         * Insufficient material scenarios:
         * King vs King
         * King + Bishop vs King
         * King + Knight vs King
         * King + Bishop vs King + Bishop, with bishops on the same color
         */
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