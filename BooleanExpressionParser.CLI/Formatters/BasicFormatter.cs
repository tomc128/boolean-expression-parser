using System.Text;
using BooleanExpressionParser;
using BooleanExpressionParser.Tokens;

namespace BooleanExpressionParser.CLI.Formatters;

public class BasicFormatter : IFormatter
{
    public string FormatTokens(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (Token token in tokens)
        {
            string s = token.ToString()!;
            if (token is OperatorToken && s.Length > 1) s = $"[[{s}]]";
            sb.Append(s);
        }

        return sb.ToString();
    }

    public string FormatTruthTable(Ast ast, List<bool[]> table, string label = "Result")
    {
        // Return a string representation of the truth table, that is simplistic and machine readable
        // Format: <var1>,<var2>,...<label>;<row1,0>,<row1,1>,...<row1,result>;...

        var sb = new StringBuilder();

        // Header
        sb.AppendJoin(',', ast.Variables);
        sb.Append(',');
        sb.Append(label);
        sb.Append(';');

        // Rows
        for (int i = 0; i < table.Count; i++)
        {
            sb.AppendJoin(',', table[i].Select(b => b ? "1" : "0"));
            sb.Append(';');
        }

        return sb.ToString();
    }

    public string JoinTruthTables(params string[] tables) => string.Join(Environment.NewLine, tables);
}