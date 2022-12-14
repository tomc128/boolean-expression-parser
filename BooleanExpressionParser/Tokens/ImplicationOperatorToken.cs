namespace BooleanExpressionParser.Tokens;

public class ImplicationOperatorToken : OperatorToken
{
    public ImplicationOperatorToken() : base(0) { }
    public override string ToString() => "=>";
}