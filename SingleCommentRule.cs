namespace Ferret
{
    public class SingleCommentRule : CachedRuleBase, ILexerRule
    {
        readonly TokenCollection _pattern = new() { TokenType.Whitespace, TokenType.Control };

        private bool IsEndOfMacro(Token currentToken, Token previousToken)
        {
            bool isWhiteSpace = currentToken == TokenType.Whitespace;
            bool newLine = currentToken.Value.EndsWith('\n');
            bool notEscaped = !previousToken.Value.EndsWith('\\');

            return isWhiteSpace && newLine && notEscaped;
        }

        public void ApplyRule(TokenStream stream)
        {
            // cache macros so we can re-insert them later
            var indices = ValidIndexes(stream);

            bool inComment = false;
            TokenCollection cachedToken = new();
            Token previousToken = new(TokenType.None, string.Empty);
            while (stream.GetToken(out var token))
            {
                int index = stream.Position - 1;

                if (indices.Contains(index))
                {
                    inComment = true;
                    previousToken = token;
                }
                else if (inComment && IsEndOfMacro(token, previousToken))
                {
                    inComment = false;
                    cachedToken.Append(token);
                    var marker = CacheToken(cachedToken);
                    stream.PutToken(marker);
                    continue;
                }
                else if (inComment)
                {
                    cachedToken.Append(token);
                    previousToken = token;
                    continue;
                }

                previousToken = token;

                stream.PutToken(token);
            }
        }

        List<int> ValidIndexes(TokenStream stream)
        {
            return stream.Tokens.IndexesOf(_pattern).Where(x => VerifyPattern(stream, x)).ToList();
        }

        bool VerifyPattern(TokenStream stream, int index)
        {
            return stream.Tokens[index + 1].Value.StartsWith("//");
        }

        public bool RuleApplies(TokenStream stream)
        {
            return ValidIndexes(stream).Count != 0;
        }
    }
}
