namespace KenticoInspector.Core.TokenExpressions.Models
{
    internal class Constants
    {
        public const char Pipe = '|';
        public const char Colon = ':';
        public const char Equality = '=';
        public const char LessThan = '<';
        public const char MoreThan = '>';

        public const char Space = ' ';

        public static readonly char[] OperationChars =
        {
            LessThan,
            MoreThan
        };

        public static readonly char[] LeadingChars =
        {
            Space,
            '`',
            '('
        };

        public static readonly char[] TrailingChars =
        {
            Space,
            '`',
            Colon,
            '.',
            ',',
            ')'
        };
    }
}