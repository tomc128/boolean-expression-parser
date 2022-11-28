class NandOperatorNode : OperatorNode
{
    public NandOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate() => !(Left.Evaluate() && Right!.Evaluate());
}