class Token
{
    static Dictionary<char, (TokenType, string)> tokenDict = new()
    {
        { '(', (TokenType.OPEN_PAREN, "(") },
        { ')', (TokenType.CLOSE_PAREN, ")") },
        { '&', (TokenType.BINARY_OPERATOR, "AND") },
        { '.', (TokenType.BINARY_OPERATOR, "AND") },
        { '|', (TokenType.BINARY_OPERATOR, "OR") },
        { '+', (TokenType.BINARY_OPERATOR, "OR") },
        { '!', (TokenType.UNARY_OPERATOR, "NOT") },
        { '¬', (TokenType.UNARY_OPERATOR, "NOT") },
    };


    public enum TokenType
    {
        OPEN_PAREN,
        CLOSE_PAREN,
        UNARY_OPERATOR,
        BINARY_OPERATOR,
        IDENTIFIER,
        EXPR_END,
    }

    public TokenType Type { get; set; }
    public string Value { get; set; }

    public Token(StringReader reader)
    {
        var c = reader.Read();
        if (c == -1)
        {
            Type = TokenType.EXPR_END;
            Value = "";
            return;
        }

        var ch = (char)c;
        if (tokenDict.ContainsKey(ch))
        {
            var (type, value) = tokenDict[ch];
            Type = type;
            Value = value;
        }
        else
        {
            string word = $"{ch}";
            while (reader.Peek() != -1 && !tokenDict.ContainsKey((char)reader.Peek()))
            {
                word += (char)reader.Read();
            }
            Type = TokenType.IDENTIFIER;
            Value = word;
        }
    }

    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}