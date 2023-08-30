using TinyLanguageCompiler.Enums;
using TinyLanguageCompiler.Exceptions;
using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class SymbolTable
{
    private readonly Dictionary<string, Function> _functions = new();
    private readonly Dictionary<string, Variable> _variables = new();

    public Variable AddVariable(string variableName, DataType dataType, string functionScope, int arraySize = 0)
    {
        string variableScopedName = $"{functionScope}.{variableName}";
        Variable variable = new(variableScopedName, dataType, arraySize);

        _variables.Add(variableScopedName, variable);

        return variable;
    }

    public Variable GetVariable(string variableName, string functionScope)
    {
        string variableScopedName = $"{functionScope}.{variableName}";

        try
        {
            return _variables[variableScopedName];
        }
        catch (KeyNotFoundException)
        {
            throw new LogicalException($"""Variable "{variableName}" is accessed without initializing first""");
        }
    }

    public Function AddFunction(string functionName, DataType returnType, List<Variable> parameters)
    {
        Function function = new(functionName, returnType, parameters);
        _functions.Add(functionName, function);

        return function;
    }

    public Function GetFunction(string functionName)
    {
        try
        {
            return _functions[functionName];
        }
        catch (KeyNotFoundException)
        {
            throw new LogicalException($"""Function "{functionName}" is accessed without declaration""");
        }
    }
}