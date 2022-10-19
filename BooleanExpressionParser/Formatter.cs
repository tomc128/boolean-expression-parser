using System.Text;

namespace BooleanExpressionParser;

class Formatter
{
    public static string FormatTokens(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            string s = token.ToString()!;
            if (token is not VariableToken && s.Length > 1) s = $"[{s}]";
            sb.Append(s);
        }

        return sb.ToString();
    }

    public static string FormatTruthTable(Ast ast, List<bool[]> table, string label = "Result")
    {
        var sb = new StringBuilder();

        var variableLine = ast.Variables.Select(v => Repeat('━', v.Length + 2)).ToList();
        var resultLine = Repeat('━', label.Length + 4);

        sb.Append("┏━");
        sb.AppendJoin(null, variableLine);
        sb.AppendLine($"━┳{resultLine}┓");

        sb.Append("┃ ");
        ast.Variables.ForEach(v => sb.Append($" {v} "));
        sb.AppendLine($" ┃  {label}  ┃");

        sb.Append("┣━");
        sb.AppendJoin(null, variableLine);
        sb.AppendLine($"━╋{resultLine}┫");

        foreach (bool[] row in table)
        {
            sb.Append("┃ ");
            for (int i = 0; i < row.Length - 1; i++)
            {
                string pad1 = Repeat(' ', (int)Math.Ceiling(ast.Variables[i].Length / 2.0f));
                string pad2 = Repeat(' ', (int)Math.Floor(ast.Variables[i].Length / 2.0f));
                sb.Append($"{pad1}{(row[i] ? '1' : '0')}{pad2} ");
            }

            string pad3 = Repeat(' ', (int)Math.Ceiling(label.Length / 2.0f));
            string pad4 = Repeat(' ', (int)Math.Floor(label.Length / 2.0f));
            sb.AppendLine($" ┃ {pad3}{(row[^1] ? '1' : '0')}{pad4}  ┃");
        }

        sb.Append("┗━");
        sb.AppendJoin(null, variableLine);
        sb.Append($"━┻{resultLine}┛");

        return sb.ToString();
    }

    static string Repeat(char c, int count) => new string(c, count);

    public static String JoinTruthTables(params string[] tables)
    {
        var sb = new StringBuilder();

        var lines = tables.Select(t => t.Split(Environment.NewLine)).ToList();

        for (int i = 0; i < lines.Max(l => l.Length); i++)
        {
            sb.AppendJoin(" ", lines.Select(l => i < l.Length ? l[i] : Repeat(' ', l[0].Length)));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}