using System.Text.RegularExpressions;
using Core.ChessBoard;
using Core.Pieces;
using File = Core.ChessBoard.File;

namespace Core.ChessGame;

public class CastlingAvailability
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

    public void UpdateAfterCastlingMove(bool isWhite)
    {
        _castlingAvailability = isWhite switch
        {
            true => _castlingAvailability.Replace("K", "").Replace("Q", ""),
            false => _castlingAvailability.Replace("k", "").Replace("q", "")
        };

        if (_castlingAvailability == "")
        {
            _castlingAvailability = "-";
        }
    }

    public void UpdateAfterRegularMove(Piece piece, Square from)
    {
        if (CanNeitherSideCastle())
        {
            return;
        }

        var pieceIsWhite = piece.IsWhite();
        if (piece is King)
        {
            if (pieceIsWhite && CanWhiteCastle())
            {
                _castlingAvailability = _castlingAvailability.Replace("K", "").Replace("Q", "");
            }
            else if (CanBlackCastle())
            {
                _castlingAvailability = _castlingAvailability.Replace("k", "").Replace("q", "");
            }
        }
        else if (piece is Rook)
        {
            _castlingAvailability = pieceIsWhite switch
            {
                true when from.File == File.A => _castlingAvailability.Replace("Q", ""),
                true when from.File == File.H => _castlingAvailability.Replace("K", ""),
                false when from.File == File.A => _castlingAvailability.Replace("q", ""),
                false when from.File == File.H => _castlingAvailability.Replace("k", ""),
                _ => _castlingAvailability
            };
        }

        if (_castlingAvailability == "")
        {
            _castlingAvailability = "-";
        }
    }

    public override string ToString()
    {
        return _castlingAvailability;
    }

    private bool CanWhiteCastle() => CanWhiteCastleKingside() || CanWhiteCastleQueenside();
    private bool CanBlackCastle() => CanBlackCastleKingside() || CanWhiteCastleQueenside();

    private static void ValidateCastlingAvailability(string castling)
    {
        if (string.IsNullOrEmpty(castling) || !Regex.Match(castling, @"^(-|(K?Q?k?q?))$").Success)
        {
            throw new FormatException("Invalid castling format: " + castling);
        }
    }
}