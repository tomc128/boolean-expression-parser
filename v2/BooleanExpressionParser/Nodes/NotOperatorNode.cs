public class NotOperatorNode : OperatorNode
{
    public NotOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate()
    {
        return !Left.Evaluate();
    }
}