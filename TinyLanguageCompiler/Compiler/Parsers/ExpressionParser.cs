using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class ExpressionParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;


    public ExpressionParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Expression ParseExpression()
    {
        Token leftOperand = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(leftOperand is not ({ Type: TokenType.Identifier } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.BooleanLiteral } or { Type: TokenType.IntLiteral }));

        Expression expression = new(_tokenizer);
        expression.AddLeftOperand(leftOperand);

        Token possibleLeftOperandIndexAccess = _tokenizer.NextToken();
        if (possibleLeftOperandIndexAccess is { Type: TokenType.Delimiter, Value: "[" })
        {
            Expression leftOperandIndexAccessExpression = ParseExpression();
            expression.AddLeftOperandIndexAccess(leftOperandIndexAccessExpression);

            Token leftOperandIndexAccessEnd = _tokenizer.NextToken();
            ExceptionFactory.CreateSyntaxExceptionIf(leftOperandIndexAccessEnd is not { Type: TokenType.Delimiter, Value: "]" });
        }
        else
        {
            _tokenizer.PreviousToken();
        }

        Token possibleOperator = _tokenizer.NextToken();
        if (possibleOperator is not ({ Type: TokenType.NumericOperator } or { Type: TokenType.BooleanOperator }))
        {
            _tokenizer.PreviousToken();
            return expression;
        }

        expression.AddOperator(possibleOperator);

        Token rightOperand = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(rightOperand is not ({ Type: TokenType.Identifier } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.BooleanLiteral } or { Type: TokenType.IntLiteral }));

        expression.AddRightOperand(rightOperand);

        Token possibleRightOperandIndexAccess = _tokenizer.NextToken();
        if (possibleRightOperandIndexAccess is { Type: TokenType.Delimiter, Value: "[" })
        {
            Expression rightOperandIndexAccessExpression = ParseExpression();
            expression.AddRightOperandIndexAccess(rightOperandIndexAccessExpression);

            Token rightOperandIndexAccessEnd = _tokenizer.NextToken();
            ExceptionFactory.CreateSyntaxExceptionIf(rightOperandIndexAccessEnd is not { Type: TokenType.Delimiter, Value: "]" });
        }
        else
        {
            _tokenizer.PreviousToken();
        }

        return expression;
    }
}