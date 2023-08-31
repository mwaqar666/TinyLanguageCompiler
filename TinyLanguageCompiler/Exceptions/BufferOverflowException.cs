namespace TinyLanguageCompiler.Exceptions;

public class BufferOverflowException : Exception
{
    public BufferOverflowException(string message) : base($"Buffer Overflow: {message}")
    {
        MessageBox.Show($"Buffer Overflow: {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}
