namespace Ferret
{
    public enum TokenType
    {
        None,
        Text,
        Whitespace,
        Control,
        CachedData
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

        public bool EndsWith(string s) => Value.EndsWith(s);
        public bool EndsWith(char c) => Value.EndsWith(c);


        public override string ToString()
        {
            return $"{{ type: {Type}, value: {Value.ToString().ReplaceWhitespaceNames()} }}";
        }

        public static implicit operator string(Token t) => t.Value;
        public static implicit operator TokenType(Token t) => t.Type;
        public static implicit operator Token(TokenType t) => new(t);
    }
}