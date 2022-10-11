public abstract class OperatorNode : Node
{

    public OperatorNode(Node left, Node right)
    {
        Left = left;
        Right = right;
    }

    public Node Left { get; private set; }
    public Node Right { get; private set; }
}