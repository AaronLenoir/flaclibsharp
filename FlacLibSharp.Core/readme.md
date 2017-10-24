This project ```FlacLibSharp.Core``` is here to allow FlacLibSharp to be built directly with .NET Core.

The project ```FlacLibSharp``` is used to actually build for release, since it generates the NuGet package and also targets both .NET Framework 2.0 and .NET Standard 1.3.

Because .NET Core on Linux cannot build projects that reference .NET Framework 2.0, I have added this project so that tests can be run on Windows and Linux (and other .NET Core platforms, I assume).