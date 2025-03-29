namespace UserService.Tests.FunctionalTests.Helpers;

public static class PathHelper
{
    private const string FunctionalTestsDirectoryName = "FunctionalTests";
    private const string ConfigurationsDirectoryName = "Configurations";

    public static string GetPathToConfiguration(DirectoryInfo runtimeDirectory, string currentProjectName,
        string fileName, params string[] configurationSubdirectories)
    {
        var currentProjectDirectory = runtimeDirectory;

        while (currentProjectDirectory.Name != currentProjectName)
            currentProjectDirectory =
                currentProjectDirectory.Parent!; //getting path to current project directory from runtime directory


        var configurationSubdirectoriesPath = Path.Combine(configurationSubdirectories);
        var filePath = Path.Combine(currentProjectDirectory.FullName, FunctionalTestsDirectoryName,
            ConfigurationsDirectoryName, configurationSubdirectoriesPath, fileName);

        return filePath;
    }
}