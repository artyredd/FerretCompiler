using System.Collections;
using System.Text;

namespace Ferret
{
    public class TokenCollection : IList<Token>, IEnumerable<Token>, IEnumerable<char>
    {
        private readonly List<Token> _tokens = new();
        private StringBuilder _builder = new();
        private string _builtString = string.Empty;
        private bool _dirty = true;

        public int Count => ((ICollection<Token>)_tokens).Count;

        public bool IsReadOnly => ((ICollection<Token>)_tokens).IsReadOnly;

        public Token this[int index] { get => ((IList<Token>)_tokens)[index]; set => ((IList<Token>)_tokens)[index] = value; }

        public TokenCollection()
        { }

        public TokenCollection(Token token)
        {
            Append(token);
        }

        public TokenCollection(List<Token> tokens)
        {
            Append(tokens);
        }

        private void AppendToBuilder(Token token)
        {
            _builder.Append($"{(int)token.Type}");
        }

        public void ForEach(Action<Token> expression) => _tokens.ForEach(expression);

        // Combines the tokens into a output string for writing to a file
        public string Build()
        {
            StringBuilder builder = new();

            ForEach(x=>builder.Append(x.Value));

            return builder.ToString();
        }

        public TokenCollection GetRange(int start)
        {
            return GetRange(start,_tokens.Count - start);
        }

        public TokenCollection GetRange(int start, int count)
        { 
            return _tokens.GetRange(start, count); 
        }

        public TokenCollection Append(List<Token> tokens)
        {
            tokens.ForEach(x => { _tokens.Add(x); AppendToBuilder(x); });
            _dirty = true;
            return this;
        }
        public TokenCollection Append(Token token)
        {
            _tokens.Add(token);
            AppendToBuilder(token);
            _dirty = true;
            return this;
        }

        public override string ToString()
        {
            if (_dirty)
            {
                _dirty = false;
                _builtString = _builder.ToString();
            }

            return _builtString;
        }

        /// <summary>
        /// Converts a list of tokens into a string where each character is an integer that corresponds
        /// to a TokenType. This generates a string you can "search" through using string tools to locate
        /// Token Groupings for lexing logic
        /// </summary>
        public static string ToParsableString(List<Token> tokens)
        {
            StringBuilder builder = new();

            tokens.ForEach(x=>builder.Append($"{(int)x.Type}"));

            return builder.ToString();
        }

        public static implicit operator string(TokenCollection collection) => collection.ToString();

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return ((IEnumerable<char>)_builtString).GetEnumerator();
        }

        public int IndexOf(TokenCollection collection)
        {
            return ToString().IndexOf(collection.ToString());        
        }

        public List<int> IndexesOf(TokenCollection collection)
        { 
            string str = ToString();
            string other = collection.ToString();

            return IndexesOf(str,other);
        }

        public List<int> IndexesOf(Token item)
        {
            return IndexesOf(ToString(),$"{(int)item.Type}");
        }

        private List<int> IndexesOf(string str, string other)
        {
            List<int> result = new();

            TokenCollection tokens = _tokens;
            int previousLength = 0;
            do {
                int index = str.IndexOf(other);
                
                if (index == -1)
                {
                    break;
                }

                result.Add(previousLength + index);
                int offset = index + other.Length;
                previousLength += offset;
                str = str.Substring(offset);
            }
            while (true);

            return result;
        }

        public int IndexOf(Token item)
        {
            return _tokens.IndexOf(item);
        }

        public void Insert(int index, Token item)
        {
            _tokens.Insert(index, item);
            string value = $"{(char)item.Type}";
            _builtString = _builtString.Insert(index, value);
            _builder.Insert(index,value);
        }

        public void RemoveAt(int index)
        {
            _tokens.RemoveAt(index);
            _builtString = _builtString.Remove(index, 1);
            _builder.Remove(index, 1);
        }

        public void Add(Token item)
        {
            Append(item);
        }

        public void Clear()
        {
            _tokens.Clear();
        }

        public bool Contains(Token item)
        {
            return _tokens.Contains(item);
        }

        public void CopyTo(Token[] array, int arrayIndex)
        {
            _tokens.CopyTo(array, arrayIndex);
        }

        public bool Remove(Token item)
        {
            int index = _tokens.IndexOf(item);
            if (index >= 0)
            {
                _builtString = _builtString.Remove(index,1);
                _builder.Remove(index,1);
            }
            return _tokens.Remove(item);
        }

        public static implicit operator TokenCollection(List<Token> tokens)
        {
            TokenCollection result = new();
            result.Append(tokens);
            return result;
        }
    }
}
