using Core.Exceptions;

namespace Core.ChessBoard;

public class Rank
{
    public static readonly Rank First = new(0);
    public static readonly Rank Second = new(1);
    public static readonly Rank Third = new(2);
    public static readonly Rank Fourth = new(3);
    public static readonly Rank Fifth = new(4);
    public static readonly Rank Sixth = new(5);
    public static readonly Rank Seventh = new(6);
    public static readonly Rank Eighth = new(7);
    
    private int Value { get; }

    private Rank()
    {
    }

    private Rank(int value)
    {
        Value = value;
    }

    public static Rank ParseChar(char rankChar) => GetByValue(rankChar - '1');

    public int DistanceTo(Rank other)
    {
        return Math.Abs(Value - other.Value);
    }

    public static implicit operator int(Rank rank) => rank.Value;

    public static explicit operator Rank(int value) => GetByValue(value);

    public static Rank operator +(Rank rank, int value) => GetByValue(rank.Value + value);

    public static Rank operator -(Rank rank, int value) => GetByValue(rank.Value - value);

    public static Rank operator ++(Rank rank) =>
        rank.Value switch
        {
            0 => Second,
            1 => Third,
            2 => Fourth,
            3 => Fifth,
            4 => Sixth,
            5 => Seventh,
            6 => Eighth,
            _ => throw new OutOfBoardException($"Rank {rank.Value + 1} is out of board (must be between 0 and 7).")
        };

    public static Rank operator --(Rank rank) =>
        rank.Value switch
        {
            1 => First,
            2 => Second,
            3 => Third,
            4 => Fourth,
            5 => Fifth,
            6 => Sixth,
            7 => Seventh,
            _ => throw new OutOfBoardException($"Rank {rank.Value - 1} is out of board (must be between 0 and 7).")
        };

    public static bool operator >(Rank rank, Rank other) => rank.Value > other.Value;

    public static bool operator <(Rank rank, Rank other) => rank.Value < other.Value;

    public static bool operator >=(Rank rank, Rank other) => rank.Value >= other.Value;

    public static bool operator <=(Rank rank, Rank other) => rank.Value <= other.Value;

    public static bool operator ==(Rank rank, Rank other) => rank.Value == other.Value;
    
    public static bool operator !=(Rank rank, Rank other) => rank.Value != other.Value;

    public override string ToString() =>
        Value switch
        {
            0 => "1",
            1 => "2",
            2 => "3",
            3 => "4",
            4 => "5",
            5 => "6",
            6 => "7",
            7 => "8",
            _ => "" // Invalid case, best practices recommend not throwing an exception
        };

    private static Rank GetByValue(int value) =>
        value switch
        {
            0 => First,
            1 => Second,
            2 => Third,
            3 => Fourth,
            4 => Fifth,
            5 => Sixth,
            6 => Seventh,
            7 => Eighth,
            _ => throw new OutOfBoardException($"Rank {value} is out of board (must be between 0 and 7).")
        };
}