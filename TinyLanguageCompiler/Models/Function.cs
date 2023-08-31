using TinyLanguageCompiler.Contracts;
using TinyLanguageCompiler.Enums;

namespace TinyLanguageCompiler.Models;

public class Function : IEvaluable
{
    public Function(string name, DataType returnType)
    {
        Name = name;
        ReturnType = returnType;
        Statements = new List<IStatement>();
    }

    public string Name { get; }

    public DataType ReturnType { get; }

    public List<IStatement> Statements { get; }

    public string Evaluate(bool force)
    {
        foreach (IStatement statement in Statements) statement.Run();

        return string.Empty;
    }

    public void AddStatement(IStatement statement)
    {
        Statements.Add(statement);
    }
}