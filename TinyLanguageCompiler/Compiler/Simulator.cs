using TinyLanguageCompiler.Models;

namespace TinyLanguageCompiler.Compiler;

public class Simulator
{
    private readonly SymbolTable _symbolTable;

    public Simulator(SymbolTable symbolTable)
    {
        _symbolTable = symbolTable;
    }

    public void RunProgram()
    {
        Function mainFunction = _symbolTable.GetFunction("main");

        mainFunction.Evaluate(true);
    }
}
