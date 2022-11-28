using System.Text;

namespace BooleanExpressionParser;

internal class Program
{

    static void Main(params string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length == 0)
        {
            args = QueryExpressions();
        }

        var tables = new List<string>();

        foreach (var expression in args)
        {
            var tokeniser = new Tokeniser(expression);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var prefixTokens = parser.ParseTokens(infixTokens);

            var ast = parser.GrowAst(prefixTokens);

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

            tables.Add(Formatter.FormatTruthTable(ast, table, label: expression));
        }

        if (tables.Count > 1)
        {
            Console.WriteLine(Formatter.JoinTruthTables(tables.ToArray()));
        }
        else
        {
            Console.WriteLine(tables[0]);
        }
    }


    static string[] QueryExpressions()
    {
        var expressions = new List<string>();

        Console.WriteLine("Enter expressions, one per line. Press enter on a blank line to continue.");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line)) break;

            expressions.Add(line);
        }

        return expressions.ToArray();
    }
}
