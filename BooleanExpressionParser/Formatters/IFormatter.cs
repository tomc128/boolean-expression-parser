using BooleanExpressionParser.Tokens;

namespace BooleanExpressionParser.Formatters;

public interface IFormatter
{
    string FormatTokens(IEnumerable<Token> tokens);

    string FormatTruthTable(Ast ast, List<bool[]> table, string label);

    string JoinTruthTables(params string[] tables);
}