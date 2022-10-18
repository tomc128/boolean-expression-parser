public class NotOperatorNode : OperatorNode
{
    public NotOperatorNode(Node left) : base(left, null) { }

    public override bool Evaluate()
    {
        return !Left.Evaluate();
    }
}