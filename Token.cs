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
        public Dictionary<string, object?> Metadata { get; private set; } = new();

        public void SetMetadata<T>(string key, T? value)
        {
            if (Metadata.ContainsKey(key) is false)
            {
                Metadata.Add(key,(object)value!);    
            }
        }

        public bool TryGetMetadata<T>(string key, out T value)
        {
            if (Metadata.TryGetValue(key, out object? maybeValue))
            {
                if (maybeValue != null)
                {
                    value = (T)maybeValue;
                    return true;
                }

            }

            value = default(T)!;

            return false;
        }

        public Token(TokenType type, string value = "")
        {
            Value = value;
            Type = type;
        }

        public bool EndsWith(string s) => Value.EndsWith(s);
        public bool EndsWith(char c) => Value.EndsWith(c);
        public bool StartsWith(string s) => Value.StartsWith(s);
        public bool StartsWith(char c) => Value.StartsWith(c);
        public bool Contains(string s) => Value.Contains(s);
        public bool Contains(char c) => Value.Contains(c);

        public Token[] Split(string s)
        {
            string[] splitStrings = Value.SplitInclusive(s);
            var result = new List<Token>();

            foreach (var str in splitStrings)
            {
                result.Add(new Token(Type, str));
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            return $"{{ type: {Type}, value: {Value.ToString().ReplaceWhitespaceNames()} }}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Token token &&
                   Type == token.Type &&
                   Value == token.Value;
        }

        public static implicit operator string(Token t) => t.Value;
        public static implicit operator TokenType(Token t) => t.Type;
        public static implicit operator Token(TokenType t) => new(t);
    }
}