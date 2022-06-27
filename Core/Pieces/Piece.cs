using Core.ChessBoard;
using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public abstract class Piece
{
    private readonly IMoveValidator _moveValidator;
    private readonly Color _color;
    public char Name { get; set; }
    
    protected Piece(Color color, IMoveValidator moveValidator)
    {
        _color = color;
        _moveValidator = moveValidator;
    }

    public bool HasSameColor(Piece piece)
    {
        return _color.Equals(piece._color);
    }

    public bool IsValidMove(Board board, Square from, Square to)
    {
        return _moveValidator.IsValidMove(board, from, to);
    }

}