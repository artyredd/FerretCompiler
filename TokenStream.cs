namespace Ferret
{
    public class TokenStream
    {
        private TokenCollection _input;
        public TokenCollection InputTokens { get { return _input; } }
        private TokenCollection _output = new();
        public TokenCollection OutputTokens { get { return _output; } }
        int _position = 0;

        public TokenStream(TokenCollection inputCollection)
        {
            _input = inputCollection;
        }

        public override string ToString()
        {
            return _input.Build();
        }

        // Swaps input and output streams
        public void Swap()
        {
            _input = _output;
            _output = new();
            ResetPosition();
        }

        public void ResetPosition()
        {
            _position = 0;
        }

        // gets the next token in the stream and consumes the token (moves the position pointer)
        // Token is not valid when this method returns false
        // Returns false when no more tokens exist in the stream
        public bool GetToken(out Token token)
        {
            token = TokenType.None;

            if (_input.Count > _position)
            { 
                token = _input[_position];
                _position += 1;
                return true;
            }

            return false;
        }

        public void PutToken(Token token)
        {
            _output.Append(token);
        }
    }
}
