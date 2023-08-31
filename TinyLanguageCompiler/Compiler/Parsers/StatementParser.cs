using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class StatementParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public StatementParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public IStatement? ParseStatement()
    {
        Token identifier = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        IStatement? statement = null;

        switch (identifier)
        {
            case { Type: TokenType.Identifier }:
                AssignmentParser assignmentParser = new(_tokenizer);
                statement = assignmentParser.ParseAssignmentStatement();
                break;

            case { Type: TokenType.WhileKeyword, Value: "while" }:
                WhileLoopParser whileLoopParser = new(_tokenizer);
                whileLoopParser.ParseWhileLoopStatement();
                break;

            case { Type: TokenType.IfKeyword, Value: "if" }:
                ConditionParser conditionParser = new(_tokenizer);
                conditionParser.ParseConditionStatement();
                break;

            case { Type: TokenType.ReturnKeyword, Value: "return" }:
                ReturnParser returnParser = new(_tokenizer);
                returnParser.ParseReturnStatement();
                break;

            default:
                throw new SyntaxException("Invalid token");
        }

        return statement;
    }
}
