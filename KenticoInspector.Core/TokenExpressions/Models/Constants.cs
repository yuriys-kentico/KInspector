namespace KenticoInspector.Core.TokenExpressions.Models
{
    internal class Constants
    {
        public const char Pipe = '|';
        public const char Colon = ':';
        public const char Equality = '=';
        public const char LessThan = '<';
        public const char MoreThan = '>';
        public static readonly char[] OperationChars = new[] { LessThan, MoreThan };

        public const char Space = ' ';
        public static readonly char[] LeadingChars = new[] { Space, '`', '(' };
        public static readonly char[] TrailingChars = new[] { Space, '`', Colon, '.', ',', ')' };
    }
}