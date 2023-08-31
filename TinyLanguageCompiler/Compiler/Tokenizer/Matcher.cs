using System.Text.RegularExpressions;

namespace TinyLanguageCompiler.Compiler.Tokenizer;

public static partial class RegexMatcher
{
    [GeneratedRegex(@"(int|float|bool|char|if|else|while|true|false|return)|('[^\n\t\r']')|(\d+\.\d{1,2})|(\d+)|(,|;|{|}|\(|\)|\[|\])|(\+|-|\*|\/|%)|(&&|\|\||==|!=|>=|<=|=|>|<)|([a-zA-Z_]\w*)")]
    public static partial Regex TokenizePattern();

    [GeneratedRegex(@"\d+")]
    public static partial Regex IntPattern();

    [GeneratedRegex(@"\d+\.\d{1,2}")]
    public static partial Regex FloatPattern();

    [GeneratedRegex(@"[a-zA-Z_]\w*")]
    public static partial Regex IdentifierPattern();

    [GeneratedRegex(@"\/\/[^\n]*")]
    public static partial Regex SingleLineCommentPattern();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    public static partial Regex MultiLineCommentPattern();
}