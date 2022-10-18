namespace BooleanExpressionParser;

class Evaluator
{
    public Ast Ast { get; }

    public Evaluator(Ast ast)
    {
        Ast = ast;
    }

    public List<bool[]> RunAll()
    {
        int numCombinations = (int)Math.Pow(2, Ast.Variables.Count);
        var table = new List<bool[]>();

        for (int i = 0; i < numCombinations; i++)
        {
            var binary = Convert.ToString(i, 2).PadLeft(Ast.Variables.Count, '0');
            var row = new bool[Ast.Variables.Count + 1];

            for (int j = 0; j < Ast.Variables.Count; j++)
            {
                row[j] = binary[j] == '1';
            }
        }
    }

    private bool Evaluate(Node root, bool[] values)
    {
        
    }
}