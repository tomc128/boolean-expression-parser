namespace BooleanExpressionParser;

internal class Program
{

    static void Main(params string[] expressions)
    {
        if (expressions.Length == 0)
        {
            // expressions = QueryExpressions();
            expressions = new[] { "A.B" };
        }

        foreach (var expression in expressions)
        {
            var trimmed = expression.Replace(" ", "");
            var tokeniser = new Tokeniser(trimmed);
            var infixTokens = tokeniser.Tokenise();

            var parser = new Parser();
            var prefixTokens = parser.ParseTokens(infixTokens);
            var ast = parser.GrowAst(prefixTokens);

            Console.WriteLine($"INFIX: {Formatter.FormatAstAsInfix(ast.Root)}");
            Console.WriteLine($"PREFIX: {Formatter.FormatPrefixTokens(prefixTokens)}");
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
