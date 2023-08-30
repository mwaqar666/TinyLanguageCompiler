namespace TinyLanguageCompiler.Enums;

public enum TokenType
{
    DataTypeKeyword = 2,
    ReturnKeyword = 4,
    ConditionalKeyword = 8,
    WhileLoopKeyword = 16,
    NumberLiteral = 32,
    CharacterLiteral = 64,
    BooleanLiteral = 128,
    Identifier = 256,
    NumberOperator = 512,
    BooleanOperator = 1024,
    ComparisonOperator = 2048,
    AssignmentOperator = 4096,
    Delimiter = 8192,
    Terminator = 16384
}