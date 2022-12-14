using System.CommandLine;
using System.Text;
using System;
using Spectre.Console;
using BooleanExpressionParser.CLI.Formatters;
using BooleanExpressionParser.Formatters;

namespace BooleanExpressionParser.CLI;


enum OutputType
{
    Display,
    Basic
}


internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var rootCommand = new RootCommand(description: "A boolean expression parser and evaluator.");

        // Shared arguments
        var expressionsArgument = new Argument<string[]>("expression(s)", description: "The boolean expression(s) to evaluate.");

        // Shared options
        var outputTypeOption = new Option<OutputType>(new[] { "--output", "--output-type", "-o" }, description: "The output type to use.");
        rootCommand.AddOption(outputTypeOption);

        // Table command
        // Takes in a list of expressions, and prints out the truth table for each one.
        var trueOption = new Option<string>(new[] { "--true", "-t" }, () => "1", description: "Character to use for true values in the truth table.");
        var falseOption = new Option<string>(new[] { "--false", "-f" }, () => "0", description: "Character to use for false values in the truth table.");
        var colourModeOption = new Option<ColourMode>(new[] { "--colour-mode", "--color-mode", "-c" }, () => ColourMode.Foreground, description: "Whether to colour the truth table with foreground or background colours (or no colours).");
        var trueColourOption = new Option<string>(new[] { "--true-colour", "--true-color" }, () => "green", description: "The colour to use for true values in the truth table.");
        var falseColourOption = new Option<string>(new[] { "--false-colour", "--false-color" }, () => "red", description: "The colour to use for false values in the truth table.");

        var tableCommand = new Command("table", description: "Prints the truth table of a boolean expression(s). If none are provided, the user will be prompted to enter them.")
        {
            // outputTypeOption,
            trueOption,
            falseOption,
            colourModeOption,
            trueColourOption,
            falseColourOption,
            expressionsArgument
        };

        tableCommand.SetHandler(TableHandler, outputTypeOption, trueOption, falseOption, colourModeOption, trueColourOption, falseColourOption, expressionsArgument);
        rootCommand.AddCommand(tableCommand);

        // Convert command
        // Takes in a list of expressions, and converts them to postfix notation.
        var convertCommand = new Command("convert", description: "Converts a boolean expression(s) to postfix notation. If none are provided, the user will be prompted to enter them.")
        {
            // outputTypeOption,
            expressionsArgument
        };

        convertCommand.SetHandler(ConvertHandler, outputTypeOption, expressionsArgument);
        rootCommand.AddCommand(convertCommand);

        rootCommand.Invoke(args);
    }


    private static void TableHandler(OutputType outputType, string @true, string @false, ColourMode colourMode, string trueColour, string falseColour, string[] args)
    {
        var expressions = ParseExpressions(args);

        var tables = new List<string>();

        var formatter = GetFormatter(outputType);

        if (formatter is DisplayFormatter displayFormatter)
        {
            displayFormatter.True = @true;
            displayFormatter.False = @false;
            displayFormatter.ColourMode = colourMode;
            displayFormatter.TrueColour = trueColour;
            displayFormatter.FalseColour = falseColour;
        }

        foreach (var expression in expressions)
        {
            var tokeniser = new Tokeniser(expression.Expression);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var postfixTokens = parser.InfixToPostfix(infixTokens);

            var ast = parser.GrowAst(postfixTokens, expression.VariableOrder);

            int numCombinations = (int)Math.Pow(2, ast.Variables.Count);
            var table = new List<bool[]>();
            for (int i = 0; i < numCombinations; i++)
            {
                var binary = Convert.ToString(i, 2).PadLeft(ast.Variables.Count, '0');
                var values = binary.Select(c => c == '1').ToArray();

                var variables = ast.Variables.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                var result = ast.Root.Evaluate(variables);
                table.Add(values.Append(result).ToArray());
            }

            tables.Add(formatter.FormatTruthTable(ast, table, label: expression.Expression));
        }

        if (tables.Count > 1)
        {
            var output = formatter.JoinTruthTables(tables.ToArray());
            AnsiConsole.Markup(output);

        }
        else
        {
            AnsiConsole.Markup(tables[0]);
        }
    }

    private static void ConvertHandler(OutputType outputType, string[] args)
    {
        var expressions = ParseExpressions(args);
        var formatter = GetFormatter(outputType);

        foreach (var expression in expressions)
        {
            var tokeniser = new Tokeniser(expression.Expression);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var postfixTokens = parser.InfixToPostfix(infixTokens);

            AnsiConsole.MarkupLine($"{expression.Expression} -> [bold]{formatter.FormatTokens(postfixTokens)}[/]");
        }
    }


    static List<ExpressionWrapper> ParseExpressions(string[] args) => args.Length == 0 ? QueryExpressions() : args.Select(arg => new ExpressionWrapper(arg)).ToList();

    static IFormatter GetFormatter(OutputType outputType) => outputType switch
    {
        OutputType.Display => new DisplayFormatter(),
        OutputType.Basic => new BasicFormatter(),
        _ => throw new ArgumentOutOfRangeException(nameof(outputType), outputType, null)
    };


    static List<ExpressionWrapper> QueryExpressions()
    {
        var expressions = new List<ExpressionWrapper>();

        Console.WriteLine("Enter expressions, one per line. Press enter on a blank line to continue.");
        Console.WriteLine("Expressions should be of the form: '<expression>(;<variable-order>)', where variable-order is optional.");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line)) break;

            var expression = new ExpressionWrapper(line);
            expressions.Add(expression);
        }

        return expressions;
    }


}
