namespace Ferret
{
    public class CachedRuleBase
    {

        public static List<TokenCollection> CachedTokens { get; set; } = new();

        public static Token CacheToken(TokenCollection token)
        {
            Token cachedToken = new(TokenType.CachedData, string.Empty);
            cachedToken.Id = CachedTokens.Count;
            CachedTokens.Add(token);
            return cachedToken;
        }
    }
}