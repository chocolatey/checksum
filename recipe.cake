#load nuget:?package=Chocolatey.Cake.Recipe&version=0.20.1

///////////////////////////////////////////////////////////////////////////////
// SCRIPT
///////////////////////////////////////////////////////////////////////////////

Func<FilePathCollection> getFilesToSign = () =>
{
    var filesToSign = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/tools/app/checksum.exe");

    Information("The following assemblies have been selected to be signed...");
    foreach (var fileToSign in filesToSign)
    {
        Information(fileToSign.FullPath);
    }

    return filesToSign;
};

///////////////////////////////////////////////////////////////////////////////
// CUSTOM TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Prepare-Chocolatey-Packages")
    .IsDependeeOf("Create-Chocolatey-Packages")
    .IsDependeeOf("Sign-PowerShellScripts")
    .IsDependeeOf("Sign-Assemblies")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping because not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunChocolatey, "Skipping because execution of Chocolatey has been disabled")
    .Does(() =>
{
    CleanDirectory(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/tools/app");

    // Copy legal documents
    CopyFile(BuildParameters.RootDirectoryPath + "/LICENSE", BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/legal/LICENSE.txt");
    

    // Copy main file
    CopyFile(BuildParameters.Paths.Directories.PublishedApplications + "/checksum/checksum.exe", BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/tools/app/checksum.exe");
});

///////////////////////////////////////////////////////////////////////////////
// RECIPE SCRIPT
///////////////////////////////////////////////////////////////////////////////

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            solutionFilePath: "./src/checksum.sln",
                            solutionDirectoryPath: "./src/checksum",
                            resharperSettingsFileName: "checksum.sln.DotSettings",
                            title: "Checksum",
                            repositoryOwner: "chocolatey",
                            repositoryName: "checksum",
                            productName: "Checksum",
                            productDescription: "chocolatey is a product of Chocolatey Software, Inc. - All Rights Reserved.",
                            productCopyright: string.Format("Copyright © 2017 - {0} Chocolatey Software, Inc. Copyright © 2011 - 2017, RealDimensions Software, LLC - All Rights Reserved.", DateTime.Now.Year),
                            shouldStrongNameSignDependentAssemblies: false,
                            treatWarningsAsErrors: false,
                            shouldPublishAwsLambdas: false,
                            getFilesToSign: getFilesToSign);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.PrintParameters(Context);

Build.Run();