using Core.ChessBoard;
using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public abstract class Piece
{
    private readonly IMoveValidator _moveValidator;
    public Color Color { get; }
    public char Name { get; protected init; }
    
    protected Piece(Color color, IMoveValidator moveValidator)
    {
        Color = color;
        _moveValidator = moveValidator;
    }

    public bool HasSameColorAs(Piece piece)
    {
        return Color.Equals(piece.Color);
    }

    public bool IsValidMove(Board board, Square from, Square to)
    {
        return _moveValidator.IsValidMove(board, from, to);
    }

}