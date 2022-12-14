namespace BooleanExpressionParser.Tokens;

public class NorOperatorNode : OperatorNode
{
    public NorOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate(Dictionary<string, bool> variables) => !(Left.Evaluate(variables) || Right!.Evaluate(variables));
}