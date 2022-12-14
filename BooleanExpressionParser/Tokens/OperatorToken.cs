public abstract class OperatorToken : Token
{
    protected OperatorToken(int precedence)
    {
        Precedence = precedence;
    }

    public int Precedence { get; protected set; }
}