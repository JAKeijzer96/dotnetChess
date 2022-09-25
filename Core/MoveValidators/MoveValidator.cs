using Core.ChessBoard;

namespace Core.MoveValidators;

public abstract class MoveValidator
{
    public abstract bool IsValidMove(Board board, Square from, Square to);
    
    protected virtual bool IsValidDestinationSquare(Board? board, Square? from, Square? to)
    {
        if (board is null) throw new ArgumentNullException(nameof(board));
        if (from is null) throw new ArgumentNullException(nameof(from));
        if (to is null) throw new ArgumentNullException(nameof(to));
        
        return from.Piece is not null &&
               from != to &&
               to.Piece?.Color != from.Piece.Color;
    } 
}