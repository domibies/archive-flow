# ![Logo](https://raw.githubusercontent.com/domibies/archive-flow/main/src/ArchiveFlow/icon_128x128.png) ArchiveFlow

ArchiveFlow is a Fluent API for streamlined and efficient processing of zipped and unzipped file archives. It lets you focus on processing logic instead of file/zip handling code.

[![.NET](https://github.com/domibies/archive-flow/actions/workflows/build_on_push.yml/badge.svg)](https://github.com/domibies/archive-flow/actions/workflows/build_on_push.yml)
[![NuGet](https://img.shields.io/nuget/v/ArchiveFlow.svg)](https://www.nuget.org/packages/ArchiveFlow)


## Features

- **Async-First Design**: Full support for modern async/await patterns with proper cancellation
- Fluent interface for easy configuration and usage
- Support for both zipped and unzipped file processing
- Supports .zip, .7z, and .rar archives
- Customizable file filtering based on extensions and custom predicates
- Options for reading files as text, binary, or streams
- Parallel processing capabilities with configurable degrees of parallelism
- Extensible design for future enhancements
- Exception handling for robust processing
- Support for a wide range of frameworks

## Getting Started

### Installation

To use ArchiveFlow in your project, add the following package to your dependencies:

```shell
dotnet add package ArchiveFlow
```

### Usage

#### Basic Async Example (Recommended)

Here's a simple example to get you started with ArchiveFlow using modern async/await patterns. This processes all text files in archive files in the specified folder:

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder("./your/path")
    .ProcessAsTextAsync(async (f, t, cancellationToken) =>
    {
        // Your async text processing logic here
        await ProcessTextAsync(t, cancellationToken);
    });

await builder.Build().ProcessFilesAsync();
```

#### Advanced Async Example with Cancellation

Here's a more advanced example showing async processing with cancellation support, parallel execution, and exception handling:

```csharp
using var cts = new CancellationTokenSource();

// Use a concurrent dictionary because we are using multiple threads
var results = new ConcurrentDictionary<string, XDocument>();

var builder = new FileProcessorBuilder()
    .FromFolder("/folder/with/xmlfiles_archived_or_not", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .FromZipWhere((z) => z.LastModified > DateTime.Now.AddDays(-10))
    .WhereFile((f) => !f.FileName.Contains("ReturnValue"))
    .ProcessAsTextAsync(async (f, t, cancellationToken) =>
    {
        XDocument xdoc = XDocument.Parse(t);
        (string? id, string? name) =
            (xdoc.Descendants("Id").FirstOrDefault()?.Value,
             xdoc.Descendants("Name").FirstOrDefault()?.Value);

        // Simulate async database operation
        await SaveToDatabase Async(id, name, cancellationToken);

        results.TryAdd($"{id}_{name}", xdoc);
    })
    .WithMaxDegreeOfParallelism(Environment.ProcessorCount)
    .HandleExceptionWith((f, ex) =>
    {
        if (f.Extension == ".zip" && ex is InvalidOperationException)
        {
            // Ignore corrupted zip files
            return true;
        }
        return false;
    });

await builder.Build().ProcessFilesAsync(cts.Token);
```

#### Async Stream Processing

For scenarios where you need direct stream access:

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder("./data/large-files", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .WithExtension(".log")
    .ProcessAsStreamAsync(async (file, stream, cancellationToken) =>
    {
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        // Process large files efficiently
        await AnalyzeLogAsync(file.FileName, content, cancellationToken);
    })
    .WithMaxDegreeOfParallelism(4);

await builder.Build().ProcessFilesAsync();
```

#### Sync API (Legacy)

For scenarios where you cannot use async/await, the synchronous API is still fully supported:

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder("./your/path")
    .ProcessAsText((f, t) =>
    {
        // Your synchronous processing logic here
        ProcessText(t);
    });

builder.Build().ProcessFiles();
```

**Note**: You cannot mix sync and async processing methods in the same builder. Choose one approach based on your needs.

## API Highlights

### Async Methods with Cancellation Support

All async processing methods support `CancellationToken`:

- `ProcessAsTextAsync(AsyncTextProcessingAction action)` - Process files as text
- `ProcessAsStreamAsync(AsyncStreamProcessingAction action)` - Process files as streams
- `ProcessAsBytesAsync(AsyncBytesProcessingAction action)` - Process files as byte arrays
- `ProcessFilesAsync(CancellationToken cancellationToken = default)` - Execute async processing

### Configuration

- `FromFolder(string path, FolderSelect folderSelect)` - Specify source folder
- `SetArchiveSearch(ArchiveSearch type)` - Control archive processing behavior
- `WithExtension(params string[] exts)` - Filter by file extensions
- `WhereFile(FileInformationFilter filter)` - Custom file filtering
- `FromZipWhere(FileInformationFilter filter)` - Filter archive files
- `WithMaxDegreeOfParallelism(int maxDegree)` - Control concurrent processing
- `HandleExceptionWith(FileExceptionHandler handler)` - Custom exception handling

## Best Practices

1. **Prefer Async**: Use async methods (`ProcessAsTextAsync`, `ProcessAsBytesAsync`, `ProcessAsStreamAsync`) for I/O-intensive operations
2. **Use CancellationToken**: Always pass `CancellationToken` when calling `ProcessFilesAsync()` to support graceful cancellation
3. **Thread Safety**: When using parallel processing (`WithMaxDegreeOfParallelism`), use thread-safe collections like `ConcurrentDictionary`
4. **Resource Management**: The library properly manages stream disposal; focus on your processing logic
5. **ConfigureAwait**: The library uses `ConfigureAwait(false)` internally for optimal async performance

## Examples

Check out this fiddle for a working example: [https://dotnetfiddle.net/sIwHrW](https://dotnetfiddle.net/sIwHrW)

For more comprehensive async examples, see [ASYNC_EXAMPLES.md](ASYNC_EXAMPLES.md)

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License.

## Contact

Dominique Biesmans - [https://www.linkedin.com/in/dominiquebiesmans/](https://www.linkedin.com/in/dominiquebiesmans/)

Project Link: [https://github.com/domibies/archive-flow](https://github.com/domibies/archive-flow)
