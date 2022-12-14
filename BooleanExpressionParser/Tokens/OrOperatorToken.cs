namespace BooleanExpressionParser.Tokens;

public class OrOperatorToken : OperatorToken
{
    public OrOperatorToken() : base(1) { }
    public override string ToString() => "+";
}