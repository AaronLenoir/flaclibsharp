| Linux (.NET Core) | Windows |
|-------------------|---------|
| [![Linux Build Status](https://travis-ci.org/AaronLenoir/flaclibsharp.svg?branch=master)](https://travis-ci.org/AaronLenoir/flaclibsharp) | [![Windows Build Status](https://ci.appveyor.com/api/projects/status/github/AaronLenoir/flaclibsharp?branch=master&svg=true)](https://ci.appveyor.com/project/AaronLenoir/flaclibsharp) |

# FlacLibSharp

A .NET library for reading and writing FLAC metadata.

### Installation

    PM> Install-Package FlacLibSharp

Or go to the nuget page: https://www.nuget.org/packages/FlacLibSharp

Platform support:

* .NET Standard 1.3

### Warning

If you use this for anything important, make sure your files are backed up. Since this was a hobby project I cannot guarantee anything in terms of quality.

Please evaluate carefully and report any issues you find here on GitHub.

### Usage example

#### Reading Metadata

```csharp
// Access to the StreamInfo class
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
	Console.WriteLine("Flac audio length in seconds: {0}", file.StreamInfo.Duration);
}

// Access to the VorbisComment IF it exists in the file
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
	var vorbisComment = file.VorbisComment;
	if (vorbisComment != null)
	{
		Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist, vorbisComment.Title);
	}
}

// Access to the VorbisComment with multiple values for a single field
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
	var vorbisComment = file.VorbisComment;
	if (vorbisComment != null)
	{
		foreach (var value in vorbisComment.Artist)
		{
			Console.WriteLine("Artist: {0}", value);
		}
	}
}

// Iterate through all VorbisComment tags
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
	var vorbisComment = file.VorbisComment;
	if (vorbisComment != null)
	{
		foreach (var tag in vorbisComment)
		{
			Console.WriteLine("{0}: {1}", tag.Key, tag.Value);
		}
	}
}

// Get all other types of metdata blocks
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
	var metadata = file.Metadata;
	foreach (MetadataBlock block in metadata) { 
		Console.WriteLine("{0} metadata block.", block.Header.Type);
	}
}
```

#### Writing Metadata

Modify a vorbis comment, create a vorbis block if none exists:

```csharp
using (FlacFile flac = new FlacFile("example.flac"))
{
    // First get the existing VorbisComment (if any)
    var comment = flac.VorbisComment;
    if (comment == null)
    {
        // Create a new vorbis comment metadata block
        comment = new VorbisComment();
        // Add it to the flac file
        flac.Metadata.Add(comment);
    }
    
    // Update the fields
    comment.Artist.Value = "Aaron";
    comment.Title.Value = "Hello World";

    // Write the changes back to the FLAC file
    flac.Save();
}
```

Check out the tests in the source code for more examples.

## Build and Test with .NET Core

    $ git clone https://github.com/AaronLenoir/flaclibsharp.git
    $ cd flaclibsharp/FlacLibSharp.Test.Core
    $ dotnet test

## Build and Test with Visual Studio

* Open the solution in Visual Studio
* Go to "Test Explorer"
* Click "Run all"

## Build NuGet Package

* Open the solution in Visual Studio
* Build project "FlacLibSharp"
* The NuGet package can be found in bin\Release
