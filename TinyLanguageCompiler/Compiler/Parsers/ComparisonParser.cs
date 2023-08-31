using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class ComparisonParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public ComparisonParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public void ParseComparison()
    {
        ExpressionParser expressionParser = new(_tokenizer);
        Expression firstComparisonExpression = expressionParser.ParseExpression();

        Token comparatorOperator = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(comparatorOperator is not { Type: TokenType.ComparisonOperator });

        ValidateExpressionComparison(firstComparisonExpression, comparatorOperator);

        Expression secondComparisonExpression = expressionParser.ParseExpression();

        if (firstComparisonExpression.ExpressionType != secondComparisonExpression.ExpressionType) throw new TypeException("Comparison between two expressions must be of same type");
    }

    private static void ValidateExpressionComparison(Expression firstComparisonExpression, Token comparatorOperator)
    {
        switch (firstComparisonExpression)
        {
            case { ExpressionType: DataType.Int } or { ExpressionType: DataType.Float }:
            case { ExpressionType: DataType.Bool } or { ExpressionType: DataType.Char } when comparatorOperator is { Value: "==" }:
                return;
            case { ExpressionType: DataType.Bool } or { ExpressionType: DataType.Char }:
                throw new LogicalException("""Boolean and Character comparison are valid for "==" operator only""");
        }
    }
}