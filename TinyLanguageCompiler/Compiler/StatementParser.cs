using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class StatementParser
{
    private readonly Function _currentFunction;
    private readonly Tokenizer _tokenizer;

    public StatementParser(Tokenizer tokenizer, Function functionContext)
    {
        _tokenizer = tokenizer;
        _currentFunction = functionContext;
    }

    public void ParseFunctionStatements()
    {
        Token identifier = _tokenizer.NextToken();
        switch (identifier)
        {
            case { Type: TokenType.Identifier }:
                // This can either be a function call or a variable assignment
                _tokenizer.PreviousToken();
                ParseAssignmentOrFunctionCallStatement();
                break;

            case { Type: TokenType.ReturnKeyword, Value: "return" }:
                _tokenizer.PreviousToken();
                ParseReturnStatement();
                break;

            case { Type: TokenType.WhileLoopKeyword, Value: "while" }:
                _tokenizer.PreviousToken();
                ParseWhileLoopStatement();
                break;

            case { Type: TokenType.ConditionalKeyword, Value: "if" }:
                _tokenizer.PreviousToken();
                ParseConditionStatement();
                break;
        }

        throw new SyntaxException("Invalid token");
    }

    private void ParseAssignmentOrFunctionCallStatement()
    {
        Token identifierToAssignOrCall = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(identifierToAssignOrCall is not { Type: TokenType.Identifier });

        Token assignmentOperator = _tokenizer.NextToken();
        if (assignmentOperator is { Type: TokenType.Delimiter, Value: "(" })
        {
            // Definitely function call, check function existence & start evaluating expressions
        }
        else if (assignmentOperator is { Type: TokenType.AssignmentOperator, Value: "=" })
        {
            // Definitely variable assignment, start evaluating expressions
        }
        else if (assignmentOperator is { Type: TokenType.Delimiter, Value: "[" })
        {
            // Definitely array index assignment, evaluate integer expression, and start evaluating expression
        }

        throw new SyntaxException($"""Identifier ${identifierToAssignOrCall}" is not assignment or call""");
    }

    private void ParseConditionStatement()
    {
        //
    }

    private void ParseWhileLoopStatement()
    {
        //
    }

    private void ParseReturnStatement()
    {
        //
    }
}
