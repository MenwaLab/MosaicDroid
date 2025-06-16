namespace MosaicDroid.Core
{
    using System.Collections;

    public class TokenStream : IEnumerable<Token>
    {
        // encapsula la lista de tokens y un índice de posición
        private readonly List<Token> tokens;
        private int position;
        public int Position => position;

        public TokenStream(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
            position = 0;
        }

        public void MoveNext(int k = 1) => position += k; // salta k posiciones

        public bool Next(TokenType type) //  comprueba y consume el siguiente token si coincide con type.
        {
            if (position < tokens.Count - 1 && LookAhead(1).Type == type)
            {
                position++;
                return true;
            }
            return false;
        }

        public Token Advance() // devuelve el token actual y avanza la posición
        {
            var tok = tokens[position];
            position++;
            return tok;
        }

        public bool CanLookAhead(int k = 0) => tokens.Count - position > k;

        public Token LookAhead(int k = 0) // permiten inspeccionar tokens futuros sin avanzar.
        {
            // clamp so we never go out of range:
            int idx = position + k;
            if (idx < 0) idx = 0;
            if (idx >= tokens.Count) idx = tokens.Count - 1;
            return tokens[idx];
        }

        public IEnumerator<Token> GetEnumerator()
        {
            for (int i = position; i < tokens.Count; i++)
                yield return tokens[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); //y
    }
}