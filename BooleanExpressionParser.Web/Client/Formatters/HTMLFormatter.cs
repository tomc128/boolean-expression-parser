using BooleanExpressionParser.Formatters;
using BooleanExpressionParser.Tokens;
using System.Text;

namespace BooleanExpressionParser.Web.Client.Formatters;

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

        sb.Append("<thead>");
        sb.Append("<tr>");
        for (int i = 0; i < ast.Variables.Count; i++)
        {
            string? item = ast.Variables[i];
            sb.Append($"<th>{item}</th>");
        }
        sb.Append($"<th class=\"result\">{label}</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");

        sb.Append("<tbody>");
        foreach (bool[] row in table)
        {
            sb.Append("<tr>");
            for (int i = 0; i < row.Length; i++)
            {
                if (i != row.Length - 1)
                    sb.Append($"<td class=\"{(row[i] ? "true" : "false")}\">{(row[i] ? "1" : "0")}</td>");
                else
                    sb.Append($"<td class=\"{(row[i] ? "true" : "false")} result\">{(row[i] ? "1" : "0")}</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");

        sb.Append("</table>");

        return sb.ToString();
    }

    public string JoinTruthTables(params string[] tables)
    {
        throw new NotImplementedException();
    }
}