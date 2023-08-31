using TinyLanguageCompiler.Compiler.Tokenizer;
using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Models;

public class Assignment : IStatement
{
    private readonly Variable _assigneeVariable;
    private Expression? _expressionEvaluation;
    private Expression? _indexAccessExpression;

    public Assignment(Token assigneeIdentifier, Tokenizer tokenizer)
    {
        if (tokenizer.CurrentFunction is null) throw new SyntaxException("Assignment cannot be outside of function scope");
        _assigneeVariable = tokenizer.SymbolTable.GetVariable(assigneeIdentifier.Value, tokenizer.CurrentFunction.Name);
    }

    public void Run()
    {
        if (_indexAccessExpression is not null) _assigneeVariable.SetIndexAccessExpression(_indexAccessExpression);

        if (_expressionEvaluation is null) throw new LogicalException("Cannot evaluate null expression");

        _assigneeVariable.SetValueExpression(_expressionEvaluation);

        _assigneeVariable.Evaluate(true);
    }

    public void SetAssigneeIndexAccessExpression(Expression indexAccessExpression)
    {
        if (indexAccessExpression is not { ExpressionType: DataType.Int }) throw new LogicalException("Invalid non-integer value used to access array index");

        _indexAccessExpression = indexAccessExpression;
    }

    public void SetAssignment(Expression expressionEvaluation)
    {
        if (_assigneeVariable.DataType != expressionEvaluation.ExpressionType) throw new TypeException($"""Cannot assign expression of type "{expressionEvaluation.ExpressionType}" to variable "{_assigneeVariable.Name}".""");

        _expressionEvaluation = expressionEvaluation;
    }
}
