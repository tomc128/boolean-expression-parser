using System.Text.RegularExpressions;

namespace BooleanExpressionParser;

class Tokeniser
{
    private readonly Regex regex = new Regex(@"(\(|\)|\w+|[.&+!¬])\s*");
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
            if (match.Index != i) throw new Exception("Invalid token (match found at wrong position) at position " + i);

            string token = match.Groups[1].Value;
            i += match.Length;

            yield return token switch
            {
                "(" => new OpenParenToken(),
                ")" => new CloseParenToken(),
                "AND" or "." or "&" => new AndOperatorToken(),
                "OR" or "+" or "|" => new OrOperatorToken(),
                "NOT" or "!" or "¬" => new NotOperatorToken(),
                _ => new VariableToken(token)
            };
        }
    }
}