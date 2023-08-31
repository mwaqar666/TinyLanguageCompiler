namespace TinyLanguageCompiler.Exceptions;

public class TypeException : Exception
{
    public TypeException(string message) : base($"Type mismatch exception: {message}")
    {
        MessageBox.Show($"Type mismatch exception: {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
    }
}
