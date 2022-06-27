using Core.ChessBoard;

namespace Core.MoveValidators;

public interface IMoveValidator
{
    bool IsValidMove(Board board, Square from, Square to);
}