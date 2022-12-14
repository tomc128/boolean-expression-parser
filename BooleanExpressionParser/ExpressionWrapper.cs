namespace BooleanExpressionParser;

public class ExpressionWrapper
{
    public string Expression { get; private set; }
    public string[] VariableOrder { get; private set; }

    public ExpressionWrapper(string input)
    {
        input = input.Trim();
        var parts = input.Split(';');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        if (input.Length == 0) throw new ArgumentException("Expression cannot be empty", nameof(input));
        if (parts[0].Length == 0) throw new ArgumentException("Expression cannot be empty", nameof(input));
    
        Expression = parts[0];

        if (parts.Length > 1 && parts[1].Length != 0)
        {
            VariableOrder = parts[1].Split(',');
            for (int i = 0; i < VariableOrder.Length; i++)
            {
                VariableOrder[i] = VariableOrder[i].Trim();
            }
        }
        else {
            VariableOrder = new string[0];
        }

    }
}