using System.Text;

internal class Program
{

    public enum Command
    {
        Evaluate,
        Compare
    }

    /// <summary>
    /// Tom's boolean expression parser and evaluator.
    /// </summary>
    /// <param name="subCommand">The command to perform</param>
    /// <param name="expression">The expression to evaluate</param>
    /// <param name="secondExpression">The expression to compare to (if using Compare sub-command)</param>

    static void Main(Command? subCommand, string? expression, string? secondExpression)
    {
        Console.WriteLine("Tom's boolean expression parser and evaluator.");
        Console.WriteLine($"Command: {subCommand}");
        Console.WriteLine($"Expression: {expression ?? "Not specified"}");
        Console.WriteLine($"Second Expression: {secondExpression ?? "Not specified"}");

        if (subCommand is null)
        {
            Console.WriteLine("Enter a command (Evaluate or Compare):");
            Console.WriteLine("> ");
            var command = Console.ReadLine();
            subCommand = command.ToLower() switch {
                "evaluate" => Command.Evaluate,
                "e" => Command.Evaluate,
                "compare" => Command.Compare,
                "c" => Command.Compare,
                _ => throw new ArgumentException($"Invalid command: {command}")
            };
            
        }

        switch (subCommand)
        {
            case Command.Evaluate:
                if (expression is null)
                {
                    Console.WriteLine("Enter expression:");
                    Console.Write("> ");
                    expression = Console.ReadLine();
                }

                var parsed = RunExpression(expression);

                Console.WriteLine($"Truth table for '{expression}':");
                Console.WriteLine(parsed.TableString);

                break;

            case Command.Compare:
                if (expression is null)
                {
                    Console.WriteLine("Enter expression:");
                    Console.Write("> ");
                    expression = Console.ReadLine();
                }
                if (secondExpression is null)
                {
                    Console.WriteLine("Enter second expression:");
                    Console.Write("> ");
                    secondExpression = Console.ReadLine();
                }

                var parsed1 = RunExpression(expression);
                var parsed2 = RunExpression(secondExpression);

                Console.WriteLine($"Truth table for '{expression}':");
                Console.WriteLine(parsed1.TableString);

                Console.WriteLine($"Truth table for '{secondExpression}':");
                Console.WriteLine(parsed2.TableString);

                bool equal = parsed1.ASTString == parsed2.ASTString;
                Console.WriteLine($"The two expressions are {(equal ? "equal" : "not equal")}.");

                break;


        }
    }

    static ParsedExpression RunExpression(string expression)
    {
        var strippedExpression = expression.Replace(" ", "");

        var tokens = new List<Token>();
        var reader = new StringReader(strippedExpression);

        Console.WriteLine("Tokenising...");
        Token token;
        do
        {
            token = new Token(reader);
            tokens.Add(token);
        } while (token.Type != Token.TokenType.EXPR_END);
        tokens.ForEach(x => Console.Write($"{x.Value} "));
        Console.WriteLine();

        Console.WriteLine("Converting to prefix (PN)...");
        var polishTokens = ToPolish(tokens);
        polishTokens.ForEach(x => Console.Write($"{x.Value} "));
        Console.WriteLine();

        Console.WriteLine("Building AST...");
        var root = BuildAST(polishTokens);

        Console.WriteLine("Stringifying AST...");
        var text = StringifyAST(root);
        Console.WriteLine($"Output expression: {text}");

        Console.WriteLine("Retrieving variables...");
        var variables = GetVariables(root);
        Console.WriteLine($"Found {variables.Count} variables: {string.Join(", ", variables)}");

        Console.WriteLine();
        Console.WriteLine($"Input: {expression}");
        Console.WriteLine($"Parsed: {text}");
        Console.WriteLine();

        int numCombinations = (int)Math.Pow(2, variables.Count);

        var tableRows = new List<bool[]>();

        Console.WriteLine($"Evaluating {numCombinations} combinations...");
        for (int i = 0; i < numCombinations; i++)
        {
            var binary = Convert.ToString(i, 2).PadLeft(variables.Count, '0');
            var values = new Dictionary<String, bool>();

            for (int j = 0; j < variables.Count; j++)
            {
                values.Add(variables[j], binary[j] == '1');
            }

            var result = Evaluate(root, values);
            Console.WriteLine($"Combination {i} ({binary}) = {(result ? "1" : "0")}");

            tableRows.Add(values.Values.Append(result).ToArray());
        }

        Console.WriteLine();
        Console.WriteLine($"Truth table for '{expression}':");

        var table = FormatTruthTable(variables, tableRows);

        return new ParsedExpression
        {
            Expression = expression,
            StrippedExpression = strippedExpression,
            Tokens = tokens,
            PolishTokens = polishTokens,
            RootNode = root,
            ASTString = text,
            Table = tableRows,
            TableString = table
        };

    }

    static List<Token> ToPolish(List<Token> normalTokens)
    {
        var output = new Queue<Token>();
        var stack = new Stack<Token>();

        foreach (var token in normalTokens)
        {
            switch (token.Type)
            {
                case Token.TokenType.IDENTIFIER:
                    output.Enqueue(token);
                    break;
                case Token.TokenType.BINARY_OPERATOR:
                case Token.TokenType.UNARY_OPERATOR:
                    while ((stack.Count > 0 && stack.Peek().Type != Token.TokenType.OPEN_PAREN)
                            /* && CHECK PRECEDENCE HERE */)
                    {
                        output.Enqueue(stack.Pop());
                    }
                    stack.Push(token);
                    break;
                case Token.TokenType.OPEN_PAREN:
                    stack.Push(token);
                    break;
                case Token.TokenType.CLOSE_PAREN:
                    while (stack.Count > 0 && stack.Peek().Type != Token.TokenType.OPEN_PAREN)
                    {
                        output.Enqueue(stack.Pop());
                    }
                    if (stack.Peek().Type == Token.TokenType.OPEN_PAREN)
                    {
                        stack.Pop();
                        if (stack.Count > 0 && (stack.Peek().Type == Token.TokenType.UNARY_OPERATOR || stack.Peek().Type == Token.TokenType.BINARY_OPERATOR))
                        {
                            output.Enqueue(stack.Pop());
                        }
                    }
                    break;
            }
        }

        while (stack.Count > 0)
        {
            if (stack.Peek().Type == Token.TokenType.OPEN_PAREN)
            {
                throw new Exception("OPEN_PAREN on operator stack.");
            }
            output.Enqueue(stack.Pop());
        }

        return output.ToList();
    }

    static Node BuildAST(List<Token> tokens)
    {
        var stack = new Stack<Node>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case Token.TokenType.IDENTIFIER:
                    stack.Push(new VariableNode(token.Value));
                    break;

                case Token.TokenType.BINARY_OPERATOR:
                    if (stack.Count < 2) throw new Exception($"2 parameters needed for operator ${token.Type}");

                    if (token.Value == "AND")
                        stack.Push(new AndOperatorNode(stack.Pop(), stack.Pop()));
                    else
                        stack.Push(new OrOperatorNode(stack.Pop(), stack.Pop()));
                    break;

                case Token.TokenType.UNARY_OPERATOR:
                    if (stack.Count < 1) throw new Exception($"1 parameter needed for operator ${token.Type}");

                    if (token.Value == "NOT")
                        stack.Push(new NotOperatorNode(stack.Pop()));
                    break;
            }
        }

        if (stack.Count != 1) throw new Exception("Expression invalid - stack not empty");

        return stack.Pop();
    }

    static String StringifyAST(Node root)
    {
        var builder = new StringBuilder();

        Visit(root);

        void Visit(Node node)
        {
            switch (node)
            {
                case AndOperatorNode op:
                    VisitAndWrap(op.Left);
                    builder.Append(" AND ");
                    VisitAndWrap(op.Right!);
                    break;

                case OrOperatorNode op:
                    VisitAndWrap(op.Left);
                    builder.Append(" OR ");
                    VisitAndWrap(op.Right!);
                    break;

                case NotOperatorNode op:
                    builder.Append($"NOT (");
                    Visit(op.Left);
                    builder.Append(")");
                    break;

                case VariableNode var:
                    builder.Append(var.Name);
                    break;

            }
        }

        void VisitAndWrap(Node node)
        {
            if (node is AndOperatorNode || node is OrOperatorNode) builder.Append("(");
            Visit(node);
            if (node is AndOperatorNode || node is OrOperatorNode) builder.Append(")");
        }

        return builder.ToString();
    }

    static List<String> GetVariables(Node root)
    {
        var variables = new List<String>();

        void Visit(Node node)
        {
            switch (node)
            {
                case VariableNode var:
                    variables.Add(var.Name);
                    break;
                case AndOperatorNode op:
                    Visit(op.Left);
                    Visit(op.Right!);
                    break;
                case OrOperatorNode op:
                    Visit(op.Left);
                    Visit(op.Right!);
                    break;
                case NotOperatorNode op:
                    Visit(op.Left);
                    break;
            }
        }

        Visit(root);

        variables.Sort();
        return variables;
    }

    static bool Evaluate(Node root, Dictionary<String, bool> variables)
    {
        void Visit(Node node)
        {
            switch (node)
            {
                case VariableNode var:
                    var.Value = variables[var.Name];
                    break;
                case AndOperatorNode op:
                    Visit(op.Left);
                    Visit(op.Right!);
                    break;
                case OrOperatorNode op:
                    Visit(op.Left);
                    Visit(op.Right!);
                    break;
                case NotOperatorNode op:
                    Visit(op.Left);
                    break;
            }
        }

        Visit(root);

        var result = root.Evaluate();

        return result;
    }


    static String FormatTruthTable(List<String> variables, List<bool[]> table)
    {
        String Repeat(char c, int count) => new String(c, count);

        var builder = new StringBuilder();

        builder.AppendLine($"┏{Repeat('━', variables.Count * 3 + 2)}┳{Repeat('━', 8)}┓");
        builder.AppendLine($"┃  {String.Join("  ", variables)}  ┃ Result ┃");
        builder.AppendLine($"┣{Repeat('━', variables.Count * 3 + 2)}╋{Repeat('━', 8)}┫");

        foreach (var row in table)
        {
            builder.AppendLine($"┃  {String.Join("  ", row[0..^1].Select(b => b ? "1" : "0"))}  ┃   {(row[^1] ? "1" : "0")}    ┃");
        }

        builder.AppendLine($"┗{Repeat('━', variables.Count * 3 + 2)}┻{Repeat('━', 8)}┛");

        return builder.ToString();
    }
}