using System.CommandLine;
using System.Text;
using Spectre.Console;

namespace BooleanExpressionParser;

internal class Program
{

    private static void Main(string[] args)
    {
        var rootCommand = new RootCommand();

        var trueOption = new Option<string>(new[] { "--true", "-t" }, () => "1", description: "Character to use for true values in the truth table.");
        var falseOption = new Option<string>(new[] { "--false", "-f" }, () => "0", description: "Character to use for false values in the truth table.");
        var expressionsArgument = new Argument<string[]>("expression(s)", description: "The boolean expression(s) to evaluate.");

        var tableCommand = new Command("table", description: "Prints the truth table of a boolean expression(s).")
        {
            trueOption,
            falseOption,
            expressionsArgument
        };

        tableCommand.SetHandler(Run, trueOption, falseOption, expressionsArgument);

        rootCommand.AddCommand(tableCommand);

        rootCommand.Invoke(args);


    }


    private static void Run(string @true, string @false, string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        List<ExpressionWrapper> expressions;

        if (args.Length == 0)
        {
            expressions = QueryExpressions();
        }
        else
        {
            expressions = new List<ExpressionWrapper>();
            foreach (string arg in args)
            {
                expressions.Add(new ExpressionWrapper(arg));
            }
        }

        var tables = new List<string>();
        var formatter = new Formatter
        {
            True = @true[0],
            False = @false[0]
        };

        foreach (var expression in expressions)
        {
            var tokeniser = new Tokeniser(expression.Expression);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var prefixTokens = parser.ParseTokens(infixTokens);

            var ast = parser.GrowAst(prefixTokens, expression.VariableOrder);

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


    static List<ExpressionWrapper> QueryExpressions()
    {
        var expressions = new List<ExpressionWrapper>();

        Console.WriteLine("Enter expressions, one per line. Press enter on a blank line to continue.");

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
