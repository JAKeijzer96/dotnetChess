using Core.Exceptions;

namespace Core.ChessBoard;

public class File
{
    public static readonly File A = new(0);
    public static readonly File B = new(1);
    public static readonly File C = new(2);
    public static readonly File D = new(3);
    public static readonly File E = new(4);
    public static readonly File F = new(5);
    public static readonly File G = new(6);
    public static readonly File H = new(7);

    private int Value { get; }

    private File() // Hide empty constructor
    {
    }

    private File(int value)
    {
        Value = value;
    }

    public static File ParseChar(char fileChar) => GetByValue(fileChar - 'a');

    public int DistanceTo(File other)
    {
        return Math.Abs(Value - other.Value);
    }

    public static implicit operator int(File file) => file.Value;

    public static explicit operator File(int value) => GetByValue(value);

    public static File operator +(File file, int value) => GetByValue(file.Value + value);

    public static File operator -(File file, int value) => GetByValue(file.Value - value);

    public static File operator ++(File file) =>
        file.Value switch
        {
            0 => B,
            1 => C,
            2 => D,
            3 => E,
            4 => F,
            5 => G,
            6 => H,
            _ => throw new OutOfBoardException($"File {file.Value + 1} is out of board (must be between 0 and 7).")
        };

    public static File operator --(File file) =>
        file.Value switch
        {
            1 => A,
            2 => B,
            3 => C,
            4 => D,
            5 => E,
            6 => F,
            7 => G,
            _ => throw new OutOfBoardException($"File {file.Value - 1} is out of board (must be between 0 and 7).")
        };

    public static bool operator >(File file, File other) => file.Value > other.Value;

    public static bool operator <(File file, File other) => file.Value < other.Value;

    public static bool operator >=(File file, File other) => file.Value >= other.Value;

    public static bool operator <=(File file, File other) => file.Value <= other.Value;

    public static bool operator ==(File file, File other) => file.Value == other.Value;
    
    public static bool operator !=(File file, File other) => file.Value != other.Value;

    public override string ToString() =>
        Value switch
        {
            0 => "a",
            1 => "b",
            2 => "c",
            3 => "d",
            4 => "e",
            5 => "f",
            6 => "g",
            7 => "h",
            _ => "" // Invalid case, best practices recommend not throwing an exception
        };

    private static File GetByValue(int value) =>
        value switch
        {
            0 => A,
            1 => B,
            2 => C,
            3 => D,
            4 => E,
            5 => F,
            6 => G,
            7 => H,
            _ => throw new OutOfBoardException($"File {value} is out of board (must be between 0 and 7).")
        };
}