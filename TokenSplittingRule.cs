namespace Ferret
{
    // splits advanced groups of tokens that contain the start of a comment
    // removes degenerate cases such as doThing();// does thing
    public class TokenSplittingRule : CachedRuleBase, ILexerRule
    {
        readonly TokenType tokenType;
        readonly string[] delimiters;

        public TokenSplittingRule(TokenType tokenType, string[] delimiters)
        {
            this.delimiters = delimiters;
            this.tokenType = tokenType;
            _startPattern = new TokenPattern()
            .When(tokenType)
            .When(x => ContainsDelmiter(delimiters, x));
        }

        static bool ContainsDelmiter(string[] delimiters, Token t)
        {
            foreach (var item in delimiters)
            {
                if (t.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        readonly TokenPattern _startPattern;

        void AddSplitTokens(TokenStream stream, Token token, string delimiter)
        {
            var tokens = token.Split(delimiter);
            if (tokens.Length == 0)
            {
                return;
            }

            foreach (var newToken in tokens)
            {
                stream.PutToken(newToken);
            }
        }

        void MaybeAddSplitTokens(TokenStream stream, Token token)
        {
            foreach (var delimiter in delimiters)
            {
                AddSplitTokens(stream,token,delimiter);
            }
        }

        public void ApplyRule(TokenStream stream)
        {
            // cache macros so we can re-insert them later
            var indices = ValidIndexes(stream);

            TokenCollection cachedToken = new();
            while (stream.GetToken(out var token))
            {
                // check to see if this token is an advanced pattern to split
                if (_startPattern.Invoke(stream.Tokens, stream.Position-1))
                {
                    MaybeAddSplitTokens(stream,token);
                }
                else 
                { 
                    stream.PutToken(token);
                }
            }
        }

        List<int> ValidIndexes(TokenStream stream)
        {
            return stream.Tokens.IndexesOf(_startPattern);
        }

        public bool RuleApplies(TokenStream stream)
        {
            return ValidIndexes(stream).Count != 0;
        }
    }
}
