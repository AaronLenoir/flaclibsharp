# FlacLibSharp

A .NET library for reading and writing FLAC metadata.

### Installation

    PM> Install-Package FlacLibSharp

Or go to the nuget page: https://www.nuget.org/packages/FlacLibSharp

### Warning

If you use this for anything important, make sure your files are backed up. Since this was a hobby project I cannot guarantee anything in terms of quality.

Please evaluate carefully and report any issues you find here on GitHub.

### Usage example

#### Reading Metadata

```csharp
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
    // Access to the StreamInfo class (actually this should ALWAYS be there ...)
    var streamInfo = file.StreamInfo;
    if (streamInfo != null) {
        Console.WriteLine("Flac audio length in seconds: {0}", streamInfo.Duration);
    }

    // Access to the VorbisComment IF it exists in the file
    var vorbisComment = file.VorbisComment;
    if (vorbisComment != null) {
        Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist.Value, vorbisComment.Title.Value);
    }

    // Access to the VorbisComment IF it exists in the file, with multiple values for a single field
    var vorbisComment = file.VorbisComment;
    if (vorbisComment != null) {
        foreach(var value in vorbisComment.Artist) {
            Console.WriteLine("Artist: {0}", value);
        }
    }
    
    // Get all other types of metdata blocks:
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
