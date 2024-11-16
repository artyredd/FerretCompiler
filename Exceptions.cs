namespace Ferret
{
    public static class Exceptions
    {
        public class CharacterNotHandledException : Exception
        {
            public CharacterNotHandledException(string? message) : base(message)
            {
            }
        }
    }
}
