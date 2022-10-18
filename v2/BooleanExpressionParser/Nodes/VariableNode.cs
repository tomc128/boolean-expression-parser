public class VariableNode : Node
{
    public VariableNode(String name)
    {
        Name = name;
    }

    public String Name {get;set;}

    public override bool Evaluate()
    {
        throw new NotImplementedException();
    }
}