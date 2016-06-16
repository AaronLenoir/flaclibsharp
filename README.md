# FlacLibSharp

A .NET library for reading (and writing) FLAC metadata.

The stable version (1.0) can only read metadata. The beta release, version 1.5, can also modify metadata.

At the moment, it looks like version 1.5 will remain in beta unless someone actually wants to use it. I personally do not have a need for this so the motivation is hard to find and I would rather do something else. But if there's somebody actually using this library, I will make sure to do more extenstive testing and make a stable release.

That said, I think version 1.5 is not too bad. So use it, but only if your FLAC files are backed up somewhere ...

Version 2.0 will also decode FLAC audio streams. But these plans are currently in the freezer, unless I feel like picking the project back up again.

### Installation

    PM> Install-Package FlacLibSharp

For 1.5 (which also edits metadata):

    PM> Install-Package FlacLibSharp -Pre

Or go to the nuget page: https://www.nuget.org/packages/FlacLibSharp

### Usage example

#### Reading Metadata

```csharp
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
    // Access to the StreamInfo class (actually this should ALWAYS be there ...)
    var streamInfo = file.StreamInfo;
    if (streamInfo != null) {
        Console.WriteLine("Flac audio length in seconds: {0}", file.StreamInfo.Duration);
    }

    // Access to the VorbisComment IF it exists in the file
    var vorbisComment = file.VorbisComment;
    if (vorbisComment != null) {
        Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist, vorbisComment.Title);
    }
    
    // Get all other types of metdata blocks:
    var metadata = file.Metadata;
    foreach (MetadataBlock block in metadata) {
        Console.WriteLine("{0} metadata block.", block.Header.Type);
    }
}
```

#### Editing Metadata (Only in 1.5.0.0-beta)

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
    comment.Artist = "Aaron";
    comment.Title = "Hello World";

    // Write the changes back to the FLAC file
    flac.Save();
}
```

Check out the tests in the source code for more examples.
