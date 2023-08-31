using TinyLanguageCompiler.Compiler.Tokenizer;
using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Models;

public class Expression : IEvaluable
{
    private readonly Function _currentFunction;
    private readonly Tokenizer _tokenizer;

    private Token? _expressionOperator;

    private Token? _leftOperandLiteral;
    private Variable? _leftOperandVariable;
    private Expression? _leftOperandVariableIndexAccess;
    private Token? _rightOperandLiteral;
    private Variable? _rightOperandVariable;
    private Expression? _rightOperandVariableIndexAccess;

    public Expression(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
        _currentFunction = tokenizer.CurrentFunction ?? throw new SyntaxException("Expression cannot be outside of function call");
    }

    public DataType ExpressionType { get; private set; }

    public string Evaluate(bool force)
    {
        return ExpressionType switch
        {
            DataType.Int => EvaluateIntegerExpression(),
            DataType.Float => EvaluateIntegerExpression(),
            DataType.Bool => EvaluateBoolExpression(),
            DataType.Char => EvaluateCharExpression(),
            _ => throw new LogicalException("Cannot evaluate unknown expression")
        };
    }

    public void AddLeftOperand(Token operand)
    {
        switch (operand)
        {
            case { Type: TokenType.Identifier }:
                _leftOperandVariable = _tokenizer.SymbolTable.GetVariable(operand.Value, _currentFunction.Name);
                SetExpressionType();
                break;

            case { Type: TokenType.BooleanLiteral } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.IntLiteral }:
                _leftOperandLiteral = operand;
                SetExpressionType();
                break;
        }
    }

    public void AddLeftOperandIndexAccess(Expression expression)
    {
        if (_leftOperandVariable is null) throw new SyntaxException("Cannot access index on a literal");

        if (expression is not { ExpressionType: DataType.Int }) throw new TypeException($"""Cannot access array index of variable "{_leftOperandVariable.Name}" using non-int""");

        _leftOperandVariableIndexAccess = expression;
    }

    public void AddRightOperand(Token operand)
    {
        switch (operand)
        {
            case { Type: TokenType.Identifier }:
                _rightOperandVariable = _tokenizer.SymbolTable.GetVariable(operand.Value, _currentFunction.Name);
                VerifyExpressionType();
                break;

            case { Type: TokenType.BooleanLiteral } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.IntLiteral }:
                _rightOperandLiteral = operand;
                VerifyExpressionType();
                break;
        }
    }

    public void AddRightOperandIndexAccess(Expression expression)
    {
        if (_rightOperandVariable is null) throw new SyntaxException("Cannot access index on a literal");

        if (expression is not { ExpressionType: DataType.Int }) throw new TypeException($"""Cannot access array index of variable "{_rightOperandVariable.Name}" using non-int""");

        _rightOperandVariableIndexAccess = expression;
    }

    public void AddOperator(Token operation)
    {
        switch (ExpressionType)
        {
            case DataType.Int when operation.Type is TokenType.NumericOperator:
            case DataType.Float when operation.Type is TokenType.NumericOperator:
            case DataType.Bool when operation.Type is TokenType.BooleanOperator:
                _expressionOperator = operation;
                break;
            case DataType.Char:
                throw new LogicalException("""Cannot perform operation on "char" data type""");
            default:
                throw new LogicalException($"""Cannot perform operation "{operation.Value}" on unknown or unsupported type expression""");
        }
    }

    private void SetExpressionType()
    {
        if (_leftOperandVariable is not null)
        {
            ExpressionType = _leftOperandVariable.DataType;
            return;
        }

        // ReSharper disable once InvertIf
        if (_leftOperandLiteral is not null)
        {
            ExpressionType = GetExpressionTypeBasedOnLiteralType(_leftOperandLiteral);
            return;
        }

        throw new LogicalException("Unexpected left hand side operand");
    }

    private void VerifyExpressionType()
    {
        if (_rightOperandVariable is not null && ExpressionType == _rightOperandVariable.DataType) return;

        if (_rightOperandLiteral is not null && ExpressionType == GetExpressionTypeBasedOnLiteralType(_rightOperandLiteral)) return;

        throw new TypeException("Left and right operands of expression must be of same type");
    }

    private string EvaluateCharExpression()
    {
        return EvaluateLeftOperand();
    }

    private string EvaluateBoolExpression()
    {
        string leftValue = EvaluateLeftOperand();

        if (_expressionOperator is null) return leftValue;

        string rightValue = EvaluateRightOperand();

        bool leftBoolValueParsed = bool.TryParse(leftValue, out bool leftBoolValue);
        bool rightBoolValueParsed = bool.TryParse(rightValue, out bool rightBoolValue);

        if (leftBoolValueParsed is false || rightBoolValueParsed is false) throw new LogicalException($"""Invalid boolean operands "{leftValue}" and "{rightValue}".""");

        return _expressionOperator.Value switch
        {
            "||" => leftBoolValue || rightBoolValue ? "true" : "false",
            "&&" => leftBoolValue && rightBoolValue ? "true" : "false",
            _ => throw new LogicalException("Invalid or null operation on boolean operands")
        };
    }

    private string EvaluateIntegerExpression()
    {
        string leftValue = EvaluateLeftOperand();

        if (_expressionOperator is null)
        {
            bool parsed = short.TryParse(leftValue, out short parsedValue);
            if (parsed is false) throw new IntegerOverflowException($"Integer {leftValue} is too large or small");

            return parsedValue.ToString();
        }

        string rightValue = EvaluateRightOperand();

        bool leftNumberValueParsed = short.TryParse(leftValue, out short leftNumberValue);
        bool rightNumberValueParsed = short.TryParse(rightValue, out short rightNumberValue);

        if (leftNumberValueParsed is false) throw new IntegerOverflowException($"Integer {leftValue} is too large or small");

        if (rightNumberValueParsed is false) throw new IntegerOverflowException($"Integer {rightValue} is too large or small");

        try
        {
            return _expressionOperator.Value switch
            {
                "+" => checked((short)(leftNumberValue + rightNumberValue)).ToString(),
                "-" => checked((short)(leftNumberValue - rightNumberValue)).ToString(),
                "*" => checked((short)(leftNumberValue * rightNumberValue)).ToString(),
                "/" => checked((short)(leftNumberValue / rightNumberValue)).ToString(),
                "%" => checked((short)(leftNumberValue % rightNumberValue)).ToString(),
                _ => throw new LogicalException("Invalid or null operation on boolean operands")
            };
        }
        catch (OverflowException)
        {
            throw new IntegerOverflowException($"Expression {leftValue} {_expressionOperator.Value} {rightValue} is too large or small");
        }
    }

    private string EvaluateLeftOperand()
    {
        if (_leftOperandLiteral is not null) return _leftOperandLiteral.Value;

        // ReSharper disable once InvertIf
        if (_leftOperandVariable is not null)
        {
            if (_leftOperandVariableIndexAccess is not null) _leftOperandVariable.SetIndexAccessExpression(_leftOperandVariableIndexAccess);

            return _leftOperandVariable.Evaluate();
        }

        throw new LogicalException("Cannot evaluate undetermined left hand operand");
    }

    private string EvaluateRightOperand()
    {
        if (_rightOperandLiteral is not null) return _rightOperandLiteral.Value;

        // ReSharper disable once InvertIf
        if (_rightOperandVariable is not null)
        {
            if (_rightOperandVariableIndexAccess is not null) _rightOperandVariable.SetIndexAccessExpression(_rightOperandVariableIndexAccess);

            return _rightOperandVariable.Evaluate();
        }

        throw new LogicalException("Cannot evaluate undetermined right hand operand");
    }

    private static DataType GetExpressionTypeBasedOnLiteralType(Token literal)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return literal.Type switch
        {
            TokenType.IntLiteral => DataType.Int,
            TokenType.FloatLiteral => DataType.Float,
            TokenType.BooleanLiteral => DataType.Bool,
            TokenType.CharacterLiteral => DataType.Char,
            _ => throw new LogicalException("Invalid expression left side literal data type")
        };
    }
}