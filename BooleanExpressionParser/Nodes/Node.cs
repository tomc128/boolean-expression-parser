namespace BooleanExpressionParser.Tokens;

public abstract class Node
{
    public abstract bool Evaluate(Dictionary<string, bool> variables);
}