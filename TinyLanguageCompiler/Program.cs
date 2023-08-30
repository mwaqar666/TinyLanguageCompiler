using TinyLanguageCompiler.Compiler;

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
    }
}
