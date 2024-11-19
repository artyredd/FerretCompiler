using System.Text.RegularExpressions;

namespace Ferret
{
    public class NamespaceRule : ILexerRule
    {
        TokenCollection _pattern = new(new List<Token>() { TokenType.Text, TokenType.Whitespace, TokenType.Text, TokenType.Control });
        TokenCollection _alternatePattern = new(new List<Token>() { TokenType.Text, TokenType.Whitespace, TokenType.Text, TokenType.Control});
        
        public void ApplyRule(TokenStream stream)
        {
            throw new NotImplementedException();
        }

        private bool VerifyPattern(TokenStream stream, int startIndex)
        {
            var tokens = stream.Tokens;
            // namespace name {
            // namespace name{
            bool hasNamespace = tokens[startIndex] == "namespace";
            bool hasBlockSeparator = tokens[startIndex + 3] == "{";
            bool otherBlockSlot  = hasBlockSeparator || tokens[startIndex + 4] == "{";

            return hasBlockSeparator && hasNamespace;
        }

        public bool RuleApplies(TokenStream stream)
        {
            int regular = stream.Tokens.IndexOf(_pattern);
            int alternate = stream.Tokens.IndexOf(_alternatePattern);

            bool regularFound = VerifyPattern(stream, regular);
            bool alternateFound = VerifyPattern(stream, alternate);

            return regularFound || alternateFound;
        }
    }
}
