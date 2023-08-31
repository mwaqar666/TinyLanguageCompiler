namespace TinyLanguageCompiler.Enums;

public enum TokenType
{
    DataTypeKeyword = 2,
    ReturnKeyword = 4,
    IfKeyword = 8,
    ElseKeyword = 16,
    WhileKeyword = 32,
    IntLiteral = 64,
    FloatLiteral = 128,
    BooleanLiteral = 256,
    CharacterLiteral = 512,
    Identifier = 1024,
    NumericOperator = 2048,
    BooleanOperator = 4096,
    AssignmentOperator = 8192,
    ComparisonOperator = 16384,
    Delimiter = 32768,
    Terminator = 65536
}