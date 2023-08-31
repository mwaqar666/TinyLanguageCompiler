namespace TinyLanguageCompiler.Exceptions;

public class LogicalException : Exception
{
    public LogicalException(string message) : base(message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}