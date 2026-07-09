using Core.ChessBoard;
using System.Runtime.InteropServices;

namespace Core.Shared;

public record Move(
    Square From,
    Square To,
    string PositionAfterMove,
    bool IsCapture,
    bool IsCastling,
    [Optional] char PromotionPiece
)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
