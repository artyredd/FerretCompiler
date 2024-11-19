namespace Ferret
{
    public enum TokenType
    {
        None,
        Text = 1 << 0,
        Whitespace = 1 << 2,
        Control = 1 << 3,
        CachedData = 1 << 4
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Id { get; set; } = -1;

        public Token(TokenType type, string value = "")
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return $"{{ type: {Type}, value: {Value.ToString().ReplaceWhitespaceNames()} }}";
        }

        public static implicit operator string(Token t) => t.Value;
        public static implicit operator TokenType(Token t) => t.Type;
        public static implicit operator Token(TokenType t) => new(t);
    }
}