using Core.Shared;

namespace Core.Pieces;

public static class PieceFactory
{
    public static Piece CreatePiece(char pieceChar) => pieceChar switch {
        'K' => new King(Color.White),
        'Q' => new Queen(Color.White),
        'R' => new Rook(Color.White),
        'B' => new Bishop(Color.White),
        'N' => new Knight(Color.White),
        'P' => new Pawn(Color.White),
        'k' => new King(Color.Black),
        'q' => new Queen(Color.Black),
        'r' => new Rook(Color.Black),
        'b' => new Bishop(Color.Black),
        'n' => new Knight(Color.Black),
        'p' => new Pawn(Color.Black),
        _ => throw new ArgumentException($"Invalid piece character: '{pieceChar}'")
    };

    public static Piece CreatePieceWithFirstMove(char pieceChar, bool isFirstMove) => pieceChar switch
    {
        'P' => new Pawn(Color.White, isFirstMove),
        'p' => new Pawn(Color.Black, isFirstMove),
        _ => throw new ArgumentException($"Invalid piece character: '{pieceChar}'")
    };
}