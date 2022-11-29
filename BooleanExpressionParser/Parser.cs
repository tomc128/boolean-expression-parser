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
            switch (token)
            {
                case VariableToken:
                    output.Enqueue(token);
                    break;

                case AndOperatorToken:
                case OrOperatorToken:
                case NotOperatorToken:
                case XorOperatorToken:
                case NandOperatorToken:
                case NorOperatorToken:
                case XnorOperatorToken:
                case ImplicationOperatorToken:
                    while ((stack.Count > 0 && stack.Peek() is OperatorToken && stack.Peek() is not OpenParenToken) && ((stack.Peek() as OperatorToken)!.Precedence >= (token as OperatorToken)!.Precedence))
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

    public Ast GrowAst(IEnumerable<Token> tokens)
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

                case AndOperatorToken:
                case OrOperatorToken:
                case XorOperatorToken:
                case NandOperatorToken:
                case NorOperatorToken:
                case XnorOperatorToken:
                case ImplicationOperatorToken:
                    if (stack.Count < 2) throw new Exception($"2 parameters needed for operator ${token}");

                    if (token is AndOperatorToken)
                        stack.Push(new AndOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is OrOperatorToken)
                        stack.Push(new OrOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is NorOperatorToken)
                        stack.Push(new NorOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is NandOperatorToken)
                        stack.Push(new NandOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is XorOperatorToken)
                        stack.Push(new XorOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is XnorOperatorToken)
                        stack.Push(new XnorOperatorNode(stack.Pop(), stack.Pop()));
                    else if (token is ImplicationOperatorToken)
                        stack.Push(new ImplicationOperatorNode(stack.Pop(), stack.Pop()));
                    break;

                case NotOperatorToken:
                    if (stack.Count < 1) throw new Exception($"1 parameter needed for operator ${token}");
                    stack.Push(new NotOperatorNode(stack.Pop()));
                    break;
            }
        }

        if (stack.Count != 1)
        {
            throw new Exception($"Expression invalid - stack not empty. Stack: {String.Join(", ", stack)}");
        }

        var root = stack.Pop();

        return new Ast(root, variables);
    }
}