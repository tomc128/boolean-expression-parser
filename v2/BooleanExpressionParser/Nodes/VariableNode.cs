class VariableNode : Node
{
    public VariableNode(String name)
    {
        Name = name;
    }

    public String Name { get; set; }
    public bool Value { get; set; }
}