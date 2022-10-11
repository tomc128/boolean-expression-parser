public class AndOperatorNode : OperatorNode
{
    public AndOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate()
    {
        return Left.Evaluate() && Right.Evaluate();
    }
}