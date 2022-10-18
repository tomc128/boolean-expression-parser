using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        string expression = @"(!(A & B) | C) & D";

        expression = expression.Replace(" ", "");

        var tokens = new List<Token>();
        var reader = new StringReader(expression);

        Token token;
        do
        {
            token = new Token(reader);
            tokens.Add(token);
        } while (token.Type != Token.TokenType.EXPR_END);

        var polishTokens = ToPolish(tokens);

        polishTokens.ForEach(x => System.Console.WriteLine($"{x.Type} : {x.Value}"));

        var root = BuildAST(polishTokens);

        var text = StringifyAST(root);


        System.Console.WriteLine("======");

        System.Console.WriteLine($"Input expression: {expression}");

        System.Console.WriteLine($"Output expression: {text}");
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

        System.Console.WriteLine($"{stack.Count} tokens remaining in stack!");

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
                    builder.Append($"(NOT ");
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

}