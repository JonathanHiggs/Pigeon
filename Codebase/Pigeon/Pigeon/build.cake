///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


///////////////////////////////////////////////////////////////////////////////
// 
///////////////////////////////////////////////////////////////////////////////

var projectFile = Argument("projectFile", @".\Pigeon.csproj");
var packageName = Argument("packageName", "Pigeon");
var outputDir = Directory(@".\bin");
var nugetDeployDir = Directory(@"\\pihub\packages");


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() => {
		Information("Cleaning: {0}", outputDir);
		CleanDirectory(outputDir);
	});


Task("Restore")
	.IsDependentOn("Clean")
	.Does(() => {
		NuGetRestore(projectFile);
	});


Task("Build")
	.IsDependentOn("Restore")
	.Does(() => {
		MSBuild(projectFile, settings => settings
			.SetConfiguration(configuration)
			.SetVerbosity(Verbosity.Minimal)
		);

		var assemblyInfoFile = GetFiles("./**/*AssemblyInfo.cs").First();
		var assemblyInfo = ParseAssemblyInfo(assemblyInfoFile);

		Information("Version:               {0}", assemblyInfo.AssemblyVersion);
		Information("File version:          {0}", assemblyInfo.AssemblyFileVersion);
		Information("Informational version: {0}", assemblyInfo.AssemblyInformationalVersion);
	});


Task("NugetAdd")
	.IsDependentOn("Build")
	.Does(() => {
		var packageFile = GetFiles("./**/*.nupkg").Select(f => f.ToString()).First();
		Information(packageFile);

		NuGetAdd(packageFile, nugetDeployDir);
	});


Task("Default")
	.IsDependentOn("NugetAdd");


RunTarget(target);