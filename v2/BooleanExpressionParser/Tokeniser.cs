namespace BooleanExpressionParser;

class Tokeniser
{
    private readonly string input;

    public Tokeniser(string input)
    {
        this.input = input;
    }

    public IEnumerable<Token> Tokenise()
    {
        var reader = new StringReader(input);
        while (reader.Peek() != -1)
        {
            var token = new Token(reader);
            yield return token;
        }
    }
}