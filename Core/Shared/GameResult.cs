namespace Core.Shared;

public enum GameResult
{
    InProgress,
    Checkmate,
    Stalemate,
    DrawByInsufficientMaterial,
    DrawByThreefoldRepetition,
    DrawByFivefoldRepetition,
    DrawByFiftyMoveRule,
    DrawBySeventyFiveMoveRule,
    DrawByAgreement
}
