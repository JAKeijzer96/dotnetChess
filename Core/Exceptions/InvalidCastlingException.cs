﻿using Core.ChessBoard;

namespace Core.Exceptions;

public class InvalidCastlingException : Exception
{
    public InvalidCastlingException()
    {
    }

    public InvalidCastlingException(string? message) : base(message)
    {
    }

    public InvalidCastlingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public InvalidCastlingException(Square from, Square to, int blockedFile) : base(
        $"Cannot castle from {from} to {to} because there is a piece blocking on file {(char) (blockedFile + 97)}")
    {
    }
}