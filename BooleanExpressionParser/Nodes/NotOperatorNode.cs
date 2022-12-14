public class NotOperatorNode : OperatorNode
{
    public NotOperatorNode(Node left) : base(left, null) { }

    public override bool Evaluate(Dictionary<string, bool> variables) => !Left.Evaluate(variables);
}