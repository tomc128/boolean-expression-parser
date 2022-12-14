using BooleanExpressionParser;
using BooleanExpressionParser.Formatter;
using System.Text;

namespace BooleanExpressionParserWeb.Client;

public class HTMLFormatter : IFormatter
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

    public string FormatTruthTable(Ast ast, List<bool[]> table, string label)
    {
        var sb = new StringBuilder();

        // Generate HTML representation of the truth table
        // table
        //   tr
        //     th th th
        //   tr ***
        //     td td td

        sb.Append("<table class=\"truth-table\">");

        sb.Append("<tr>");
        for (int i = 0; i < ast.Variables.Count; i++)
        {
            string? item = ast.Variables[i];
            sb.Append($"<th>{item}</th>");
        }
        sb.Append($"<th>{label}</th>");
        sb.Append("</tr>");

        foreach (bool[] row in table)
        {
            sb.Append("<tr>");
            for (int i = 0; i < row.Length; i++)
            {
                sb.Append($"<td class=\"{(row[i] ? "true" : "false")}\">{(row[i] ? "1" : "0")}</td>");
            }
            sb.Append("</tr>");
        }

        return sb.ToString();
    }

    public string JoinTruthTables(params string[] tables)
    {
        throw new NotImplementedException();
    }
}