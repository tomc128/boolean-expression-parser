namespace BooleanExpressionParser.Tokens;

public class AndOperatorToken : OperatorToken
{
    public AndOperatorToken() : base(2) { }

    public override string ToString() => ".";
}