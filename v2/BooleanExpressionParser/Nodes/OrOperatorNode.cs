public class OrOperatorNode : OperatorNode
{
    public OrOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate()
    {
        return Left.Evaluate() || Right.Evaluate();
    }
}