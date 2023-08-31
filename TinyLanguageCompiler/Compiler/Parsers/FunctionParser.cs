using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class FunctionParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public FunctionParser(Tokenizer.Tokenizer tokenizer)
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

        Token functionParamBracketEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionParamBracketEnd is not { Type: TokenType.Delimiter, Value: ")" });

        try
        {
            DataType returnType = Tokenizer.Tokenizer.GuessDataType(functionDataType);

            if (functionIdentifier.Value is "main" && returnType is not DataType.Int)
            {
                throw new LogicalException("""Return type of "main" function must always be integer!""");
            }

            _tokenizer.CurrentFunction = _tokenizer.SymbolTable.AddFunction(functionIdentifier.Value, returnType);
        }
        catch (ArgumentException)
        {
            throw new LogicalException($"""Function "{functionIdentifier.Value}' already exists""");
        }

        Token functionBodyBracketStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionBodyBracketStart is not { Type: TokenType.Delimiter, Value: "{" });

        ParseFunctionDeclarations();

        ParseFunctionStatements(_tokenizer.CurrentFunction);

        Token functionBodyBracketEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(functionBodyBracketEnd is not { Type: TokenType.Delimiter, Value: "}" });
    }

    private void ParseFunctionDeclarations()
    {
        Token variableDataType = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        if (variableDataType is not { Type: TokenType.DataTypeKeyword }) return;

        while (true)
        {
            ParseFunctionDeclaration();

            Token nextDeclarationDataType = _tokenizer.NextToken();
            _tokenizer.PreviousToken();

            if (nextDeclarationDataType is not { Type: TokenType.DataTypeKeyword }) return;
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
            if (_tokenizer.CurrentFunction is null) throw new Exception("Function context missing!");

            Token variableIdentifier = _tokenizer.NextToken();
            ExceptionFactory.CreateSyntaxExceptionIf(variableIdentifier is not { Type: TokenType.Identifier });

            Token arrayLiteralStart = _tokenizer.NextToken();

            try
            {
                if (arrayLiteralStart is not { Type: TokenType.Delimiter, Value: "[" })
                {
                    _tokenizer.PreviousToken();
                    _tokenizer.SymbolTable.AddVariable(variableIdentifier.Value, Tokenizer.Tokenizer.GuessDataType(variableDataType), _tokenizer.CurrentFunction.Name);
                }
                else
                {
                    Token arraySize = _tokenizer.NextToken();
                    ExceptionFactory.CreateSyntaxExceptionIf(arraySize is not { Type: TokenType.IntLiteral });

                    Token arrayDelimiterEnd = _tokenizer.NextToken();
                    ExceptionFactory.CreateSyntaxExceptionIf(arrayDelimiterEnd is not { Type: TokenType.Delimiter, Value: "]" });

                    _tokenizer.SymbolTable.AddVariable(variableIdentifier.Value, Tokenizer.Tokenizer.GuessDataType(variableDataType), _tokenizer.CurrentFunction.Name, int.Parse(arraySize.Value));
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

    private void ParseFunctionStatements(Function function)
    {
        Token functionBodyBracketEnd = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        if (functionBodyBracketEnd is { Type: TokenType.Delimiter, Value: "}" }) return;

        StatementParser statementParser = new(_tokenizer);

        while (true)
        {
            IStatement? statement = statementParser.ParseStatement();

            if (statement is not null && _tokenizer.SymbolTable.CodeBlockLevel is 0) function.AddStatement(statement);

            functionBodyBracketEnd = _tokenizer.NextToken();
            _tokenizer.PreviousToken();

            if (functionBodyBracketEnd is not { Type: TokenType.Delimiter, Value: "}" }) continue;

            break;
        }
    }
}
