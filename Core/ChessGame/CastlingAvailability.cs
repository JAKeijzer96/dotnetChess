using System.Text.RegularExpressions;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using File = Core.ChessBoard.File;

namespace Core.ChessGame;

public partial class CastlingAvailability
{
    private string _castlingAvailability;

    public CastlingAvailability(string castlingAvailability)
    {
        ValidateCastlingAvailability(castlingAvailability);

        _castlingAvailability = castlingAvailability;
    }

    public bool CanNeitherSideCastle() => _castlingAvailability == "-";
    public bool CanWhiteCastleKingside() => _castlingAvailability.Contains('K');
    public bool CanWhiteCastleQueenside() => _castlingAvailability.Contains('Q');
    public bool CanBlackCastleKingside() => _castlingAvailability.Contains('k');
    public bool CanBlackCastleQueenside() => _castlingAvailability.Contains('q');

    public CastlingAvailability AfterCastlingMove(Color color)
    {
        var newCastlingAvailability = color switch
        {
            Color.White => _castlingAvailability.Replace("K", "").Replace("Q", ""),
            Color.Black => _castlingAvailability.Replace("k", "").Replace("q", ""),
            _ => throw new InvalidColorException("Invalid color: " + color.ToString())
        };

        if (newCastlingAvailability == "")
        {
            newCastlingAvailability = "-";
        }

        return new CastlingAvailability(newCastlingAvailability);
    }

    public CastlingAvailability AfterRegularMove(Piece piece, Square from)
    {
        if (CanNeitherSideCastle())
        {
            return this;
        }

        var newCastlingAvailability = _castlingAvailability;
        var pieceIsWhite = piece.IsWhite;
        if (piece is King)
        {
            if (pieceIsWhite && CanWhiteCastle())
            {
                newCastlingAvailability = newCastlingAvailability.Replace("K", "").Replace("Q", "");
            }
            else if (CanBlackCastle())
            {
                newCastlingAvailability = newCastlingAvailability.Replace("k", "").Replace("q", "");
            }
        }
        else if (piece is Rook)
        {
            newCastlingAvailability = pieceIsWhite switch
            {
                true when from.File == File.A => newCastlingAvailability.Replace("Q", ""),
                true when from.File == File.H => newCastlingAvailability.Replace("K", ""),
                false when from.File == File.A => newCastlingAvailability.Replace("q", ""),
                false when from.File == File.H => newCastlingAvailability.Replace("k", ""),
                _ => newCastlingAvailability
            };
        }

        if (newCastlingAvailability == "")
        {
            newCastlingAvailability = "-";
        }

        return new CastlingAvailability(newCastlingAvailability);
    }

    public override string ToString()
    {
        return _castlingAvailability;
    }

    private bool CanWhiteCastle() => CanWhiteCastleKingside() || CanWhiteCastleQueenside();
    private bool CanBlackCastle() => CanBlackCastleKingside() || CanWhiteCastleQueenside();

    private static void ValidateCastlingAvailability(string castling)
    {
        if (string.IsNullOrEmpty(castling) || !ValidCastlingRegex().IsMatch(castling))
        {
            throw new FormatException("Invalid castling format: " + castling);
        }
    }

    [GeneratedRegex(@"^(-|(K?Q?k?q?))$")]
    private static partial Regex ValidCastlingRegex();
}