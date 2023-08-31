using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Parsers;

public class ConditionParser
{
    private readonly Tokenizer.Tokenizer _tokenizer;

    public ConditionParser(Tokenizer.Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public void ParseConditionStatement()
    {
        _tokenizer.SymbolTable.IncrementCodeBlockLevel();

        Token ifKeyword = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(ifKeyword is not { Type: TokenType.IfKeyword, Value: "if" });

        Token delimiterStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(delimiterStart is not { Type: TokenType.Delimiter, Value: "(" });

        ComparisonParser comparisonParser = new(_tokenizer);
        comparisonParser.ParseComparison();

        Token delimiterEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(delimiterEnd is not { Type: TokenType.Delimiter, Value: ")" });

        Token bodyStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyStart is not { Type: TokenType.Delimiter, Value: "{" });

        ParseConditionBody();

        Token bodyEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyEnd is not { Type: TokenType.Delimiter, Value: "}" });

        Token elseKeyword = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        if (elseKeyword is not { Type: TokenType.ElseKeyword, Value: "else" })
        {
            _tokenizer.SymbolTable.DecrementCodeBlockLevel();
            return;
        }

        ParseElseCodeBlock();
        _tokenizer.SymbolTable.DecrementCodeBlockLevel();
    }

    private void ParseElseCodeBlock()
    {
        Token elseKeyword = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(elseKeyword is not { Type: TokenType.ElseKeyword, Value: "else" });

        Token bodyStart = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyStart is not { Type: TokenType.Delimiter, Value: "{" });

        ParseConditionBody();

        Token bodyEnd = _tokenizer.NextToken();
        ExceptionFactory.CreateSyntaxExceptionIf(bodyEnd is not { Type: TokenType.Delimiter, Value: "}" });
    }

    private void ParseConditionBody()
    {
        Token possibleBodyBracketEnd = _tokenizer.NextToken();
        _tokenizer.PreviousToken();

        if (possibleBodyBracketEnd is { Type: TokenType.Delimiter, Value: "}" }) return;

        StatementParser statementParser = new(_tokenizer);

        while (true)
        {
            statementParser.ParseStatement();

            possibleBodyBracketEnd = _tokenizer.NextToken();
            _tokenizer.PreviousToken();

            if (possibleBodyBracketEnd is not { Type: TokenType.Delimiter, Value: "}" }) continue;

            break;
        }
    }
}