using System.Runtime.CompilerServices;

namespace Ferret
{
    public class MacroRule : ILexerRule
    {
        private readonly TokenPattern _pattern = new TokenPattern()
            .When(TokenType.Control)
            .When("#")
            .IsPreceededBy(TokenType.Whitespace)
            .IsFollowedBy(TokenType.Text);

        private readonly TokenPattern _endOfMacroPattern = new TokenPattern()
            .When(TokenType.Whitespace)
            .When(x => x.EndsWith('\n') || x.EndsWith("\r\n"))
            .IsPreceededBy(x => !x.EndsWith("\\"));

        public static List<TokenCollection> CachedTokens { get; set; } = new();

        public static Token CacheToken(TokenCollection token)
        {
            Token cachedToken = new(TokenType.CachedData, string.Empty);
            cachedToken.Id = CachedTokens.Count;
            CachedTokens.Add(token);
            return cachedToken;
        }

        public void ApplyRule(TokenStream stream)
        {
            // cache macros so we can re-insert them later
            var indices = ValidIndexes(stream);

            bool inMacro = false;
            TokenCollection cachedToken = new();
            while (stream.GetToken(out var token))
            {
                int index = stream.Position - 1;

                if (indices.Contains(index))
                {
                    inMacro = true;
                }
                else if (inMacro && _endOfMacroPattern.Invoke(stream.Tokens, index))
                {
                    inMacro = false;
                    cachedToken.Append(token);
                    var marker = CacheToken(cachedToken);
                    stream.PutToken(marker);
                    continue;
                }

                if (inMacro)
                {
                    cachedToken.Append(token);
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
