using System.Net;

namespace Ferret
{

    public class Tokenizer
    {
        private readonly string _data;
        public TokenCollection Tokens { get; set; } = new();

        public Tokenizer(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException($"'{nameof(data)}' cannot be null or empty.", nameof(data));
            }

            _data = data;
        }
        private TokenType GetTokenType(char c)
        {
            bool isLetterOrNumber = char.IsLetterOrDigit(c);
            bool isWhitespace = char.IsWhiteSpace(c);
            bool isControl = !isWhitespace && !isLetterOrNumber;

            if (isLetterOrNumber)
            {
                if (c == '=')
                {
                    return TokenType.Control;
                }
                return TokenType.Text;
            }

            if (isWhitespace)
                return TokenType.Whitespace;

            if (isControl)
                return TokenType.Control;

            throw new Exceptions.CharacterNotHandledException($"Character not handled: {c}");
        }

        void AddToken(int startIndex, int endIndex, TokenType type)
        {
            int start = startIndex;
            int end = int.Max(endIndex, 0);
            int count = end - start;
            if (count != 0)
            {
                Tokens.Append(new Token(type, _data.Substring(start, count)));
            }
        }

        public TokenCollection Parse()
        {
            Tokens = new();

            // start of file should be the same as whitespace
            TokenType previousType = TokenType.None;

            int tokenStartIndex = 0;
            for (int i = 0; i < _data.Length; i++)
            {
                char c = _data[i];

                TokenType currentType = GetTokenType(c);

                if (currentType != previousType || currentType == TokenType.Control)
                {
                    AddToken(tokenStartIndex, i, previousType);

                    previousType = currentType;
                    tokenStartIndex = i;
                }
            }

            AddToken(tokenStartIndex, _data.Length, previousType);

            return Tokens;
        }
    }
}
