# FlacLibSharp

A .NET library for reading FLAC metadata.

Currently it only reads metadata. Version 1.5 will also allow you to add, remove and modify metdata. Version 2.0 will also decode FLAC audio streams.

For a more complete C# Flac Library, that already decodes audio streams, check out nflac on github!

### Installation

    PM> Install-Package FlacLibSharp

Or go to the nuget page: http://www.nuget.org/packages/FlacLibSharp

### Usage example

```csharp
using (FlacFile file = new FlacFile(@"Data\testfile1.flac"))
{
    // Access to the StreamInfo class (actually this should ALWAYS be there ...)
    var streamInfo = file.StreamInfo;
    if (streamInfo != null)
        Console.WriteLine("Flac audio length in seconds: {0}", file.StreamInfo.Duration);
    
    // Access to the VorbisComment IF it exists in the file
    var vorbisComment = file.VorbisComment;
    if (vorbisComment != null)
        Console.WriteLine("Artist - Title: {0} - {1}", vorbisComment.Artist, vorbisComment.Title);

    // Get all other types of metdata blocks:
    var metadata = file.Metadata;
    foreach (MetadataBlock block in metadata)
        Console.WriteLine("{0} metadata block.", block.Header.Type);
}
```
