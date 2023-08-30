using System.Text.RegularExpressions;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class Tokenizer
{
    private readonly List<Token> _tokens = new();
    private string _code;
    private int _currentToken = -1;

    public Tokenizer(string code, SymbolTable symbolTable)
    {
        _code = code;
        SymbolTable = symbolTable;
    }

    public SymbolTable SymbolTable { get; }

    public Tokenizer PreProcess()
    {
        _code = RegexMatcher.MultiLineCommentPattern().Replace(_code, string.Empty);

        _code = RegexMatcher.SingleLineCommentPattern().Replace(_code, string.Empty);

        return this;
    }

    public Tokenizer Tokenize()
    {
        MatchCollection matches = RegexMatcher.TokenizePattern().Matches(_code);

        foreach (Match match in matches)
        {
            string value = match.Value;

            if (value is "int" or "float" or "char" or "bool")
                _tokens.Add(new Token(TokenType.DataTypeKeyword, value));

            else if (value is "return")
                _tokens.Add(new Token(TokenType.ReturnKeyword, value));

            else if (value is "true" or "false")
                _tokens.Add(new Token(TokenType.BooleanLiteral, value));

            else if (value is "while")
                _tokens.Add(new Token(TokenType.WhileLoopKeyword, value));

            else if (value is "if" or "else")
                _tokens.Add(new Token(TokenType.ConditionalKeyword, value));

            else if (value is ";")
                _tokens.Add(new Token(TokenType.Terminator, value));

            else if (value is "+" or "-" or "*" or "/" or "%")
                _tokens.Add(new Token(TokenType.NumberOperator, value));

            else if (value is "||" or "&&")
                _tokens.Add(new Token(TokenType.BooleanOperator, value));

            else if (value is "==" or ">=" or "<=" or ">" or "<")
                _tokens.Add(new Token(TokenType.ComparisonOperator, value));

            else if (value is "=")
                _tokens.Add(new Token(TokenType.AssignmentOperator, value));

            else if (value is "," or "(" or ")" or "{" or "}" or "[" or "]")
                _tokens.Add(new Token(TokenType.Delimiter, value));

            else if (value.Length == 3 && value.StartsWith("'") && value.EndsWith("'"))
                _tokens.Add(new Token(TokenType.CharacterLiteral, value));

            else if (RegexMatcher.NumberPattern().IsMatch(value))
                _tokens.Add(new Token(TokenType.NumberLiteral, value));

            else if (RegexMatcher.IdentifierPattern().IsMatch(value))
                _tokens.Add(new Token(TokenType.Identifier, value));
        }

        return this;
    }

    public void ParseProgram()
    {
        ParseFunctions();
    }

    private void ParseFunctions()
    {
        FunctionParser functionParser = new(this);
        functionParser.ParseFunction();
    }

    public static DataType GuessDataType(Token dataType)
    {
        return dataType.Value switch
        {
            "int" => DataType.Int,
            "float" => DataType.Float,
            "bool" => DataType.Bool,
            "char" => DataType.Char,
            _ => throw new LogicalException($"""Unsupported "{dataType.Value}" data type""")
        };
    }

    public Token NextToken()
    {
        return _tokens[++_currentToken];
    }

    public Token PreviousToken()
    {
        return _tokens[--_currentToken];
    }
}
