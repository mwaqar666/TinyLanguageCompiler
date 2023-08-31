using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class AssignmentParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public AssignmentParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Assignment ParseAssignmentStatement()
    {
        Token identifierToAssign = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(identifierToAssign is not { Type: TokenType.Identifier });

        Token assignmentOperator = _tokenizer.NextToken();

        Assignment assignment;

        switch (assignmentOperator)
        {
            case { Type: TokenType.AssignmentOperator, Value: "=" }:
                assignment = CreateAssignment(identifierToAssign);
                break;

            case { Type: TokenType.Delimiter, Value: "[" }:
                _tokenizer.PreviousToken();
                assignment = ParseArrayAssignment(identifierToAssign);
                break;

            default:
                throw new SyntaxException($"""Identifier ${identifierToAssign}" is not assignment or call""");
        }

        Token terminator = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(terminator is not { Type: TokenType.Terminator, Value: ";" });

        return assignment;
    }

    private Assignment ParseArrayAssignment(Token identifier)
    {
        Token arrayIndexBracketStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(arrayIndexBracketStart is not { Type: TokenType.Delimiter, Value: "[" });

        ExpressionParser expressionParser = new(_tokenizer);
        Expression indexAccessExpression = expressionParser.ParseExpression();

        Token arrayIndexBracketEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(arrayIndexBracketEnd is not { Type: TokenType.Delimiter, Value: "]" });

        Token assignmentDelimiter = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(assignmentDelimiter is not { Type: TokenType.AssignmentOperator, Value: "=" });

        return CreateAssignment(identifier, indexAccessExpression);
    }

    private Assignment CreateAssignment(Token identifier, Expression? indexAccessExpression = null)
    {
        Assignment assignment = new(identifier, _tokenizer);

        if (indexAccessExpression is not null) assignment.SetAssigneeIndexAccessExpression(indexAccessExpression);

        ExpressionParser expressionParser = new(_tokenizer);
        Expression assignedExpression = expressionParser.ParseExpression();

        assignment.SetAssignment(assignedExpression);

        return assignment;
    }
}