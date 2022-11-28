namespace BooleanExpressionParser;

class Evaluator
{
    public Ast Ast { get; }

    public Evaluator(Ast ast)
    {
        Ast = ast;
    }

    public List<bool[]> EvaluateAll()
    {
        int numCombinations = (int)Math.Pow(2, Ast.Variables.Count);
        var table = new List<bool[]>();

        for (int i = 0; i < numCombinations; i++)
        {
            var binary = Convert.ToString(i, 2).PadLeft(Ast.Variables.Count, '0');
            var values = binary.Select(c => c == '1').ToArray();

            var result = EvaluateNode(Ast.Root, values);
            table.Add(values.Append(result).ToArray());
        }

        return table;
    }

    public bool Evaluate(bool[] values)
    {
        return EvaluateNode(Ast.Root, values);
    }

    private bool EvaluateNode(Node node, bool[] values)
    {
        if (node is VariableNode var)
        {
            var index = Ast.Variables.IndexOf(var.Name);
            return values[index];
        }
        else if (node is OperatorNode op)
        {
            var left = EvaluateNode(op.Left, values);

            if (node is NotOperatorNode) return !left;

            var right = EvaluateNode(op.Right!, values);

            if (node is AndOperatorNode) return left && right;
            if (node is OrOperatorNode) return left || right;
            if (node is XorOperatorNode) return left ^ right;
            if (node is NandOperatorNode) return !(left && right);
            if (node is NorOperatorNode) return !(left || right);
            if (node is XnorOperatorNode) return !(left ^ right);

        }
        else
        {
            throw new Exception($"Unknown node type: {node.GetType()}");
        }

        throw new Exception("This should never happen");
    }
}