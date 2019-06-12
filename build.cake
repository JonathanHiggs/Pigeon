/////////////////////////
// Includes & Using
///////////////////////

// versioning //
#addin nuget:?package=semver&version=2.0.4
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

// cake addins //
#addin nuget:?package=Cake.Email.Common
#addin nuget:?package=Cake.Json
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.SemVer&version=3.0.0


/////////////////////////
// Arguments
///////////////////////

var target          = Argument("target", "Default");
var configuration   = Argument("configuration", "Release");
var solution        = Argument("solution", "Pigeon");

var gitCheck        = Argument<bool>("git-check", true);
var commitVersion   = Argument<bool>("commit-version", true);
var createTag       = Argument<bool>("create-tag", true);
var revertOnError   = Argument<bool>("revert-on-error", true);

var description     = "Library to abstracting transport and serialization concerns such that microservices can host multiple endpoints";
var summary         = "Microservice transport and serialization library";
var source          = "";
var uri             = new Uri(@"");
var owner           = "Jonathan Higgs <jonathan.higgs@capstoneco.com>";
var nugetRepo       = "";


/////////////////////////
// Files & Directories
///////////////////////

var root            = GitFindRootFromPath(MakeAbsolute(Directory(".")));
var solutionDir     = Directory($"{root}/Codebase/{solution}");
var solutionFile    = File($"{solutionDir}/{solution}.sln");
var buildDir        = Directory($"{solutionDir}/Build");
var publishDir      = Directory($"{solutionDir}/Publish");
var infoFile        = File($"{solutionDir}/VersionInfo.cs");


/////////////////////////
// Variables
///////////////////////

var projectNames = new List<string> {
    "Pigeon",
    "Pigeon.Json",
    "Pigeon.NetMQ",
    "Pigeon.Unity"
};

Semver.SemVersion semVersion;

var completed = false;
var revertSafe = false;


/////////////////////////
// Setup & Teardown
///////////////////////

Setup(context => {
    if (!DirectoryExists(buildDir)) {
        CreateDirectory(buildDir);
        Information($"Created build directory: {buildDir}");
    }

    if (!DirectoryExists(publishDir)) {
        CreateDirectory(publishDir);
        Information($"Created publish directory: {publishDir}");
    }
});

Teardown(context => {
    // Reset the version number on failure
    if (!completed && revertSafe && revertOnError)
        StartAndReturnProcess("git", new ProcessSettings{ Arguments = $"checkout HEAD {infoFile}" }).WaitForExit();
});


/////////////////////////
// Tasks
///////////////////////

Task("Check")
    .Does(() => {
        // Validate all conditions and report
        var messages = new List<string>();

        if (gitCheck && GitHasStagedChanges(root)) {
            var message = "Error: Git has staged changes, please commit all files before building";
            Error(message);
            messages.Add(message);
        }

        if (gitCheck && GitHasUncommitedChanges(root)) {
            var message = "Error: Git has uncommitted changes, please commit all files before building";
            Error(message);
            messages.Add(message);
        }

        if (gitCheck && GitHasUntrackedFiles(root)) {
            var message = "Error: Git has untracked files, please add and commit or exclude before building";
            Error(message);
            messages.Add(message);
        }

        if (messages.Any())
            throw new Exception(string.Join("\r\n", messages));

        revertSafe = true;
    });


Task("Info")
    .IsDependentOn("Check")
    .Does(() => {
        Information($"Git Root:         {root}");
        Information($"Solution Dir:     {solutionDir}");
        Information($"Solution File:    {solutionFile}");
        Information($"Projects:         {string.Join(", ", projectNames)}");
        Information($"Build Dir:        {buildDir}");
        Information($"Publish Dir:      {publishDir}");
    });


Task("ReadVersion")
    .IsDependentOn("Info")
    .Does(() => {
        var info = ParseAssemblyInfo(infoFile);
        semVersion = ParseSemVer(info.AssemblyVersion);
    });


Task("Clean")
    .IsDependentOn("Info")
    .Does(() => {
        CleanDirectories($"./**/bin/{configuration}");
        CleanDirectories($"./**/obj/{configuration}");
        Information("Clean completed");
    });


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        NuGetRestore(solutionFile);
        Information($"Restore complete: {solutionFile}");
    });


Task("Pack")
    .IsDependentOn("Restore")
    .Does(() => {
        var projects = projectNames.Select(p => File($"{solutionDir}/{p}/{p}.csproj"));

        var msBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", semVersion)
            .WithProperty("AssemblyVersion", semVersion)
            .WithProperty("FileVersion", semVersion);
            //.WithProperty("AssemblyInformationalVersion", semVersion);

        if (!IsRunningOnWindows())
        {
            var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

            Information("Pack will use FrameworkPathOverride={0} since not building on windows", frameworkPathOverride);
            msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
        }

        var settings = new DotNetCorePackSettings {
            NoBuild = true,
            Configuration = configuration,
            OutputDirectory = buildDir,
            MSBuildSettings = msBuildSettings,
            ArgumentCustomization = (args) => {
                args.Append("--include-source");
                return args;
            }
        };

        foreach (var project in projects)
            DotNetCorePack(project.ToString(), settings);
    });


Task("Package")
    .IsDependentOn("Pack")
    .Does(() => {

    });


//Task("Version")
//    .IsDependentOn("")
//    .Does(() => {
//        // Increment version number from solution info file for the next build
//        var info = ParseAssemblyInfo(infoFile);
//        var old = ParseSemVer(info.AssemblyVersion);
//            
//        semVersion = CreateSemVer(old.Major, old.Minor, old.Patch + 1);
//
//        Information($"Version bumped {versionBump} to {semVersion} for the next build");
//        
//        // Update SolutionInfo.cs
//        CreateAssemblyInfo(infoFile, new AssemblyInfoSettings {
//            Configuration = configuration,
//            Copyright = $"Copyright (c) Capstone Investment Advisors {DateTime.Now.Year}",
//            FileVersion = semVersion.ToString(),
//            Version = semVersion.ToString()
//        });
//    });
