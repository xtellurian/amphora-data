using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Amphora.Common.Extensions
{
    public static class IdentifierExtensions
    {
        // definition of a valid C# identifier: http://msdn.microsoft.com/en-us/library/aa664670(v=vs.71).aspx
        private const string FORMATTING = @"\p{Cf}";
        private const string CONNECTING = @"\p{Pc}";
        private const string DECIMAL = @"\p{Nd}";
        private const string COMBINING = @"\p{Mn}|\p{Mc}";
        private const string LETTER = @"\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}";

        private const string PART = LETTER + "|" +
                                                         DECIMAL + "|" +
                                                         CONNECTING + "|" +
                                                         COMBINING + "|" +
                                                         FORMATTING;

        private const string PARTS = "(" + PART + ")+";
        private const string START = "(" + LETTER + "|_)";

        private const string ALL = START + "(" + PARTS + ")*";

        // C# keywords: http://msdn.microsoft.com/en-us/library/x53a06bb(v=vs.71).aspx
        private static readonly string[] _keywords = new[]
        {
        "abstract",  "event",      "new",        "struct",
        "as",        "explicit",   "null",       "switch",
        "base",      "extern",     "object",     "this",
        "bool",      "false",      "operator",   "throw",
        "break",     "finally",    "out",        "true",
        "byte",      "fixed",      "override",   "try",
        "case",      "float",      "params",     "typeof",
        "catch",     "for",        "private",    "uint",
        "char",      "foreach",    "protected",  "ulong",
        "checked",   "goto",       "public",     "unchekeced",
        "class",     "if",         "readonly",   "unsafe",
        "const",     "implicit",   "ref",        "ushort",
        "continue",  "in",         "return",     "using",
        "decimal",   "int",        "sbyte",      "virtual",
        "default",   "interface",  "sealed",     "volatile",
        "delegate",  "internal",   "short",      "void",
        "do",        "is",         "sizeof",     "while",
        "double",    "lock",       "stackalloc",
        "else",      "long",       "static",
        "enum",      "namespace",  "string"
        };

        private static readonly Regex _validIdentifierRegex = new Regex("^" + ALL + "$", RegexOptions.Compiled);

        public static bool IsValidIdentifier(this string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return false;
            }

            var normalizedIdentifier = identifier.Normalize();

            // 1. check that the identifier match the validIdentifer regex and it's not a C# keyword
            if (_validIdentifierRegex.IsMatch(normalizedIdentifier) && !_keywords.Contains(normalizedIdentifier))
            {
                return true;
            }

            // 2. check if the identifier starts with @
            if (normalizedIdentifier.StartsWith("@") && _validIdentifierRegex.IsMatch(normalizedIdentifier.Substring(1)))
            {
                return true;
            }

            // 3. it's not a valid identifier
            return false;
        }
    }
}