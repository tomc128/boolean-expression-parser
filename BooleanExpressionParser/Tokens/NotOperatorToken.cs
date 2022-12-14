namespace BooleanExpressionParser.Tokens;

public class NotOperatorToken : OperatorToken
{
    public NotOperatorToken() : base(3) { }
    public override string ToString() => "!";

}