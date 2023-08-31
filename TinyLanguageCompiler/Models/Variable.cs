using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Models;

public class Variable : IEvaluable
{
    private readonly bool _isArray;
    private readonly string[] _variableArray;
    private Expression? _indexAccessExpression;

    private Expression? _valueExpression;

    public Variable(string fullName, DataType dataType, int arraySize = 0)
    {
        DataType = dataType;
        Name = fullName[(fullName.IndexOf('.') + 1)..];

        _variableArray = new string[Math.Max(1, arraySize)];
        _isArray = arraySize > 0;
    }

    public string Name { get; }

    public DataType DataType { get; }

    public string Evaluate(bool force = false)
    {
        int internalArrayIndex = 0;

        if (_isArray)
        {
            if (_indexAccessExpression is null) throw new LogicalException($"""Cannot assign to, or access from array "{Name}" without index""");

            if (_indexAccessExpression is not { ExpressionType: DataType.Int }) throw new LogicalException("Invalid non-integer value used to access array index");

            internalArrayIndex = int.Parse(_indexAccessExpression.Evaluate(force));

            if (internalArrayIndex >= _variableArray.Length) throw new BufferOverflowException($"""Cannot access index {internalArrayIndex} of variable "{Name}" with array size {_variableArray.Length}""");
        }

        if (!force) return _variableArray[internalArrayIndex];

        if (_valueExpression is null) throw new LogicalException($"""Cannot evaluate value of variable "{Name}".""");

        _variableArray[internalArrayIndex] = _valueExpression.Evaluate(force);

        return _variableArray[internalArrayIndex];
    }

    public void SetIndexAccessExpression(Expression indexAccessExpression)
    {
        if (!_isArray) throw new LogicalException($"""Cannot access index of variable "{Name}" because it is not an array""");

        _indexAccessExpression = indexAccessExpression;
    }

    public void SetValueExpression(Expression valueExpression)
    {
        _valueExpression = valueExpression;
    }
}