using BooleanExpressionParser.Tokens;

namespace BooleanExpressionParser;

public interface IFormatter
{
    string FormatTokens(IEnumerable<Token> tokens);

    string FormatTruthTable(Ast ast, List<bool[]> table, string label);

    string JoinTruthTables(params string[] tables);
}