using TinyLanguageCompiler.Compiler;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;

namespace TinyLanguageCompiler.Models;

public class Expression
{
    private readonly Function _currentFunction;
    private readonly Tokenizer _tokenizer;
    private BasicExpressionType _expressionType = BasicExpressionType.Unknown;

    private Token? _leftOperandLiteral;
    private ExpressionOperandType _leftOperandType = ExpressionOperandType.Unknown;
    private Variable? _leftOperandVariable;

    private Token? _rightOperandLiteral;
    private ExpressionOperandType _rightOperandType = ExpressionOperandType.Unknown;
    private Variable? _rightOperandVariable;

    private Token? _expressionOperator;

    public Expression(Tokenizer tokenizer, Function functionContext)
    {
        _tokenizer = tokenizer;
        _currentFunction = functionContext;
    }

    public void AddLeftOperand(Token operand)
    {
        switch (operand)
        {
            case { Type: TokenType.Identifier }:
                _leftOperandType = ExpressionOperandType.Variable;
                Variable variable = _tokenizer.SymbolTable.GetVariable(operand.Value, _currentFunction.Name);
                if (variable.HasNullValue()) throw new LogicalException($"""Variable "{variable.NameWithoutScope}" used in expression is null""");
                _leftOperandVariable = variable;
                break;

            case { Type: TokenType.BooleanLiteral } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.NumberLiteral }:
                _leftOperandType = ExpressionOperandType.Literal;
                _leftOperandLiteral = operand;
                break;
        }

        SetExpressionType();
    }

    public void AddRightOperand(Token operand)
    {
        switch (operand)
        {
            case { Type: TokenType.Identifier }:
                _rightOperandType = ExpressionOperandType.Variable;
                Variable variable = _tokenizer.SymbolTable.GetVariable(operand.Value, _currentFunction.Name);
                if (variable.HasNullValue()) throw new LogicalException($"""Variable "{variable.NameWithoutScope}" used in expression is null""");
                _rightOperandVariable = variable;
                break;

            case { Type: TokenType.BooleanLiteral } or { Type: TokenType.CharacterLiteral } or { Type: TokenType.NumberLiteral }:
                _rightOperandType = ExpressionOperandType.Literal;
                _rightOperandLiteral = operand;
                break;
        }

        VerifyExpressionType();
    }

    public void AddOperator(Token operation)
    {
        switch (_expressionType)
        {
            case BasicExpressionType.IntExpression when operation.Type is TokenType.NumberOperator:
            case BasicExpressionType.FloatExpression when operation.Type is TokenType.NumberOperator:
            case BasicExpressionType.BoolExpression when operation.Type is TokenType.BooleanOperator:
                _expressionOperator = operation;
                break;
            case BasicExpressionType.CharExpression:
                throw new LogicalException("""Cannot perform operation on "char" data type""");
            case BasicExpressionType.Unknown:
            default:
                throw new LogicalException($"""Cannot perform operation "{operation.Value}" on unknown or unsupported type expression""");
        }
    }

    public string EvaluateExpression()
    {
        return _expressionType switch
        {
            BasicExpressionType.CharExpression => EvaluateCharExpression(),
            BasicExpressionType.BoolExpression => EvaluateBoolExpression(),
            BasicExpressionType.IntExpression => EvaluateNumberExpression(),
            BasicExpressionType.FloatExpression => EvaluateNumberExpression(),
            BasicExpressionType.Unknown => throw new LogicalException("Cannot evaluate unknown expression"),
            _ => throw new LogicalException("Cannot evaluate unknown expression"),
        };
    }

    private void SetExpressionType()
    {
        switch (_leftOperandType)
        {
            case ExpressionOperandType.Unknown:
                throw new LogicalException("Unknown left side operand in expression");

            case ExpressionOperandType.Variable:
                if (_leftOperandVariable is null) throw new LogicalException("Unexpected left hand side variable");
                SetExpressionTypeBasedOnVariableType(_leftOperandVariable);
                break;

            case ExpressionOperandType.Literal:
                if (_leftOperandLiteral is null) throw new LogicalException("Unexpected left hand side literal");
                SetExpressionTypeBasedOnLiteralType(_leftOperandLiteral);
                break;

            default:
                throw new LogicalException("Unexpected left hand side operand");
        }
    }

    private void VerifyExpressionType()
    {
        switch (_rightOperandType)
        {
            case ExpressionOperandType.Unknown:
                throw new LogicalException("Unknown right side operand in expression");

            case ExpressionOperandType.Variable:
                if (_rightOperandVariable is null) throw new LogicalException("Unexpected right hand side variable");
                VerifyExpressionTypeBasedOnVariableType(_rightOperandVariable);
                break;

            case ExpressionOperandType.Literal:
                if (_rightOperandLiteral is null) throw new LogicalException("Unexpected right hand side literal");
                VerifyExpressionTypeBasedOnLiteralType(_rightOperandLiteral);
                break;

            default:
                throw new LogicalException("Unexpected right hand side operand");
        }
    }

    private void SetExpressionTypeBasedOnVariableType(Variable variable)
    {
        _expressionType = variable.DataType switch
        {
            DataType.Int => BasicExpressionType.IntExpression,
            DataType.Float => BasicExpressionType.FloatExpression,
            DataType.Bool => BasicExpressionType.BoolExpression,
            DataType.Char => BasicExpressionType.CharExpression,
            _ => throw new LogicalException("Invalid expression left side variable data type")
        };
    }

    private void VerifyExpressionTypeBasedOnVariableType(Variable variable)
    {
        switch (_expressionType)
        {
            case BasicExpressionType.IntExpression when variable.DataType is DataType.Int:
            case BasicExpressionType.FloatExpression when variable.DataType is DataType.Float:
            case BasicExpressionType.BoolExpression when variable.DataType is DataType.Bool:
            case BasicExpressionType.CharExpression when variable.DataType is DataType.Char:
                return;
            case BasicExpressionType.Unknown:
                throw new Exception("Cannot add right operand to an expression before expression type is determined");
            default:
                throw new LogicalException("Left and right operands of expression must be of same type");
        }
    }

    private void SetExpressionTypeBasedOnLiteralType(Token literal)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        _expressionType = literal.Type switch
        {
            TokenType.NumberLiteral => BasicExpressionType.IntExpression,
            TokenType.BooleanLiteral => BasicExpressionType.BoolExpression,
            TokenType.CharacterLiteral => BasicExpressionType.CharExpression,
            _ => throw new LogicalException("Invalid expression left side literal data type")
        };
    }

    private void VerifyExpressionTypeBasedOnLiteralType(Token literal)
    {
        switch (_expressionType)
        {
            case BasicExpressionType.IntExpression when literal.Type is TokenType.NumberLiteral:
            case BasicExpressionType.FloatExpression when literal.Type is TokenType.NumberLiteral:
            case BasicExpressionType.BoolExpression when literal.Type is TokenType.BooleanLiteral:
            case BasicExpressionType.CharExpression when literal.Type is TokenType.CharacterLiteral:
                return;
            case BasicExpressionType.Unknown:
            default:
                throw new Exception("Cannot add right operand to an expression before expression type is determined");
        }
    }

    private string EvaluateCharExpression()
    {
        return _leftOperandType switch
        {
            ExpressionOperandType.Literal when _leftOperandLiteral is not null => _leftOperandLiteral.Value,
            ExpressionOperandType.Variable when _leftOperandVariable?.Value is not null => _leftOperandVariable.Value,
            ExpressionOperandType.Unknown => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
            _ => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
        };
    }

    private string EvaluateBoolExpression()
    {
        string leftValue = _leftOperandType switch
        {
            ExpressionOperandType.Literal when _leftOperandLiteral is not null => _leftOperandLiteral.Value,
            ExpressionOperandType.Variable when _leftOperandVariable?.Value is not null => _leftOperandVariable.Value,
            ExpressionOperandType.Unknown => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
            _ => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
        };

        string rightValue = _rightOperandType switch
        {
            ExpressionOperandType.Literal when _rightOperandLiteral is not null => _rightOperandLiteral.Value,
            ExpressionOperandType.Variable when _rightOperandVariable?.Value is not null => _rightOperandVariable.Value,
            ExpressionOperandType.Unknown => throw new LogicalException("Cannot evaluate undetermined right hand operand"),
            _ => throw new LogicalException("Cannot evaluate undetermined right hand operand"),
        };

        bool leftBoolValueParsed = bool.TryParse(leftValue, out bool leftBoolValue);
        bool rightBoolValueParsed = bool.TryParse(rightValue, out bool rightBoolValue);

        if (leftBoolValueParsed is false || rightBoolValueParsed is false)
        {
            throw new LogicalException($"""Invalid boolean operands "{leftValue}" and "{rightValue}".""");
        }

        return _expressionOperator?.Value switch
        {
            "||" => leftBoolValue || rightBoolValue ? "true" : "false",
            "&&" => leftBoolValue && rightBoolValue ? "true" : "false",
            _ => throw new LogicalException("Invalid or null operation on boolean operands")
        };
    }

    private string EvaluateNumberExpression()
    {
        string leftValue = _leftOperandType switch
        {
            ExpressionOperandType.Literal when _leftOperandLiteral is not null => _leftOperandLiteral.Value,
            ExpressionOperandType.Variable when _leftOperandVariable?.Value is not null => _leftOperandVariable.Value,
            ExpressionOperandType.Unknown => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
            _ => throw new LogicalException("Cannot evaluate undetermined left hand operand"),
        };

        string rightValue = _rightOperandType switch
        {
            ExpressionOperandType.Literal when _rightOperandLiteral is not null => _rightOperandLiteral.Value,
            ExpressionOperandType.Variable when _rightOperandVariable?.Value is not null => _rightOperandVariable.Value,
            ExpressionOperandType.Unknown => throw new LogicalException("Cannot evaluate undetermined right hand operand"),
            _ => throw new LogicalException("Cannot evaluate undetermined right hand operand"),
        };

        bool leftNumberValueParsed = short.TryParse(leftValue, out short leftNumberValue);
        bool rightNumberValueParsed = short.TryParse(rightValue, out short rightNumberValue);

        if (leftNumberValueParsed is false || rightNumberValueParsed is false)
        {
            throw new LogicalException($"""Integer overflow: Number "${leftValue}" or "${rightValue}" is too large or small""");
        }

        try
        {
            return _expressionOperator?.Value switch
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
            throw new LogicalException($"""Integer overflow: "${leftValue}" "{_expressionOperator?.Value}" "${rightValue}" is too large or small""");
        }
    }
}
