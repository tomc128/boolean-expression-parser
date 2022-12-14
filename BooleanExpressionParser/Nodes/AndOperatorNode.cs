public class AndOperatorNode : OperatorNode
{
    public AndOperatorNode(Node left, Node right) : base(left, right) { }

    public override bool Evaluate(Dictionary<string, bool> variables) => Left.Evaluate(variables) && Right!.Evaluate(variables);
}