namespace BooleanExpressionParser;

class Parser
{
    public Parser()
    {
    }

    /// <summary>
    /// Converts a list of tokens in infix notation to a list of tokens in prefix notation.
    /// </summary>
    /// <param name="tokens">The tokens to parse.</param>
    /// <returns>The root node of the expression tree.</returns>
    public IEnumerable<Token> ParseTokens(IEnumerable<Token> tokens)
    {
        var output = new Queue<Token>();
        var stack = new Stack<Token>();

        foreach (var token in tokens)
        {
            switch (token)
            {
                case VariableToken:
                    output.Enqueue(token);
                    break;

                case OperatorToken op:
                    while ((stack.Count > 0 && stack.Peek() is OperatorToken && stack.Peek() is not OpenParenToken) &&
                           ((stack.Peek() as OperatorToken)!.Precedence >= op!.Precedence))
                    {
                        output.Enqueue(stack.Pop());
                    }
                    stack.Push(token);
                    break;

                case OpenParenToken:
                    stack.Push(token);
                    break;

                case CloseParenToken:
                    while (stack.Count > 0 && stack.Peek() is not OpenParenToken)
                    {
                        output.Enqueue(stack.Pop());
                    }

                    if (stack.Peek() is OpenParenToken)
                    {
                        stack.Pop();
                        // NOTE: Changed this check from several specific checks to 'is OperatorToken' 
                        if (stack.Count > 0 && (stack.Peek() is OperatorToken))
                        {
                            output.Enqueue(stack.Pop());
                        }
                    }
                    break;
            }
        }

        while (stack.Count > 0)
        {
            if (stack.Peek() is OpenParenToken)
            {
                throw new Exception("OPEN_PAREN on operator stack.");
            }
            output.Enqueue(stack.Pop());
        }

        return output;
    }

    public Ast GrowAst(IEnumerable<Token> tokens, string[] variableOrder)
    {
        var stack = new Stack<Node>();
        var variables = new List<string>();

        foreach (var token in tokens)
        {
            switch (token)
            {
                case VariableToken var:
                    stack.Push(new VariableNode(var.Name));
                    if (!variables.Contains(var.Name)) variables.Add(var.Name);
                    break;

                case NotOperatorToken:
                    if (stack.Count < 1) throw new Exception($"1 parameter needed for operator ${token}");
                    stack.Push(new NotOperatorNode(stack.Pop()));
                    break;

                // All other operators
                case OperatorToken:
                    if (stack.Count < 2) throw new Exception($"2 parameters needed for operator ${token}");

                    OperatorNode node = token switch
                    {
                        AndOperatorToken => new AndOperatorNode(stack.Pop(), stack.Pop()),
                        OrOperatorToken => new OrOperatorNode(stack.Pop(), stack.Pop()),
                        NorOperatorToken => new NorOperatorNode(stack.Pop(), stack.Pop()),
                        NandOperatorToken => new NandOperatorNode(stack.Pop(), stack.Pop()),
                        XorOperatorToken => new XorOperatorNode(stack.Pop(), stack.Pop()),
                        XnorOperatorToken => new XnorOperatorNode(stack.Pop(), stack.Pop()),
                        ImplicationOperatorToken => new ImplicationOperatorNode(stack.Pop(), stack.Pop()),
                        _ => throw new Exception($"Unknown operator ${token}")
                    };

                    stack.Push(node);
                    break;
            }
        }

        if (stack.Count != 1)
        {
            throw new Exception($"Expression invalid - stack not empty. Stack: {String.Join(", ", stack)}");
        }

        var root = stack.Pop();

        variables = variables.OrderBy(v => Array.IndexOf(variableOrder, v)).ToList();

        return new Ast(root, variables);
    }
}