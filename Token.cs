namespace Ferret
{
    public enum TokenType
    {
        None,
        Text,
        Whitespace,
        Control
    }
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string value = "")
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return $"{{ type: {Type}, value: {Value.ToString().ReplaceWhitespaceNames()} }}";
        }

        public static implicit operator TokenType(Token t) => t.Type;
        public static implicit operator Token(TokenType t) => new(t);
    }
}
