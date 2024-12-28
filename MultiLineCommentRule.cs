namespace Ferret
{
    public class MultiLineCommentRule : CachedRuleBase, ILexerRule
    {
        private readonly TokenPattern _pattern = new TokenPattern()
                .When(TokenType.Control)
                .When('/')
                .IsFollowedBy('*');

        private readonly TokenPattern _endPattern = new TokenPattern()
            .When(TokenType.Control)
            .When('/')
            .IsPreceededBy("*");

        public void ApplyRule(TokenStream stream)
        {
            // cache macros so we can re-insert them later
            var indices = ValidIndexes(stream);

            bool inComment = false;
            TokenCollection cachedToken = new();
            while (stream.GetToken(out var token))
            {
                int index = stream.Position - 1;

                if (indices.Contains(index))
                {
                    inComment = true;
                }
                else if (inComment && _endPattern.Invoke(stream.Tokens, index))
                {
                    inComment = false;
                    cachedToken.Append(token);
                    var marker = CacheToken(cachedToken);
                    stream.PutToken(marker);
                    continue;
                }

                if (inComment)
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
