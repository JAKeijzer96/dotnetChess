using System.Runtime.InteropServices;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

namespace Core.ChessGame;

public class Game
{
    public Board Board { get; }
    public Color Turn { get; private set; }
    public string Castling { get; private set; }
    public Square? EnPassant { get; private set; }
    public int HalfMoveCount { get; private set; }
    public int FullMoveCount { get; private set; }

    public Game()
    {
        Board = new Board();
        Turn = Color.White;
        Castling = "KQkq";
        EnPassant = null;
        HalfMoveCount = 0;
        FullMoveCount = 1;
    }

    public Game(Board board, Color turn, string castling, Square? enPassant, int halfMoveCount, int fullMoveCount)
    {
        Board = board;
        Turn = turn;
        Castling = castling;
        EnPassant = enPassant;
        HalfMoveCount = halfMoveCount;
        FullMoveCount = fullMoveCount;
    }
    
    public bool MakeMove(string from, string to, [Optional] char promotionPieceChar)
    {
        return MakeMove(Board[from], Board[to], promotionPieceChar);
    }

    private bool MakeMove(Square from, Square to, [Optional] char promotionPieceChar)
    {
        var piece = from.Piece;
        if (piece is null || piece.Color != Turn)
        {
            return false;
        }

        var isEnPassantMove = IsEnPassantMove(from, to);
        var isCastlingMove = IsCastlingMove(from, to);
        if (!(piece.IsValidMove(Board, from, to) || isEnPassantMove || isCastlingMove))
        {
            return false;
        }
        
        // Move is valid. Move pieces and update gamestate
        var pieceCaptured = to.Piece is not null || isEnPassantMove;
        UpdateEnPassantSquare(from, to);
        MovePieces(from, to, isEnPassantMove, isCastlingMove, promotionPieceChar);
        UpdateHalfMoveCount(piece, pieceCaptured);
        EndTurn();
        
        return true;
    }
    
    private bool IsEnPassantMove(Square from, Square to)
    {
        if (EnPassant is null)
        {
            return false;
        }

        var piece = from.Piece!;

        if (piece is not Pawn)
        {
            return false;
        }

        return to.File == EnPassant.File && to.Rank == EnPassant.Rank;
    }

    private bool IsCastlingMove(Square from, Square to)
    {
        if (from.Piece is not King)
        {
            return false;
        }

        var king = from.Piece;

        if ( ((king.IsWhite() && from.Rank == 0 && to.Rank == 0) || 
              (king.IsBlack() && from.Rank == 7 && to.Rank == 7))
            && Math.Abs(from.File - to.File) >= 2)
        {
            if (Castling == "-")
            {
                throw new InvalidCastlingException($"Cannot castle from {from} to {to} because the " +
                                                   $"king and/or rook have moved (Castling string: {Castling}).");
            }
            var direction = from.File < to.File ? 1 : -1;
            if ((king.IsWhite() && direction ==  1 && !Castling.Contains('K')) ||
                (king.IsWhite() && direction == -1 && !Castling.Contains('Q')) ||
                (king.IsBlack() && direction ==  1 && !Castling.Contains('k')) ||
                (king.IsBlack() && direction == -1 && !Castling.Contains('q')))
            {
                throw new InvalidCastlingException($"Cannot castle from {from} to {to} because the " +
                                                   $"king and/or rook have moved (Castling string: {Castling}).");
            }
            var currentFile = from.File;
            var destinationFile = from.File + 2 * direction;
            while (currentFile != destinationFile)
            {
                currentFile += direction;
                if (Board.GetSquare(currentFile, from.Rank).IsOccupied())
                {
                    // TODO: Remove magic numbers from exception message
                    throw new InvalidCastlingException($"Cannot castle from {from} to {to} because there is a piece blocking on file {(char) (currentFile + 97)}");
                }
            }

            if (currentFile < from.File && Board.GetSquare(1, from.Rank).IsOccupied())
            {
                // TODO: Remove magic numbers from exception message
                
                // When castling queenside, need to check the b-file for obstructions because the rook needs to move through it
                throw new InvalidCastlingException($"Cannot castle from {from} to {to} because there is a piece blocking on file {(char) (1 + 97)}");
            }

            return true;
        }
        // TODO: Refactor (invert if)

        return false;

    }

    private void UpdateEnPassantSquare(Square from, Square to)
    {
        var piece = from.Piece;

        // We already know the move is valid, so to check if it's a pawns first move we only
        // need to check the difference in rank between the start and end square, and can omit
        // checking if the start rank is the second rank (for white) or seventh rank (for black)
        if (piece is Pawn && Math.Abs(from.Rank - to.Rank) == 2)
        {
            var direction = piece.IsWhite() ? 1 : -1;
            EnPassant = Board.GetSquare(from.File, from.Rank + direction);
        }
        else
        {
            EnPassant = null;
        }
    }

    private void MovePieces(Square from, Square to, bool isEnPassantMove, bool isCastlingMove, [Optional] char promotionPieceChar)
    {
        var piece = from.Piece!;
        var isPromotionMove = IsPromotionMove(piece, to, promotionPieceChar);

        if (isCastlingMove)
        {
            PerformCastlingMove(from, to);
        }
        else if (isPromotionMove)
        {
            PromotePawn(from, to, promotionPieceChar);
        } 
        else
        {
            Board.MovePiece(from, to);
            if (isEnPassantMove)
            {
                RemoveCapturedEnPassantPawn(from, to);
            }
            UpdateCastlingAfterRegularMove(piece, from);
        }
    }
    
    private bool IsPromotionMove(Piece piece, Square to, char promotionPieceChar)
    {
        if (piece is not Pawn || to.Rank is not (0 or 7))
        {
            return false;
        }
        
        var whitePromotionPieces = new[] {'Q', 'R', 'B', 'N'};
        var blackPromotionPieces = new[] {'q', 'r', 'b', 'n'};

        if ((piece.IsWhite() && !whitePromotionPieces.Contains(promotionPieceChar)) ||
            (piece.IsBlack() && !blackPromotionPieces.Contains(promotionPieceChar)))
        {
            throw new InvalidPromotionException($"Invalid promotion character {promotionPieceChar} for color {Turn}");
        }

        return true;
    }

    private void PerformCastlingMove(Square from, Square to)
    {
        var direction = from.File < to.File ? 1 : -1;
        var rookFile = from.File < to.File ? 7 : 0;
            
        // Move king and rook
        var kingDestinationSquare = Board.GetSquare(from.File + 2 * direction, from.Rank);
        Board.MovePiece(from: from, to: kingDestinationSquare);
        var rookSquare = Board.GetSquare(rookFile, from.Rank); 
        var rookDestinationSquare = Board.GetSquare(from.File + direction, from.Rank);
        Board.MovePiece(from: rookSquare, to: rookDestinationSquare);
        
        // Update Castling property
        var isWhite = kingDestinationSquare.Piece!.IsWhite(); 
        UpdateCastlingAfterCastlingMove(isWhite);
    }

    private void UpdateCastlingAfterCastlingMove(bool isWhite)
    {
        Castling = isWhite switch
        {
            true  => Castling.Replace("K", "").Replace("Q", ""),
            false => Castling.Replace("k", "").Replace("q", "")
        };

        if (Castling == "")
        {
            Castling = "-";
        }
    }

    private static void PromotePawn(Square from, Square to, char promotionPieceChar)
    {
        from.Piece = null;
        to.Piece = PieceFactory.CreatePiece(promotionPieceChar);
    }

    private void RemoveCapturedEnPassantPawn(Square from, Square to)
    {
        Board.GetSquare(to.File, from.Rank).Piece = null;
    }

    private void UpdateCastlingAfterRegularMove(Piece piece, Square from)
    {
        if (Castling == "-")
        {
            return;
        }
        
        var pieceIsWhite = piece.IsWhite();
        if (piece is King)
        {
            if (pieceIsWhite && (Castling.Contains('K') || Castling.Contains('Q')))
            {
                Castling = Castling.Replace("K", "").Replace("Q", "");
            }
            else if (Castling.Contains('k') || Castling.Contains('q'))
            {
                Castling = Castling.Replace("k", "").Replace("q", "");
            }
        }
        else if (piece is Rook)
        {
            Castling = (pieceIsWhite, from.File) switch
            {
                (true, 0) => Castling.Replace("Q", ""),
                (true, 7) => Castling.Replace("K", ""),
                (false, 0) => Castling.Replace("q", ""),
                (false, 7) => Castling.Replace("k", ""),
                _ => Castling
            };
        }
        
        if (Castling == "")
        {
            Castling = "-";
        }
    }

    private void UpdateHalfMoveCount(Piece movedPiece, bool pieceCaptured)
    {
        if (movedPiece is Pawn || pieceCaptured)
        {
            HalfMoveCount = 0;
        }
        else
        {
            HalfMoveCount++;
        }
    }
    
    private void EndTurn()
    {
        if (Turn == Color.Black)
        {
            Turn = Color.White;
            FullMoveCount++;
        }
        else
        {
            Turn = Color.Black;
        }
    }
}