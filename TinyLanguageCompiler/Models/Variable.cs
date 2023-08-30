using TinyLanguageCompiler.Enums;

namespace TinyLanguageCompiler.Models;

public class Variable
{
    public Variable(string name, DataType dataType, int arraySize = 0)
    {
        Name = name;
        DataType = dataType;
        ArraySize = arraySize;
    }

    public string Name { get; }

    public string NameWithoutScope => Name.Split('.')[1];

    public DataType DataType { get; }
    public int ArraySize { get; }
    public string? Value { get; }

    public bool HasNullValue()
    {
        return Value is null;
    }
}
