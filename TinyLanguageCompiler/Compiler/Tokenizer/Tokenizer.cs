using System.Text.RegularExpressions;
using TinyLanguageCompiler.Compiler.Parsers;
using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler.Tokenizer;

public class Tokenizer
{
    private readonly FunctionParser _functionParser;
    private readonly List<Token> _tokens = new();
    private string _code;
    private int _currentToken = -1;

    public Tokenizer(string code, SymbolTable symbolTable)
    {
        _code = code;
        SymbolTable = symbolTable;
        _functionParser = new FunctionParser(this);
    }


    public Function? CurrentFunction { get; set; }

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

            switch (value)
            {
                case "int" or "float" or "bool" or "char":
                    _tokens.Add(new Token(TokenType.DataTypeKeyword, value));
                    break;
                case "true" or "false":
                    _tokens.Add(new Token(TokenType.BooleanLiteral, value));
                    break;
                case "return":
                    _tokens.Add(new Token(TokenType.ReturnKeyword, value));
                    break;
                case "if":
                    _tokens.Add(new Token(TokenType.IfKeyword, value));
                    break;
                case "else":
                    _tokens.Add(new Token(TokenType.ElseKeyword, value));
                    break;
                case "while":
                    _tokens.Add(new Token(TokenType.WhileKeyword, value));
                    break;
                case ";":
                    _tokens.Add(new Token(TokenType.Terminator, value));
                    break;
                case "+" or "-" or "*" or "/" or "%":
                    _tokens.Add(new Token(TokenType.NumericOperator, value));
                    break;
                case "!=" or "==" or ">=" or "<=" or ">" or "<":
                    _tokens.Add(new Token(TokenType.ComparisonOperator, value));
                    break;
                case "||" or "&&":
                    _tokens.Add(new Token(TokenType.BooleanOperator, value));
                    break;
                case "=":
                    _tokens.Add(new Token(TokenType.AssignmentOperator, value));
                    break;
                case "," or "(" or ")" or "{" or "}" or "[" or "]":
                    _tokens.Add(new Token(TokenType.Delimiter, value));
                    break;
                default:
                {
                    if (value.Length == 3 && value.StartsWith("'") && value.EndsWith("'"))
                        _tokens.Add(new Token(TokenType.CharacterLiteral, value));

                    else if (RegexMatcher.FloatPattern().IsMatch(value))
                        _tokens.Add(new Token(TokenType.FloatLiteral, value));

                    else if (RegexMatcher.IntPattern().IsMatch(value))
                        _tokens.Add(new Token(TokenType.IntLiteral, value));

                    else if (RegexMatcher.IdentifierPattern().IsMatch(value))
                        _tokens.Add(new Token(TokenType.Identifier, value));

                    break;
                }
            }
        }

        return this;
    }

    public void ParseProgram()
    {
        ParseFunctions();
    }

    private void ParseFunctions()
    {
        _functionParser.ParseFunction();
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
