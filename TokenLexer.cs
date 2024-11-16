using System.Reflection;
using System.Collections;
using System.Linq;

namespace Ferret
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleAttribute : Attribute
    { }

    public interface ILexerRule
    {
        // returns true when the rule possibly applies to the given stream
        public bool RuleApplies(TokenStream stream);
        
        // Reads, consumes, and outputs tokens to the stream to apply the rule
        public void ApplyRule(TokenStream stream);
    }

    public class TokenLexer
    {
        private readonly TokenStream _stream;
        private readonly List<ILexerRule> _rules = new();

        public TokenLexer(TokenStream stream)
        {
            _stream = stream;
        }

        public TokenLexer AppendRule(ILexerRule rule)
        {
            _rules.Add(rule);
            return this;
        }

        public void ProcessRules()
        {
            foreach (var rule in _rules)
            {
                while (rule.RuleApplies(_stream))
                {
                    rule.ApplyRule(_stream);
                    _stream.Swap();
                }
            }
        }
    }
}
