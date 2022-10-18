using System.Text;

namespace BooleanExpressionParser;

class Formatter
{
    public static string FormatAstAsInfix(Node root)
    {
        var sb = new StringBuilder();

        void Visit(Node node)
        {
            switch (node)
            {
                case AndOperatorNode op:
                    VisitAndWrap(op.Left);
                    sb.Append(" AND ");
                    VisitAndWrap(op.Right!);
                    break;

                case OrOperatorNode op:
                    VisitAndWrap(op.Left);
                    sb.Append(" OR ");
                    VisitAndWrap(op.Right!);
                    break;

                case NotOperatorNode op:
                    sb.Append($"NOT (");
                    Visit(op.Left);
                    sb.Append(")");
                    break;

                case VariableNode var:
                    sb.Append(var.Name);
                    break;

            }
        }

        void VisitAndWrap(Node node)
        {
            if (node is AndOperatorNode || node is OrOperatorNode) sb.Append("(");
            Visit(node);
            if (node is AndOperatorNode || node is OrOperatorNode) sb.Append(")");
        }

        Visit(root);

        return sb.ToString();
    }

    public static string FormatPrefixTokens(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            sb.Append(token.Value);
            sb.Append(" ");
        }

        return sb.ToString();
    }

    public static string FormatTruthTable(Ast ast, List<bool[]> table)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"┏{Repeat('━', ast.Variables.Count * 3 + 2)}┳{Repeat('━', 8)}┓");
        sb.AppendLine($"┃  {String.Join("  ", ast.Variables)}  ┃ Result ┃");
        sb.AppendLine($"┣{Repeat('━', ast.Variables.Count * 3 + 2)}╋{Repeat('━', 8)}┫");

        foreach (var row in table)
        {
            sb.AppendLine($"┃  {String.Join("  ", row[0..^1].Select(b => b ? "1" : "0"))}  ┃   {(row[^1] ? "1" : "0")}    ┃");
        }

        sb.AppendLine($"┗{Repeat('━', ast.Variables.Count * 3 + 2)}┻{Repeat('━', 8)}┛");

        return sb.ToString();
    }

    static string Repeat(char c, int count) => new string(c, count);
}