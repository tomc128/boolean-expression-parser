namespace BooleanExpressionParser.Tokens;

public class VariableToken : Token
{
    public string Name { get; }

    public VariableToken(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}