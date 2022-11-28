class NotOperatorNode : OperatorNode
{
    public NotOperatorNode(Node left) : base(left, null) { }

    public override bool Evaluate() => !Left.Evaluate();
}