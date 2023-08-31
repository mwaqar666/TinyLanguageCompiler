namespace TinyLanguageCompiler.Exceptions;

public class IntegerOverflowException : Exception
{
    public IntegerOverflowException(string message) : base($"Integer Overflow: {message}")
    {
        MessageBox.Show($"Integer Overflow: {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}