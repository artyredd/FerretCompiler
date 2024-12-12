using System.Text;

namespace Ferret
{
    public static class TokenExtensions
    {
        public static string[] SplitInclusive(this string source, string delimiter)
        {
            string str = source;
            var result = new List<string>();

            int index = 0;
            while ((index = str.IndexOf(delimiter)) != -1)
            {
                if (index > 0)
                { 
                    result.Add(str[..index]);
                }
                result.Add(delimiter);
                str = str[(index+delimiter.Length)..];
            }

            if (str.Length > 0)
            { 
                result.Add(str);
            }

            return result.ToArray();
        }

        public static string ReplaceWhitespaceNames(this string str)
        {
            StringBuilder builder = new();

            foreach (var item in str)
            {
                builder.Append(item.MaybeGetWhitespaceName());
            }

            return builder.ToString();
        }

        public static string ToReadableString(this TokenCollection tokens)
        { 
            StringBuilder builder = new();

            tokens.ForEach(x=>builder.Append(x.ToString() + "\n"));

            return builder.ToString();
        }

        public static string MaybeGetWhitespaceName(this char c)
        {
            if (char.IsWhiteSpace(c))
            {
                switch (c)
                { 
                    case ' ':
                        return "\\s";
                    case '\r':
                        return "\\r";
                    case '\n':
                        return "\\n";
                    case '\t':
                        return "\\t";
                }
            }

            return c.ToString();
        }
    }
}
