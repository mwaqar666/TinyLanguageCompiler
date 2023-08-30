using System.Reflection;

namespace TinyLanguageCompiler;

public class Loader
{
    private readonly Dictionary<int, string> _filesDictionary = new();

    public void LoadTinyPrograms()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string projectName = Path.GetFileNameWithoutExtension(executingAssembly.ManifestModule.Name);
        string projectPath = executingAssembly.Location;

        while (!projectPath.EndsWith(projectName))
        {
            string? parentFolder = Path.GetDirectoryName(projectPath);
            if (parentFolder == null) return;

            projectPath = parentFolder;
        }

        string tinyProgramsFilePath = Path.Combine(projectPath, "resources", "tiny");
        string[] files = Directory.GetFiles(tinyProgramsFilePath, "*.tiny", SearchOption.TopDirectoryOnly);

        for (int counter = 0; counter < files.Length; counter++) _filesDictionary.Add(counter, files[counter]);
    }

    public void DisplayAvailablePrograms()
    {
        foreach (KeyValuePair<int, string> fileEntry in _filesDictionary) Console.WriteLine($"File {fileEntry.Key + 1} -> {fileEntry.Value}");
    }

    public string ChooseProgram()
    {
        Console.Write("Enter the file you want to parse: ");
        string? fileNumberInput = Console.ReadLine();
        if (fileNumberInput == null) return string.Empty;

        int fileIndex = int.Parse(fileNumberInput) - 1;
        return File.ReadAllText(_filesDictionary[fileIndex]);
    }
}