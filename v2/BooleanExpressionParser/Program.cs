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

        foreach (var t in polishTokens)
        {
            Console.WriteLine(t);
        }
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
                    while (stack.Count > 0 && stack.Peek().Type != Token.TokenType.OPEN_PAREN)
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
            output.Enqueue(stack.Pop());
        }

        return output.ToList();
    }
}