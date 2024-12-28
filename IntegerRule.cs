namespace Ferret
{
    public class IntegerRule : CachedRuleBase, ILexerRule
    {
        private readonly TokenPattern _pattern = new TokenPattern()
            .When(TokenType.Text)
            .When(x=>IsInteger(x))
            .IsNotFollowedBy('.')
            .IsNotPreceededBy('.');

        // 0xFFE12A
        static bool IsHexNumber(string s)
        {
            // |-|
            // 0xF
            if (s.Length < 3)
            {
                return false;
            }

            // ||
            // 0xF
            if (s[0] != '0' || s[1] != 'x')
            {
                return false;
            }

            //   |----|
            // 0xFFE12A
            for (int i = 2; i < s.Length; i++)
            { 
                char c = s[i];

                bool isCapitalHexChar = c >= 'A' && c <= 'F';
                bool isLowercaseHexChar = c >= 'a' && c <= 'f';

                bool isHexLetter = isCapitalHexChar || isLowercaseHexChar; 

                if (!isHexLetter && !char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsBinaryNumber(string s)
        {
            // 0b0
            if (s.Length < 3)
            {
                return false;
            }

            //  |
            // 0b0
            if (s[1] != 'b')
            {
                return false;
            }

            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];
                if (c != '0' && c != '1')
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsUnsignedNumber(string s)
        {
            // 0U
            if (s.Length < 2)
            {
                return false;
            }

            var endings = new string[] { 
                "U",
                "UL",
                "ULL",
                "LLU",
                "LU",
                "L",
                "LL"
            };

            string foundEnding = string.Empty;
            foreach (var ending in endings)
            {
                if (s.EndsWith(ending))
                {
                    foundEnding = ending;
                }
            }
            
            if (foundEnding.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < s.Length - foundEnding.Length; i++)
            {
                char c = s[i];
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsPlainNumber(string s)
        {
            foreach (var c in s)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsInteger(string s)
        {
            bool isInteger = IsPlainNumber(s);

            isInteger = isInteger || IsUnsignedNumber(s);
            isInteger = isInteger || IsBinaryNumber(s);
            isInteger = isInteger || IsHexNumber(s);

            return isInteger;
        }

        public void ApplyRule(TokenStream stream)
        {
            // cache macros so we can re-insert them later
            var indices = ValidIndexes(stream);

            TokenCollection cachedToken = new();
            while (stream.GetToken(out var token))
            {
                int index = stream.Position - 1;

                if (indices.Contains(index))
                {
                    cachedToken.Append(token);
                    var marker = CacheToken(cachedToken);
                    marker.SetMetadata("type","integer");
                    stream.PutToken(marker);
                    continue;
                }

                stream.PutToken(token);
            }
        }

        List<int> ValidIndexes(TokenStream stream)
        {
            return stream.Tokens.IndexesOf(_pattern);
        }

        public bool RuleApplies(TokenStream stream)
        {
            return ValidIndexes(stream).Count != 0;
        }
    }
}
