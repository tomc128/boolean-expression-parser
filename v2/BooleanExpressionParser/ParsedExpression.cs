public record ParsedExpression
{   
    public string Expression { get; set; }
    public string StrippedExpression { get; set; }

    public List<Token> Tokens { get; set; }
    public List<Token> PolishTokens { get; set; }

    public Node RootNode { get; set; }
    public String ASTString { get; set; }

    public List<bool[]> Table { get; set; }
    public String TableString { get; set; }
    
}