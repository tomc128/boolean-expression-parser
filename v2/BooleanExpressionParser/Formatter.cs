using System.Text;

namespace BooleanExpressionParser;

class Formatter
{
    public static string FormatInfixTokens(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            sb.Append(token.ToString());
        }

        return sb.ToString();
    }

    public static string FormatPrefixTokens(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            sb.Append(token.ToString());
            sb.Append(" ");
        }

        return sb.ToString();
    }

    public static string FormatTruthTable(Ast ast, List<bool[]> table)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"┏{Repeat('━', ast.Variables.Count * 3 + 2)}┳{Repeat('━', 8)}┓");
        sb.AppendLine($"┃  {String.Join("  ", ast.Variables)}  ┃ Result ┃");
        sb.AppendLine($"┣{Repeat('━', ast.Variables.Count * 3 + 2)}╋{Repeat('━', 8)}┫");

        foreach (var row in table)
        {
            sb.AppendLine($"┃  {String.Join("  ", row[0..^1].Select(b => b ? "1" : "0"))}  ┃   {(row[^1] ? "1" : "0")}    ┃");
        }

        sb.AppendLine($"┗{Repeat('━', ast.Variables.Count * 3 + 2)}┻{Repeat('━', 8)}┛");

        return sb.ToString();
    }

    static string Repeat(char c, int count) => new string(c, count);
}