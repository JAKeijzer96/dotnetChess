namespace Core.ChessGame;

public class FenGameState
{
    private object Board;
    private object Turn;
    private object Castling;
    private object EnPassant;
    private object HalfMoveCount;
    private object FullMoveCount;

    public FenGameState(object board, object turn, object castling, object enPassant, object halfMoveCount, object fullMoveCount)
    {
        Board = board;
        Turn = turn;
        Castling = castling;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
    }
}