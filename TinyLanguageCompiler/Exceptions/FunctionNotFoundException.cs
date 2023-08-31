namespace TinyLanguageCompiler.Exceptions;

public class FunctionNotFoundException : Exception
{
    public FunctionNotFoundException(string message) : base($"Function not found: {message}")
    {
        MessageBox.Show($"Function not found: {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}