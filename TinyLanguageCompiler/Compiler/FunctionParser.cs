using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class FunctionParser
{
    private readonly Tokenizer _tokenizer;
    private Function? _currentFunction;

    public FunctionParser(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public void ParseFunction()
    {
        Token functionDataType = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionDataType is not { Type: TokenType.DataTypeKeyword });

        Token functionIdentifier = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionIdentifier is not { Type: TokenType.Identifier });

        Token functionParamBracketStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionParamBracketStart is not { Type: TokenType.Delimiter, Value: "(" });

        List<Variable> parameters = new();
        ParseFunctionParameters(functionIdentifier, ref parameters);

        Token functionParamBracketEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionParamBracketEnd is not { Type: TokenType.Delimiter, Value: ")" });

        try
        {
            _currentFunction = _tokenizer.SymbolTable.AddFunction(functionIdentifier.Value, Tokenizer.GuessDataType(functionDataType), parameters);
        }
        catch (ArgumentException)
        {
            throw new LogicalException($"""Function "{functionIdentifier.Value}' already exists""");
        }

        Token functionBodyBracketStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionBodyBracketStart is not { Type: TokenType.Delimiter, Value: "{" });

        ParseFunctionDeclarations();

        ParseFunctionStatements();

        Token functionBodyBracketEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionBodyBracketEnd is not { Type: TokenType.Delimiter, Value: "}" });
    }

    private void ParseFunctionParameters(Token functionIdentifier, ref List<Variable> parameters)
    {
        Token functionParamBracketEnd = _tokenizer.NextToken();
        if (functionParamBracketEnd is { Type: TokenType.Delimiter, Value: ")" })
        {
            _tokenizer.PreviousToken();
            return;
        }

        _tokenizer.PreviousToken();

        while (true)
        {
            parameters.Add(ParseFunctionParameter(functionIdentifier));

            Token parameterDelimiter = _tokenizer.NextToken();
            if (parameterDelimiter is { Type: TokenType.Delimiter, Value: "," }) continue;

            _tokenizer.PreviousToken();
            break;
        }
    }

    private Variable ParseFunctionParameter(Token functionIdentifier)
    {
        Token parameterDataType = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(parameterDataType is not { Type: TokenType.DataTypeKeyword });

        Token parameterIdentifier = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(parameterIdentifier is not { Type: TokenType.Identifier });

        return _tokenizer.SymbolTable.AddVariable(parameterIdentifier.Value, Tokenizer.GuessDataType(parameterDataType), functionIdentifier.Value);
    }

    private void ParseFunctionDeclarations()
    {
        Token variableDataType = _tokenizer.NextToken();
        if (variableDataType is not { Type: TokenType.DataTypeKeyword })
        {
            _tokenizer.PreviousToken();
            return;
        }

        _tokenizer.PreviousToken();

        while (true)
        {
            ParseFunctionDeclaration();

            Token nextDeclarationDataType = _tokenizer.NextToken();

            if (nextDeclarationDataType is not { Type: TokenType.DataTypeKeyword })
            {
                _tokenizer.PreviousToken();
                return;
            }

            _tokenizer.PreviousToken();
        }
    }

    private void ParseFunctionDeclaration()
    {
        Token variableDataType = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(variableDataType is not { Type: TokenType.DataTypeKeyword });

        ParseEachDelimitedDeclaration(variableDataType);

        Token terminator = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(terminator is not { Type: TokenType.Terminator, Value: ";" });
    }

    private void ParseEachDelimitedDeclaration(Token variableDataType)
    {
        while (true)
        {
            if (_currentFunction is null) throw new Exception("Function context missing!");

            Token variableIdentifier = _tokenizer.NextToken();
            ExceptionFactory.CreateSyntaxExceptionIf(variableIdentifier is not { Type: TokenType.Identifier });

            Token arrayLiteralStart = _tokenizer.NextToken();

            try
            {
                if (arrayLiteralStart is not { Type: TokenType.Delimiter, Value: "[" })
                {
                    _tokenizer.PreviousToken();
                    _tokenizer.SymbolTable.AddVariable(variableIdentifier.Value, Tokenizer.GuessDataType(variableDataType), _currentFunction.Name);
                }
                else
                {
                    Token arraySize = _tokenizer.NextToken();
                    ExceptionFactory.CreateSyntaxExceptionIf(arraySize is not { Type: TokenType.NumberLiteral });

                    Token arrayDelimiterEnd = _tokenizer.NextToken();
                    ExceptionFactory.CreateSyntaxExceptionIf(arrayDelimiterEnd is not { Type: TokenType.Delimiter, Value: "]" });

                    _tokenizer.SymbolTable.AddVariable(variableIdentifier.Value, Tokenizer.GuessDataType(variableDataType), _currentFunction.Name, int.Parse(arraySize.Value));
                }
            }
            catch (ArgumentException)
            {
                throw new LogicalException($"""Variable "{variableIdentifier.Value}" cannot be declared twice""");
            }

            Token terminatorOrNextDeclarationDelimiter = _tokenizer.NextToken();
            if (terminatorOrNextDeclarationDelimiter is { Type: TokenType.Delimiter, Value: "," }) continue;

            _tokenizer.PreviousToken();
            break;
        }
    }

    private void ParseFunctionStatements()
    {
        Token functionBodyBracketEnd = _tokenizer.NextToken();
        if (functionBodyBracketEnd is { Type: TokenType.Delimiter, Value: "}" })
        {
            _tokenizer.PreviousToken();
            return;
        }

        _tokenizer.PreviousToken();

        if (_currentFunction is null) throw new Exception("Function context missing!");

        StatementParser statementParser = new(_tokenizer, _currentFunction);

        statementParser.ParseFunctionStatements();
    }
}
