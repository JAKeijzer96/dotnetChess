using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

namespace Core.Parsers;

// Resolves Standard Algebraic Notation (SAN) strings into move coordinates and promotion piece if applicable
internal static class SanResolver
{
    internal static (string from, string to, char promotionPiece) Resolve(string san, Game game)
    {
        string stripped = san.TrimEnd('+', '#');

        if (stripped.StartsWith("O-O")) return ResolveCastling(stripped, game);

        char promotionPieceChar = default;
        int equalSignIndex = stripped.IndexOf('=');
        if (equalSignIndex != -1)
        {
            char promotionLetter = stripped[equalSignIndex + 1];
            promotionPieceChar = game.Turn == Color.White ? char.ToUpper(promotionLetter) : char.ToLower(promotionLetter);
            stripped = stripped[..equalSignIndex];
        }

        if (stripped.Length < 2) throw new InvalidPgnException($"SAN '{san}' is too short to parse.");

        var destinationSquareString = stripped[^2..];
        var leftSide = stripped[..^2];

        var hasPieceLetter = leftSide.Length > 0 && char.IsUpper(leftSide[0]) && "BNQRK".Contains(leftSide[0]);
        var pieceLetter = hasPieceLetter ? leftSide[..1] : string.Empty;
        var afterPiece = hasPieceLetter ? leftSide[1..] : leftSide;
        var rankFileDisambiguation = afterPiece.Replace("x", string.Empty);

        Type pieceType = pieceLetter switch
        {
            "B" => typeof(Bishop),
            "N" => typeof(Knight),
            "Q" => typeof(Queen),
            "R" => typeof(Rook),
            "K" => typeof(King),
            _   => typeof(Pawn)
        };

        Square destinationSquare;
        try
        {
            destinationSquare = game.Board[destinationSquareString];
        }
        catch (Exception ex)
        {
            throw new InvalidPgnException($"SAN '{san}' contains invalid destination square '{destinationSquareString}'.", ex);
        }
        
        var candidates = game.GetLegalMoves(game.Turn)
            .Where(m => m.to.File == destinationSquare.File && m.to.Rank == destinationSquare.Rank)
            .Where(m => m.from.Piece?.GetType() == pieceType)
            .Where(m => MatchesDisambiguation(m.from, rankFileDisambiguation))
            .ToList();

        if (candidates.Count != 1)
            throw new InvalidPgnException($"SAN '{san}' is ambiguous or illegal: {candidates.Count} candidate(s) found.");

        (Square from, Square to) = candidates[0];
        return (from.ToString(), to.ToString(), promotionPieceChar);
    }

    private static (string from, string to, char promotionPiece) ResolveCastling(string san, Game game)
    {
        bool isQueenside = san == "O-O-O";
        var candidates = game.GetLegalMoves(game.Turn)
            .Where(m => m.from.Piece is King)
            .Where(m => isQueenside ? m.to.File < m.from.File : m.to.File > m.from.File)
            .Where(m => m.from.File.DistanceTo(m.to.File) == 2) // Parse castling moves as king moves of exactly two squares
            .ToList();

        if (candidates.Count != 1)
            throw new InvalidPgnException($"SAN '{san}' cannot be resolved: {candidates.Count} candidate(s) found.");

        (Square from, Square to) = candidates[0];
        return (from.ToString(), to.ToString(), default);
    }

    private static bool MatchesDisambiguation(Square from, string disambiguation) => disambiguation.Length switch
    {
        0 => true,
        1 => char.IsLetter(disambiguation[0])
                ? from.File.ToString() == disambiguation
                : from.Rank.ToString() == disambiguation,
        2 => from.ToString() == disambiguation,
        _ => false
    };
}
