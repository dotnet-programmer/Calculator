namespace Calculator.WpfApp.Models.Calculation;

internal class Token
{
	public Token(TokenType tokenType, string value)
	{
		TokenType = tokenType;
		Value = value;
	}

	public Token(TokenType tokenType, char value)
	{
		TokenType = tokenType;
		Value = value.ToString();
	}

	public TokenType TokenType { get; private set; }
	public string Value { get; private set; }
}