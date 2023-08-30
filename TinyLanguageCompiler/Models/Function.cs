using TinyLanguageCompiler.Enums;

namespace TinyLanguageCompiler.Models;

public class Function
{
    public Function(string name, DataType returnType, List<Variable> parameters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
    }

    public string Name { get; }
    public DataType ReturnType { get; }
    public List<Variable> Parameters { get; }
}