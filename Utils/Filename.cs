using System.Collections.Generic;

namespace Media_Rename.Utils
{
    static class Filename
    {
        public static string CleanUp(string text)
        {
            return RemoveTrailingSymbols(RemoveBrackets(text.Replace("_", " ").Replace(".", " "))).Trim();
        }

        static string RemoveTrailingSymbols(string text)
        {
            for (var i = text.Length - 1; i >= 0; i--)
            {
                if (text[i] != ' ' && text[i] != '-')
                    return text.Substring(0, i + 1);
            }
            return "";
        }

        static string RemoveBrackets(string text)
        {
            var output = new char[text.Length];
            var outputLength = 0;
            var stack = new Stack<char>();
            for (var i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '(':
                    case '[':
                    case '{':
                        stack.Push(text[i] == '(' ? ')' : text[i] == '[' ? ']' : '}');
                        break;
                    case ')':
                    case ']':
                    case '}':
                        if (stack.Contains(text[i]))
                        {
                            while (stack.Count > 0 && stack.Pop() != text[i]) ;
                        }
                        break;
                    default:
                        if (stack.Count == 0)
                        {
                            output[outputLength++] = text[i];
                        }
                        break;
                }
            }
            return new string(output, 0, outputLength);
        }
    }
}
