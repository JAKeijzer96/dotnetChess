using Core.ChessBoard;
using System.Runtime.InteropServices;

namespace Core.Shared;

public record Move(
    Square From,
    Square To,
    [Optional] char PromotionPiece,
    MoveResult Result,
    string PositionAfterMove
);
