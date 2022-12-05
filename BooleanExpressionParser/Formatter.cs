using System.Globalization;
using System.Text;
using Spectre.Console;

namespace BooleanExpressionParser;

class Formatter
{
    private static int FinalPadding = 1;

    private string @true = "1";
    private string @false = "0";
    public string True { get => @true; set => @true = value.Trim(); }
    public string False { get => @false; set => @false = value.Trim(); }

    public string FormatTokens(IEnumerable<Token> tokens)
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

    public string FormatTruthTableOld(Ast ast, List<bool[]> table, string label = "Result")
    {
        var sb = new StringBuilder();

        var maxTrueFalse = Math.Max(True.Length, False.Length);


        var variableLine = new List<string>();
        foreach (var v in ast.Variables)
        {
            var difference = maxTrueFalse - v.Length;
            var maxLength = Math.Max(ast.Variables.Max(x => x.Length), v.Length);
            int pad = (int)Math.Ceiling(maxLength / 2.0f + difference / 2.0f) + (int)Math.Floor(maxLength / 2.0f + difference / 2.0f) + 1;
            variableLine.Add($"{Repeat('━', pad)}");
        }

        // var resultLine = Repeat('━', label.Length + maxTrueFalse + 4);
        var resultDifference = Math.Abs(maxTrueFalse - label.Length);
        var resultMaxLength = Math.Max(maxTrueFalse, label.Length);
        int resultPad = (int)Math.Ceiling(resultMaxLength / 2.0f + resultDifference / 2.0f) + (int)Math.Floor(resultMaxLength / 2.0f + resultDifference / 2.0f) + 1;
        var resultLine = Repeat('━', resultPad);

        sb.Append("┏━");
        sb.AppendJoin(null, variableLine);
        sb.AppendLine($"━┳{resultLine}┓");

        sb.Append("┃");
        // ast.Variables.ForEach(v => sb.Append($" {v} "));
        for (int i = 0; i < ast.Variables.Count; i++)
        {
            string? v = ast.Variables[i];
            var difference = maxTrueFalse - v.Length;
            var maxLength = Math.Max(ast.Variables[i].Length, v.Length);
            string pad1 = Repeat(i % 2 == 0 ? 'x' : 'X', (int)Math.Ceiling(maxLength / 2.0f + difference / 2.0f));
            string pad2 = Repeat(i % 2 == 0 ? 'x' : 'X', (int)Math.Floor(maxLength / 2.0f + difference / 2.0f) + 1);
            sb.Append($"{pad1}{v}{pad2}");
        }
        sb.AppendLine($"┃{label.EscapeMarkup()}┃");

        sb.Append("┣━");
        sb.AppendJoin(null, variableLine);
        sb.AppendLine($"━╋{resultLine}┫");

        foreach (bool[] row in table)
        {
            sb.Append("┃");
            for (int i = 0; i < row.Length - 1; i++)
            {
                var difference1 = maxTrueFalse - (row[i] ? True.Length : False.Length);
                var maxLength = Math.Max(ast.Variables[i].Length, row[i] ? True.Length : False.Length);

                string pad1 = Repeat(i % 2 == 0 ? 'x' : 'X', (int)Math.Ceiling(maxLength / 2.0f + difference1 / 2.0f));
                string pad2 = Repeat(i % 2 == 0 ? 'x' : 'X', (int)Math.Floor(maxLength / 2.0f + difference1 / 2.0f) + 1);
                sb.Append($"{pad1}{(row[i] ? $"[green]{True}[/]" : $"[red]{False}[/]")}{pad2}");
            }

            var difference2 = maxTrueFalse - (row[^1] ? True.Length : False.Length);
            var maxLength2 = Math.Max(label.Length, row[^1] ? True.Length : False.Length);

            string pad3 = Repeat('z', (int)Math.Ceiling(maxLength2 / 2.0f));
            string pad4 = Repeat('z', (int)Math.Floor(maxLength2 / 2.0f + difference2));
            sb.AppendLine($"┃{pad3}{(row[^1] ? $"[green]{True}[/]" : $"[red]{False}[/]")}{pad4}┃");
        }

        sb.Append("┗━");
        sb.AppendJoin(null, variableLine);
        sb.Append($"━┻{resultLine}┛");

        return sb.ToString();
    }

    public string FormatTruthTable(Ast ast, List<bool[]> table, string label = "Result")
    {
        var sb = new StringBuilder();

        var trueInfo = new StringInfo(True);
        var falseInfo = new StringInfo(False);
        // var maxTrueFalse = Math.Max(trueInfo.LengthInTextElements, falseInfo.LengthInTextElements);
        var maxTrueFalse = Math.Max(True.Length, False.Length);

        var horizontalLineTop = "";
        var variableRow = "";
        var horizontalLineMiddle = "";
        var tableRows = new List<string>();
        var horizontalLineBottom = "";

        for (int i = 0; i < ast.Variables.Count; i++)
        {
            string? item = ast.Variables[i];
            var pc = i % 2 == 0 ? '.' : ',';
            var width = Math.Max(item.Length, maxTrueFalse) + 2 * FinalPadding;
            horizontalLineTop += Repeat('━', width);
            horizontalLineMiddle += Repeat('━', width);
            horizontalLineBottom += Repeat('━', width);
            variableRow += $"[bold]{item.PadLeft(width / 2, pc).PadRight(width, pc)}[/]";
        }

        var resultLine = Repeat('━', label.Length + 2 * FinalPadding);

        horizontalLineTop = $"┏{horizontalLineTop}┳{resultLine}┓";
        horizontalLineMiddle = $"┣{horizontalLineMiddle}╋{resultLine}┫";
        horizontalLineBottom = $"┗{horizontalLineBottom}┻{resultLine}┛";
        variableRow = $"┃{variableRow}┃[bold]{label.PadLeft(label.Length / 2 + FinalPadding, 'z').PadRight(label.Length + 2 * FinalPadding, ';')}[/]┃";

        foreach (bool[] row in table)
        {
            var tableRow = "";
            for (int i = 0; i < row.Length - 1; i++)
            {
                var pc = i % 2 == 0 ? '.' : ',';
                var width = Math.Max(ast.Variables[i].Length, maxTrueFalse) + 2 * FinalPadding;
                tableRow += $"{(row[i] ? True : False).PadLeft(width / 2, pc).PadRight(width, pc)}";
            }
            // tableRows.Add($"┃{tableRow}┃{(row[^1] ? True : False).PadLeft(label.Length / 2 + FinalPadding, ';').PadRight(label.Length + 2 * FinalPadding, ';')}┃");
            tableRows.Add($"┃{tableRow}┃{PadUnicode((row[^1] ? True : False), label.Length + 2 * FinalPadding, ';')}┃");
        }

        sb.AppendLine(horizontalLineTop);
        sb.AppendLine(variableRow);
        sb.AppendLine(horizontalLineMiddle);
        foreach (var row in tableRows)
        {
            sb.AppendLine(row);
        }
        sb.AppendLine(horizontalLineBottom);


        return sb.ToString();
    }

    static string Repeat(char c, int count) => new string(c, count);

    static string PadUnicode(string s, int length, char paddingChar = ' ')
    {
        var si = new StringInfo(s);
        var padding = Repeat(paddingChar, length - si.LengthInTextElements);
        return si.SubstringByTextElements(0, si.LengthInTextElements / 2) + padding + si.SubstringByTextElements(si.LengthInTextElements / 2);
    }

    public String JoinTruthTables(params string[] tables)
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