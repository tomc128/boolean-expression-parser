namespace BooleanExpressionParser;

internal class Program
{

    static void Main(params string[] args)
    {
        if (args.Length == 0)
        {
            args = QueryExpressions();
        }

        foreach (var expression in args)
        {
            var tokeniser = new Tokeniser(expression);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var prefixTokens = parser.ParseTokens(infixTokens);

            Console.WriteLine($"Infix: {Formatter.FormatInfixTokens(infixTokens)}");
            Console.WriteLine($"Prefix: {Formatter.FormatPrefixTokens(prefixTokens)}");

            var ast = parser.GrowAst(prefixTokens);

            var evaluator = new Evaluator(ast);
            var table = evaluator.EvaluateAll();

            var tableString = Formatter.FormatTruthTable(ast, table);
            Console.WriteLine(tableString);
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
