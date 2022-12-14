namespace BooleanExpressionParser.Tokens;

public class VariableNode : Node
{
    public VariableNode(String name)
    {
        Name = name;
    }

    public String Name { get; set; }
    // public bool Value { get; set; }

    public override bool Evaluate(Dictionary<string, bool> variables) => variables[Name];
}