using Core.ChessGame;

namespace Core.Parsers;

public record ParsedPgn(Game Game, IReadOnlyDictionary<string, string> Tags);
