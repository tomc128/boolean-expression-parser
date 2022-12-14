using BooleanExpressionParser.Tokens;

namespace BooleanExpressionParser;

public class Ast
{
    public Node Root { get; }

    public List<string> Variables { get; }

    public Ast(Node root, List<string> variables)
    {
        Root = root;
        Variables = variables;
    }
}