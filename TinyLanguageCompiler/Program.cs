using TinyLanguageCompiler.Compiler;
using TinyLanguageCompiler.Compiler.Tokenizer;

namespace TinyLanguageCompiler;

public static class Program
{
    public static void Main()
    {
        Loader loader = new();
        loader.LoadTinyPrograms();
        loader.DisplayAvailablePrograms();
        string programContents = loader.ChooseProgram();

        SymbolTable symbolTable = new();

        Tokenizer tokenizer = new(programContents, symbolTable);
        tokenizer.PreProcess().Tokenize().ParseProgram();

        Console.Write("""
                      Provided program is valid ✅
                      Check for Buffer and Integer Overflow? ([y] / n):
                      """);

        string? shouldRunProgram = Console.ReadLine();

        if (shouldRunProgram is null or not ("" or "y")) return;

        Simulator simulator = new(symbolTable);
        simulator.RunProgram();

        Console.Write("""
                      Buffer and Integer Overflow checks passed ✅
                      Exiting
                      """);
    }
}
