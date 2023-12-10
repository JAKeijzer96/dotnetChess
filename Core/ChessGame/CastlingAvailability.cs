using System.Text.RegularExpressions;
using Core.ChessBoard;
using Core.Pieces;
using File = Core.ChessBoard.File;

namespace Core.ChessGame;

public class CastlingAvailability
{
    private string _castling;

    public CastlingAvailability(string castlingAvailability)
    {
        ValidateCastlingAvailability(castlingAvailability);

        _castling = castlingAvailability;
    }

    public bool CanNeitherSideCastle() => _castling == "-";
    public bool CanWhiteCastleKingside() => _castling.Contains('K');
    public bool CanWhiteCastleQueenside() => _castling.Contains("Q");
    public bool CanBlackCastleKingside() => _castling.Contains('k');
    public bool CanBlackCastleQueenside() => _castling.Contains('q');

    public void UpdateAfterCastlingMove(bool isWhite)
    {
        _castling = isWhite switch
        {
            true => _castling.Replace("K", "").Replace("Q", ""),
            false => _castling.Replace("k", "").Replace("q", "")
        };

        if (_castling == "")
        {
            _castling = "-";
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
                _castling = _castling.Replace("K", "").Replace("Q", "");
            }
            else if (CanBlackCastle())
            {
                _castling = _castling.Replace("k", "").Replace("q", "");
            }
        }
        else if (piece is Rook)
        {
            _castling = pieceIsWhite switch
            {
                true when from.File == File.A => _castling.Replace("Q", ""),
                true when from.File == File.H => _castling.Replace("K", ""),
                false when from.File == File.A => _castling.Replace("q", ""),
                false when from.File == File.H => _castling.Replace("k", ""),
                _ => _castling
            };
        }

        if (_castling == "")
        {
            _castling = "-";
        }
    }

    public override string ToString()
    {
        return _castling;
    }

    private bool CanWhiteCastle() => CanWhiteCastleKingside() || CanWhiteCastleQueenside();
    private bool CanBlackCastle() => CanBlackCastleKingside() || CanWhiteCastleQueenside();

    private static void ValidateCastlingAvailability(string castling)
    {
        if (!(string.IsNullOrEmpty(castling) || Regex.Match(castling, @"^(-|(K?Q?k?q?))$").Success))
        {
            throw new FormatException("Invalid castling format: " + castling);
        }
    }
}