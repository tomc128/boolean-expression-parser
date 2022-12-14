namespace BooleanExpressionParser.Tokens;

public class XorOperatorNode : OperatorNode
{
    public XorOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate(Dictionary<string, bool> variables) => Left.Evaluate(variables) ^ Right!.Evaluate(variables);
}