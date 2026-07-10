/*
 * PGN (Portable Game Notation) — Implemented Subset
 *
 * STRUCTURE
 *   A PGN file consists of two sections:
 *     1. Tag pairs  — zero or more lines of the form: [Name "Value"]
 *     2. Movetext   — the sequence of moves in SAN, terminated by a result token
 *
 * TAG PAIRS
 *   The seven roster tags (always present, in this order):
 *     [Event "?"]  [Site "?"]  [Date "????.??.??"]  [Round "?"]
 *     [White "?"]  [Black "?"] [Result "1-0" | "0-1" | "1/2-1/2" | "*"]
 *   For games starting from a non-standard position, an additional FEN tag is required:
 *     [FEN "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4"]
 *     This tag is also included in the serialized PGN output when the game does not start from the standard initial position.
 *
 * MOVETEXT / SAN (Standard Algebraic Notation)
 *   Move numbers precede White's move and follow a variation open:
 *     1. e4 e5 2. Nf3 ...
 *   After a variation closes ')' and the main line resumes with Black's move,
 *   the move number is repeated with ellipsis:
 *     1. e4 (1. d4 d5) 1... e5
 *
 *   SAN token format:
 *     Piece moves : [BNQRK] [file|rank|both]? x? [a-h][1-8]          [+|#]?
 *     Pawn moves  :         [file]?           x? [a-h][1-8] [=BNQR]? [+|#]?
 *     Castling    : O-O | O-O-O                                      [+|#]?
 *
 *   Disambiguation: added when two (or more) pieces of the same type can legally
 *   reach the same destination square.
 *     - file alone  when the two pieces are on different files
 *     - rank alone  when they share the same file but differ in rank
 *     - both        when neither alone is sufficient (rare, three pieces)
 *
 * VARIATIONS
 *   A variation is an alternative line enclosed in parentheses, branching from
 *   the move immediately preceding the opening '(':
 *     1. e4 (1. d4 d5 2. c4) 1... e5
 *   Variations may be nested arbitrarily.
 *
 * RESULT TOKENS
 *   1-0      White wins
 *   0-1      Black wins
 *   1/2-1/2  Draw
 *   *        Game in progress / no result
 *
 * OUT OF SCOPE
 *   - NAG annotations ($1, $2, ...)
 *   - Comments ({ ... } and ; ...)
 *   - Clock annotations (%clk, %emt)
 *
 * DECISIONS
 *   FEN fullmove count vs. movetext move numbers:
 *     The fullmove count from the FEN tag is leading. Move numbers in the movetext
 *     are parsed for structure only and will be overwritten by the FEN value.
 *
 *   Check/checkmate annotations (+/#) in movetext:
 *     Annotations are stripped before SAN resolution and not semantically validated.
 *     If the movetext omits a '+' or '#' that the position warrants, it is parsed as
 *     a legal move regardless. If the movetext includes '+' or '#' for a move that
 *     is not actually check(mate), it is still parsed as a legal move. Annotation
 *     accuracy is the responsibility of the PGN producer, not the parser.
 */

using System.Collections.Immutable;
using System.Text;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

namespace Core.Parsers;

public static class PgnParser
{
    public static string Serialize(Game game)
    {
        var sb = new StringBuilder();

        AppendTags(sb, game);
        sb.AppendLine();
        AppendMovetext(sb, game);

        return sb.ToString();
    }

    private static readonly IReadOnlySet<string> RequiredTags =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Event", "Site", "Date", "Round", "White", "Black", "Result" };

    public static Game Parse(string pgn)
    {
        ArgumentNullException.ThrowIfNull(pgn);

        var stack = new Stack<Game>();
        var game = new Game();
        var seenTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string? resultTagValue = null;

        foreach (PgnToken token in Tokenize(pgn))
        {
            switch (token.Type)
            {
                case PgnTokenType.Tag:
                    string? tagName = ParseTagName(token.Value);
                    if (tagName is not null)
                        seenTags.Add(tagName);
                    string? fen = TryParseFenTag(token.Value);
                    if (fen is not null)
                        try
                        {
                            game = FenParser.Parse(fen);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPgnException("An error occured when parsing the FEN tag", ex);
                        }
                    string? resultTag = TryParseResultTag(token.Value);
                    if (resultTag is not null)
                        resultTagValue = resultTag;
                    break;

                case PgnTokenType.San:
                    (string from, string to, char promotionPiece) = SanResolver.Resolve(token.Value, game);
                    MakeMoveResult result = game.MakeMove(from, to, promotionPiece);
                    if (result.MoveResult != MoveResult.Success)
                        throw new InvalidPgnException($"Failed to apply move '{token.Value}': {result.MoveResult}.");
                    game = result.Game;
                    break;

                case PgnTokenType.VariationOpen:
                    if (game.CurrentMoveIndex == 0)
                        throw new InvalidPgnException("Unexpected '(' in PGN: no moves to branch from.");
                    stack.Push(game);
                    game = game.GoToPreviousMove();
                    break;

                case PgnTokenType.VariationClose:
                    if (stack.Count == 0)
                        throw new InvalidPgnException("Unexpected ')' in PGN: no matching '('.");
                    Game savedGame = stack.Pop();
                    game = new Game(
                        savedGame.Board,
                        savedGame.Turn,
                        savedGame.CastlingAvailability,
                        savedGame.EnPassant,
                        savedGame.HalfMoveCount,
                        savedGame.FullMoveCount,
                        game.RootContinuations,
                        savedGame.CurrentPath,
                        savedGame.InitialGame
                    );
                    break;

                case PgnTokenType.Result:
                    foreach (string required in RequiredTags)
                    {
                        if (!seenTags.Contains(required)) throw new InvalidPgnException($"Missing required tag: [{required}].");
                    }
                    if (resultTagValue is not null && token.Value != resultTagValue)
                        throw new InvalidPgnException($"Result tag '[Result \"{resultTagValue}\"]' does not match movetext result '{token.Value}'.");
                    return game;
            }
        }

        throw new InvalidPgnException($"Invalid PGN: {pgn}");
    }

    private static void AppendTags(StringBuilder sb, Game game)
    {
        sb.AppendLine("[Event \"?\"]");
        sb.AppendLine("[Site \"?\"]");
        sb.AppendLine("[Date \"????.??.??\"]");
        sb.AppendLine("[Round \"?\"]");
        sb.AppendLine("[White \"?\"]");
        sb.AppendLine("[Black \"?\"]");
        sb.AppendLine($"[Result \"{GameResultToken(game.GameResult, game.Turn)}\"]");

        string initialFen = FenParser.Serialize(game.InitialGame);
        if (initialFen != FenParser.Serialize(new Game()))
            sb.AppendLine($"[FEN \"{initialFen}\"]");
    }

    private static void AppendMovetext(StringBuilder sb, Game game)
    {
        AppendContinuations(sb, game.RootContinuations, game.InitialGame, isFirstToken: true);
        sb.Append(' ');
        sb.Append(GameResultToken(game.GameResult, game.Turn));
    }

    // Recursively append the main line and all variation branches
    private static void AppendContinuations(
        StringBuilder sb,
        ImmutableList<MoveNode> continuations,
        Game game,
        bool isFirstToken = false,
        bool resumeWithMoveNumber = false)
    {
        if (continuations.IsEmpty) return;

        MoveNode mainLineNode = continuations[0];
        bool isWhiteMove = game.Turn == Color.White;

        if (isWhiteMove || isFirstToken || resumeWithMoveNumber)
        {
            if (!isFirstToken)
            {
                sb.Append(' ');
            }
            sb.Append(game.FullMoveCount);
            sb.Append(isWhiteMove ? "." : "...");
        }

        Game gameAfterMove = game.MakeMove(mainLineNode.Move.From.ToString(), mainLineNode.Move.To.ToString(), mainLineNode.Move.PromotionPiece).Game;
        sb.Append(' ');
        sb.Append(ToSan(mainLineNode.Move, game, gameAfterMove));

        for (var i = 1; i < continuations.Count; i++)
        {
            AppendVariation(sb, continuations[i], game, isWhiteMove);
        }

        var resumeWithMoveNumberAfterVariation = continuations.Count > 1 && isWhiteMove;
        AppendContinuations(sb, mainLineNode.Continuations, gameAfterMove, resumeWithMoveNumber: resumeWithMoveNumberAfterVariation);
    }

    private static void AppendVariation(StringBuilder sb, MoveNode node, Game game, bool isWhiteMove)
    {
        sb.Append(" (");
        sb.Append(game.FullMoveCount);
        sb.Append(isWhiteMove ? ". " : "... ");

        Game gameAfterMove = game.MakeMove(node.Move.From.ToString(), node.Move.To.ToString(), node.Move.PromotionPiece).Game;
        sb.Append(ToSan(node.Move, game, gameAfterMove));

        AppendContinuations(sb, node.Continuations, gameAfterMove, resumeWithMoveNumber: !isWhiteMove);
        sb.Append(')');
    }

    private static string ToSan(Move move, Game gameBefore, Game gameAfter)
    {
        if (move.IsCastling) return BuildCastlingSan(move, gameAfter);

        var sb = new StringBuilder();

        Square from = gameBefore.Board[move.From.File, move.From.Rank];
        Piece piece = from.Piece!;

        if (piece is not Pawn)
        {
            sb.Append(char.ToUpper(piece.Name));
        }

        string? fromSquareDisambiguation = GetFromSquareDisambiguation(move, gameBefore, piece);
        if (fromSquareDisambiguation is not null)
        {
            sb.Append(fromSquareDisambiguation);
        }

        if (move.IsCapture)
        {
            if (piece is Pawn)
            {
                sb.Append(move.From.File.ToString());
            }
            sb.Append('x');
        }

        sb.Append(move.To.File.ToString());
        sb.Append(move.To.Rank.ToString());

        if (move.PromotionPiece != default)
        {
            sb.Append('=');
            sb.Append(char.ToUpper(move.PromotionPiece));
        }

        sb.Append(CheckSuffix(gameAfter));

        return sb.ToString();
    }

    private static string BuildCastlingSan(Move move, Game gameAfter)
    {
        string castling = move.To.File < move.From.File ? "O-O-O" : "O-O";
        return castling + CheckSuffix(gameAfter);
    }

    private static string? GetFromSquareDisambiguation(Move move, Game gameBefore, Piece piece)
    {
        if (piece is Pawn) return null;

        var ambiguous = gameBefore.GetLegalMoves(piece.Color)
            .Where(m =>
                m.to.File == move.To.File &&
                m.to.Rank == move.To.Rank &&
                (m.from.File != move.From.File || m.from.Rank != move.From.Rank))
            .Where(m => gameBefore.Board[m.from.File, m.from.Rank].Piece?.GetType() == piece.GetType())
            .ToList();

        if (ambiguous.Count == 0) return null;

        bool fileUnique = ambiguous.All(m => m.from.File != move.From.File);
        if (fileUnique) return move.From.File.ToString();

        bool rankUnique = ambiguous.All(m => m.from.Rank != move.From.Rank);
        if (rankUnique) return move.From.Rank.ToString();

        return move.From.File.ToString() + move.From.Rank.ToString();
    }

    private static string CheckSuffix(Game gameAfter)
    {
        if (gameAfter.GameResult == GameResult.Checkmate) return "#";
        if (gameAfter.Board.IsKingInCheck(gameAfter.Turn)) return "+";
        return string.Empty;
    }

    private static string GameResultToken(GameResult result, Color? turn = null) => result switch
    {
        GameResult.Checkmate => turn == Color.White ? "0-1" : "1-0",
        GameResult.Stalemate or
        GameResult.DrawBySeventyFiveMoveRule or
        GameResult.DrawByInsufficientMaterial or
        GameResult.DrawByFivefoldRepetition => "1/2-1/2",
        _ => "*"
    };

    private static IEnumerable<PgnToken> Tokenize(string pgn)
    {
        pgn = pgn.Replace("\r\n", "\n").Replace("\r", "\n");
        var sb = new StringBuilder();
        foreach (string line in pgn.Split('\n'))
        {
            string trimmed = line.TrimStart();
            if (trimmed.StartsWith('[')) { yield return new PgnToken(PgnTokenType.Tag, trimmed); continue; }

            int semicolonIndex = line.IndexOf(';');
            sb.Append(semicolonIndex >= 0 ? line[..semicolonIndex] : line);
            sb.Append(' ');
        }

        string text = StripBraceComments(sb.ToString());
        text = text.Replace("(", " ( ").Replace(")", " ) ");

        foreach (string token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (IsMoveNumber(token) || token.StartsWith('$')) continue;
            if (token == "(") { yield return new PgnToken(PgnTokenType.VariationOpen, token); continue; }
            if (token == ")") { yield return new PgnToken(PgnTokenType.VariationClose, token); continue; }
            if (IsResultToken(token)) { yield return new PgnToken(PgnTokenType.Result, token); continue; }
            yield return new PgnToken(PgnTokenType.San, token);
        }
    }

    private static string StripBraceComments(string text)
    {
        var sb = new StringBuilder(text.Length);
        int depth = 0;
        foreach (char c in text)
        {
            if (c == '{') { depth++; continue; }
            if (c == '}') { depth--; continue; }
            if (depth == 0) sb.Append(c);
        }
        return sb.ToString();
    }

    private static bool IsMoveNumber(string token)
    {
        int i = 0;
        while (i < token.Length && char.IsDigit(token[i])) i++;
        return i > 0 && token[i..].TrimStart('.').Length == 0;
    }

    private static bool IsResultToken(string token) => token is "1-0" or "0-1" or "1/2-1/2" or "*";

    private static string? TryParseFenTag(string tag)
    {
        if (!tag.StartsWith("[FEN ", StringComparison.OrdinalIgnoreCase)) return null;
        int start = tag.IndexOf('"');
        int end = tag.LastIndexOf('"');
        if (start < 0 || end < start) return null;
        return tag[(start + 1)..end];
    }

    private static string? TryParseResultTag(string tag)
    {
        if (!tag.StartsWith("[Result ", StringComparison.OrdinalIgnoreCase)) return null;
        int start = tag.IndexOf('"');
        int end = tag.LastIndexOf('"');
        if (start < 0 || end < start) return null;
        return tag[(start + 1)..end];
    }

    private static string? ParseTagName(string tag)
    {
        if (!tag.StartsWith('[')) return null;
        int nameEnd = tag.IndexOf(' ');
        if (nameEnd < 0) return null;
        return tag[1..nameEnd];
    }
}
