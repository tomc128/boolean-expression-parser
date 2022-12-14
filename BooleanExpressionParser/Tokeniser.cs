using System.Text.RegularExpressions;

namespace BooleanExpressionParser;

class Tokeniser
{
    private readonly Regex regex = new Regex(@"([([{]|[)\]}]|[\w]+|[.&+!¬|]|=>)\s*");
    private readonly string input;

    public Tokeniser(string input)
    {
        this.input = input;
    }

    public IEnumerable<Token> Tokenise()
    {
        int i = 0;

        while (i < input.Length)
        {
            var match = regex.Match(input, i);
            if (!match.Success) throw new Exception("Invalid token (no match found) at position " + i);
            if (match.Index != i) throw new Exception($"Invalid token (match found at wrong position) at position {i}, match '{match.Value}' found at position {match.Index}");

            string token = match.Groups[1].Value;
            i += match.Length;

            yield return token.ToLower() switch
            {
                "(" or "[" or "{" => new OpenParenToken(),
                ")" or "]" or "}"  => new CloseParenToken(),
                "and" or "." or "&" => new AndOperatorToken(),
                "or" or "+" or "|" => new OrOperatorToken(),
                "not" or "!" or "¬" => new NotOperatorToken(),
                "xor" => new XorOperatorToken(),
                "nand" => new NandOperatorToken(),
                "nor" => new NorOperatorToken(),
                "xnor" => new XnorOperatorToken(),
                "implies" or "=>" => new ImplicationOperatorToken(),
                _ => new VariableToken(token)
            };
        }
    }
}