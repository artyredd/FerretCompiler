namespace Ferret
{
    internal partial class Program
    {
        static List<string> GetPathsFromArgs(string[] args)
        {
            List<string> paths = new();

            foreach (string filePath in args)
            {
                if (File.Exists(filePath))
                {
                    paths.Add(filePath);
                }
                else
                {
                    Console.WriteLine($"WARNING: File not found: {filePath}");
                }
            }
            return paths;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a list of file paths.");
                return;
            }

            var paths = GetPathsFromArgs(args);

            foreach (string filePath in paths)
            {
                var data = File.ReadAllText(filePath);
                var tokens = new Tokenizer(data).Parse();
                var stream = new TokenStream(tokens);
                var lexer = new TokenLexer(stream);
            }
        }
    }
}
