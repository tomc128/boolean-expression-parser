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

        var variableLine = ast.Variables.Select(v => Repeat('━', v.Length + 2)).ToList();

        sb.Append("┏━");
        variableLine.ForEach(s => sb.Append(s));
        sb.AppendLine("━┳━━━━━━━━┓");

        sb.Append("┃ ");
        ast.Variables.ForEach(v => sb.Append($" {v} "));
        sb.AppendLine(" ┃ Result ┃");

        sb.Append("┣━");
        variableLine.ForEach(s => sb.Append(s));
        sb.AppendLine("━╋━━━━━━━━┫");

        foreach (bool[] row in table)
        {
            sb.Append("┃ ");
            for (int i = 0; i < row.Length - 1; i++)
            {
                string pad1 = Repeat(' ', (int)Math.Ceiling(ast.Variables[i].Length / 2.0f));
                string pad2 = Repeat(' ', (int)Math.Floor(ast.Variables[i].Length / 2.0f));
                sb.Append($"{pad1}{(row[i] ? '1' : '0')}{pad2} ");
            }

            sb.AppendLine($" ┃   {(row[^1] ? '1' : '0')}    ┃");
        }

        sb.Append("┗━");
        variableLine.ForEach(s => sb.Append(s));
        sb.AppendLine("━┻━━━━━━━━┛");

        return sb.ToString();
    }

    static string Repeat(char c, int count) => new string(c, count);
}