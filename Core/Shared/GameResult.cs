namespace Core.Shared;

public enum GameResult
{
    InProgress,
    Checkmate,
    Stalemate,
    DrawByFiftyMoveRule,
    DrawByInsufficientMaterial,
    DrawByFivefoldRepetition
}
