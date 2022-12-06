using System.Globalization;
using System.Text;
using Spectre.Console;

namespace BooleanExpressionParser;

enum ColourMode
{
    None,
    Foreground,
    Background
}


class Formatter
{
    private static int FinalPadding = 2;

    private string @true = "1";
    private string @false = "0";
    public string True { get => @true; set => @true = value.Trim(); }
    public string False { get => @false; set => @false = value.Trim(); }

    public ColourMode ColourMode { get; set; } = ColourMode.Foreground;


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

    public string FormatTruthTable(Ast ast, List<bool[]> table, string label = "Result")
    {
        var sb = new StringBuilder();

        var maxTrueFalse = Math.Max(True.Length, False.Length);
        var maxResultLength = Math.Max(label.Length, maxTrueFalse);

        var horizontalLineTop = "";
        var variableRow = "";
        var horizontalLineMiddle = "";
        var tableRows = new List<string>();
        var horizontalLineBottom = "";

        for (int i = 0; i < ast.Variables.Count; i++)
        {
            string? item = ast.Variables[i];
            var width = Math.Max(item.Length, maxTrueFalse) + FinalPadding;
            horizontalLineTop += Repeat('━', width);
            horizontalLineMiddle += Repeat('━', width);
            horizontalLineBottom += Repeat('━', width);
            variableRow += $"[bold]{PadBoth(item, width)}[/]";
        }

        var resultLine = Repeat('━', label.Length + FinalPadding);

        horizontalLineTop = $"┏{horizontalLineTop}┳{resultLine}┓";
        horizontalLineMiddle = $"┣{horizontalLineMiddle}╋{resultLine}┫";
        horizontalLineBottom = $"┗{horizontalLineBottom}┻{resultLine}┛";
        variableRow = $"┃{variableRow}┃[bold]{PadBoth(label, maxResultLength + FinalPadding)}[/]┃";

        foreach (bool[] row in table)
        {
            var tableRow = "";
            for (int i = 0; i < row.Length - 1; i++)
            {
                var width = Math.Max(ast.Variables[i].Length, maxTrueFalse) + FinalPadding;
                tableRow += $"{PadDisplayAndColour(row[i], width)}";
            }
            tableRows.Add($"┃{tableRow}┃{PadDisplayAndColour(row[^1], label.Length + FinalPadding)}┃");
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


    string TrueStyle => ColourMode switch
    {
        ColourMode.Foreground => "[green]",
        ColourMode.Background => "[on green]",
        _ => "[default]"
    };

    string FalseStyle => ColourMode switch
    {
        ColourMode.Foreground => "[red]",
        ColourMode.Background => "[on red]",
        _ => "[default]"
    };


    string Colour(bool value, string text) => value ? $"{TrueStyle}{text}[/]" : $"{FalseStyle}{text}[/]";

    string PadDisplayAndColour(bool value, int totalLength) => Colour(value, PadBoth(value ? True : False, totalLength));



    static string Repeat(char c, int count) => new string(c, count);

    static string PadBoth(string source, int totalLength, char paddingChar = ' ')
    {
        int spaces = totalLength - source.Length;
        int padLeft = spaces / 2 + source.Length;
        return source.PadLeft(padLeft, paddingChar).PadRight(totalLength, paddingChar);

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