namespace TinyLanguageCompiler.Exceptions;

public class SyntaxException : Exception
{
    public SyntaxException(string message) : base(message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}