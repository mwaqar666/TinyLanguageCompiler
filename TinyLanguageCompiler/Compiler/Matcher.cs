using System.Text.RegularExpressions;

namespace TinyLanguageCompiler.Compiler;

public static partial class RegexMatcher
{
    [GeneratedRegex(@"/(int|float|char|bool|return|if|else|while|true|false)|('[^\n']')|(\d+(\.\d{1,2})?)|(,|;|{|}|\(|\)|\[|\])|(\+|-|\*|\/|%)|(&&|\|\|)|(==|>=|<=|>|<)|(=)|([a-zA-Z_]\w*)")]
    public static partial Regex TokenizePattern();

    [GeneratedRegex(@"\d+(\.\d{1,2})?")]
    public static partial Regex NumberPattern();

    [GeneratedRegex(@"[a-zA-Z_]\w*")]
    public static partial Regex IdentifierPattern();

    [GeneratedRegex(@"\/\/[^\n]*")]
    public static partial Regex SingleLineCommentPattern();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    public static partial Regex MultiLineCommentPattern();
}