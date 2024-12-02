﻿using System.Collections.Frozen;

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
            _preExpressions.Add(x=>x==type);
            return this;
        }

        public TokenPattern IsFollowedBy(Func<Token,bool> expression)
        { 
            _postExpressions.Add(expression);
            return this;
        }

        public TokenPattern IsFollowedBy(TokenType type)
        {
            _postExpressions.Add(x => x == type);
            return this;
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
            }

            // dont bother checking preceeding or following expressions
            // if we dont have to
            if (result && (_preExpressions.Count > 0 || _postExpressions.Count > 0))
            {
                int preceedingIndex = int.Max(index - _preExpressions.Count, 0);
                int followingIndex = int.Min(index+1, tokens.Count - 1);

                bool notEnoughTokensBeforeIndex = index < _preExpressions.Count;
                bool notEnoughTokensAfter = (tokens.Count - (index + _postExpressions.Count)) > tokens.Count - index;

                if (!MissingTokensCountAsPassing && (notEnoughTokensBeforeIndex || notEnoughTokensAfter))
                {
                    return false;
                }

                for (int i = preceedingIndex; i < index; i++)
                {
                    var token = tokens[i];
                    if (_preExpressions[i-preceedingIndex](token) is false)
                    {
                        return false;
                    }
                }

                for (int i = followingIndex; i < int.Min(_postExpressions.Count, tokens.Count); i++)
                {
                    var token = tokens[i];
                    if (_postExpressions[i - index](token) is false)
                    {
                        return false;
                    }
                }
            }

            return result;
        }
    }
}