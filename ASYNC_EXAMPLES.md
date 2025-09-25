# Async Examples for ArchiveFlow

This document provides examples of using the new async support in ArchiveFlow.

## Basic Async Text Processing

```csharp
using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;

// Process text files asynchronously
var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/files", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .WithExtension(".txt", ".xml")
    .ProcessAsTextAsync(async (file, text) =>
    {
        // Simulate async processing (e.g., API calls, database operations)
        await ProcessTextContentAsync(file, text);
        Console.WriteLine($"Processed {file.FileName}");
    });

// Call the async version
await builder.Build().ProcessFilesAsync();
```

## Async Stream Processing with Parallel Execution

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/archives")
    .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
    .WithExtension(".log")
    .ProcessAsStreamAsync(async (file, stream) =>
    {
        // Process stream asynchronously
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        // Simulate async work (e.g., sending to external service)
        await AnalyzeLogFileAsync(file.FileName, content);
    })
    .WithMaxDegreeOfParallelism(Environment.ProcessorCount); // Enable parallel processing

await builder.Build().ProcessFilesAsync();
```

## Async Bytes Processing

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/images", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .WithExtension(".jpg", ".png", ".gif")
    .ProcessAsBytesAsync(async (file, bytes) =>
    {
        // Process binary data asynchronously
        var processedImage = await ProcessImageBytesAsync(bytes);
        await SaveProcessedImageAsync(file.FileName, processedImage);
    });

await builder.Build().ProcessFilesAsync();
```

## Advanced Example: Database Operations

```csharp
using System.Collections.Concurrent;

var processedFiles = new ConcurrentDictionary<string, int>();

var builder = new FileProcessorBuilder()
    .FromFolder(@"/data/archives")
    .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
    .FromZipWhere(z => z.LastModified > DateTime.Now.AddDays(-30)) // Only recent archives
    .WhereFile(f => !f.FileName.StartsWith("temp_"))
    .ProcessAsTextAsync(async (file, text) =>
    {
        try
        {
            // Parse and save to database asynchronously
            var data = ParseFileContent(text);
            await SaveToDatabase(file.FileName, data);
            
            processedFiles.TryAdd(file.FileName, text.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {file.FileName}: {ex.Message}");
        }
    })
    .WithMaxDegreeOfParallelism(4) // Limit concurrency for database connections
    .HandleExceptionWith((file, ex) =>
    {
        Console.WriteLine($"Failed to process {file.FileName}: {ex.Message}");
        return true; // Continue processing other files
    });

await builder.Build().ProcessFilesAsync();

Console.WriteLine($"Successfully processed {processedFiles.Count} files");
```

## Key Benefits of Async Support

1. **True Asynchronous I/O**: Utilize `async`/`await` for file reading and processing
2. **Better Resource Utilization**: Non-blocking operations allow better thread pool usage
3. **Scalability**: Handle larger workloads with better memory and thread management
4. **Parallel Processing**: Enhanced parallel execution with proper async coordination
5. **Integration**: Seamlessly integrate with other async APIs (databases, web services, etc.)

## Important Notes

- You cannot mix sync and async processing methods in the same builder
- Use `ProcessFilesAsync()` instead of `ProcessFiles()` when using async delegates
- Async methods automatically handle concurrency when `WithMaxDegreeOfParallelism()` is used
- All existing sync functionality remains unchanged and fully supported