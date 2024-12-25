using System.Collections.Frozen;

namespace Ferret
{
    public class TokenPattern
    {
        private readonly List<Func<Token, bool>> _preExpressions = new();
        private readonly List<Func<Token, bool>> _postExpressions = new();
        private readonly List<Func<Token, bool>> _expressions = new();

        public TokenPattern(bool missingTokensCountAsPassing = true)
        {
            MissingTokensCountAsPassing = missingTokensCountAsPassing;
        }

        public bool MissingTokensCountAsPassing { get; set; } = true;

        public TokenPattern When(Func<Token, bool> expression)
        {
            _expressions.Add(expression);
            return this;
        }

        public TokenPattern When(string s)
        {
            _expressions.Add(x=> x == s);
            return this;
        }

        public TokenPattern When(char c)
        {
            _expressions.Add(x=>x == $"{c}");
            return this;
        }

        public TokenPattern When(TokenType type)
        {
            _expressions.Add(x => x == type);
            return this;
        }

        public TokenPattern IsPreceededBy(Func<Token, bool> expression) 
        {    
           _preExpressions.Add(expression);
            return this;
        }

        public TokenPattern IsPreceededBy(TokenType type)
        {
            return IsPreceededBy(x => x == type);
        }

        public TokenPattern IsPreceededBy(string s)
        {
            return IsPreceededBy(x => x == s);
        }

        public TokenPattern IsPreceededBy(char c)
        {
            return IsPreceededBy(x => x == $"{c}");
        }


        public TokenPattern IsFollowedBy(Func<Token,bool> expression)
        { 
            _postExpressions.Add(expression);
            return this;
        }

        public TokenPattern IsFollowedBy(TokenType type)
        {
            return IsFollowedBy(x => x == type);
        }

        public TokenPattern IsFollowedBy(string s)
        {
            return IsFollowedBy( x => x == s);
        }

        public TokenPattern IsFollowedBy(char c)
        {
            return IsFollowedBy(x => x == $"{c}");
        }

        public bool Invoke(List<Token> tokens, int index)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }
            if (index > (tokens.Count - 1) || index < 0)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            bool result = true;

            foreach (var expr in _expressions)
            {
                bool exprValue = expr(tokens[index]);
                result = result && exprValue;
                if (result is false)
                {
                    return false;
                }
            }

            // dont bother checking preceeding or following expressions
            // if we dont have to
            if (_preExpressions.Count > 0 || _postExpressions.Count > 0)
            {
                int preceedingIndex = int.Max(index - _preExpressions.Count, 0);
                int followingIndex = int.Min(index+1, tokens.Count - 1);

                bool notEnoughTokensBeforeIndex = index < _preExpressions.Count;
                bool notEnoughTokensAfter = (tokens.Count - (index + _postExpressions.Count)) > tokens.Count - index;

                if (!MissingTokensCountAsPassing && (notEnoughTokensBeforeIndex || notEnoughTokensAfter))
                {
                    return false;
                }

                int i = index;
                int expressionIndex = 0;
                while (i-->preceedingIndex)
                {
                    var token = tokens[i];
                    if (_preExpressions[expressionIndex++](token) is false)
                    {
                        return false;
                    }
                }

                expressionIndex = 0;
                for (i = followingIndex; i < int.Min(_postExpressions.Count, tokens.Count); i++)
                {
                    var token = tokens[i];
                    if (_postExpressions[expressionIndex++](token) is false)
                    {
                        return false;
                    }
                }
            }

            return result;
        }
    }
}
