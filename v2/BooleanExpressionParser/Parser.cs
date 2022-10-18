namespace BooleanExpressionParser;

class Parser
{
    public Parser()
    {
    }

    public IEnumerable<Token> ParseTokens(IEnumerable<Token> tokens)
    {
        var output = new Queue<Token>();
        var stack = new Stack<Token>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case Token.TokenType.IDENTIFIER:
                    output.Enqueue(token);
                    break;

                case Token.TokenType.BINARY_OPERATOR:
                case Token.TokenType.UNARY_OPERATOR:
                    while ((stack.Count > 0 && stack.Peek().Type != Token.TokenType.OPEN_PAREN) /* && CHECK PRECEDENCE HERE */)
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

        return output;
    }

    public Ast GrowAst(IEnumerable<Token> tokens)
    {
        var stack = new Stack<Node>();
        var variables = new List<string>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case Token.TokenType.IDENTIFIER:
                    stack.Push(new VariableNode(token.Value));
                    if (!variables.Contains(token.Value)) variables.Add(token.Value);
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

        var root = stack.Pop();

        return new Ast(root, variables);
    }
}