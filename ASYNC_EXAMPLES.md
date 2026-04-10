# Async Examples for ArchiveFlow

This document provides comprehensive examples of using the async support in ArchiveFlow with proper cancellation and best practices.

## Basic Async Text Processing

```csharp
using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using System.Threading;

// Create a cancellation token source for graceful cancellation
using var cts = new CancellationTokenSource();

// Process text files asynchronously
var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/files", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .WithExtension(".txt", ".xml")
    .ProcessAsTextAsync(async (file, text, cancellationToken) =>
    {
        // Pass the cancellation token to async operations
        await ProcessTextContentAsync(file, text, cancellationToken);
        Console.WriteLine($"Processed {file.FileName}");
    });

// Call the async version with cancellation support
await builder.Build().ProcessFilesAsync(cts.Token);
```

## Async Stream Processing with Parallel Execution

```csharp
using var cts = new CancellationTokenSource();

var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/archives")
    .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
    .WithExtension(".log")
    .ProcessAsStreamAsync(async (file, stream, cancellationToken) =>
    {
        // Process stream asynchronously
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        // Simulate async work with cancellation support
        await AnalyzeLogFileAsync(file.FileName, content, cancellationToken);
    })
    .WithMaxDegreeOfParallelism(Environment.ProcessorCount); // Enable parallel processing

await builder.Build().ProcessFilesAsync(cts.Token);
```

## Async Bytes Processing

```csharp
using var cts = new CancellationTokenSource();

var builder = new FileProcessorBuilder()
    .FromFolder(@"/path/to/images", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .WithExtension(".jpg", ".png", ".gif")
    .ProcessAsBytesAsync(async (file, bytes, cancellationToken) =>
    {
        // Process binary data asynchronously with cancellation
        var processedImage = await ProcessImageBytesAsync(bytes, cancellationToken);
        await SaveProcessedImageAsync(file.FileName, processedImage, cancellationToken);
    });

await builder.Build().ProcessFilesAsync(cts.Token);
```

## Advanced Example: Database Operations with Cancellation

```csharp
using System.Collections.Concurrent;
using System.Threading;

// Create a cancellation token source with timeout
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));

var processedFiles = new ConcurrentDictionary<string, int>();
var errors = new ConcurrentBag<(string FileName, Exception Error)>();

var builder = new FileProcessorBuilder()
    .FromFolder(@"/data/archives")
    .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
    .FromZipWhere(z => z.LastModified > DateTime.Now.AddDays(-30)) // Only recent archives
    .WhereFile(f => !f.FileName.StartsWith("temp_"))
    .ProcessAsTextAsync(async (file, text, cancellationToken) =>
    {
        try
        {
            // Check for cancellation before expensive operations
            cancellationToken.ThrowIfCancellationRequested();

            // Parse and save to database asynchronously
            var data = ParseFileContent(text);
            await SaveToDatabaseAsync(file.FileName, data, cancellationToken);

            processedFiles.TryAdd(file.FileName, text.Length);
        }
        catch (OperationCanceledException)
        {
            // Cancellation requested - rethrow to stop processing
            throw;
        }
        catch (Exception ex)
        {
            // Log other exceptions
            errors.Add((file.FileName, ex));
            Console.WriteLine($"Error processing {file.FileName}: {ex.Message}");
        }
    })
    .WithMaxDegreeOfParallelism(4) // Limit concurrency for database connections
    .HandleExceptionWith((file, ex) =>
    {
        if (ex is OperationCanceledException)
        {
            // Don't swallow cancellation exceptions
            return false;
        }

        Console.WriteLine($"Failed to process {file.FileName}: {ex.Message}");
        return true; // Continue processing other files
    });

try
{
    await builder.Build().ProcessFilesAsync(cts.Token);
    Console.WriteLine($"Successfully processed {processedFiles.Count} files");

    if (errors.Any())
    {
        Console.WriteLine($"Encountered {errors.Count} errors during processing");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Processing was cancelled");
}
```

## Cancellation with User Input

```csharp
using System.Threading;
using System.Threading.Tasks;

var cts = new CancellationTokenSource();

// Cancel processing when user presses any key
var keyTask = Task.Run(() =>
{
    Console.WriteLine("Press any key to cancel processing...");
    Console.ReadKey();
    cts.Cancel();
});

var builder = new FileProcessorBuilder()
    .FromFolder(@"/large/dataset")
    .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
    .ProcessAsTextAsync(async (file, text, cancellationToken) =>
    {
        // Respect cancellation in your processing logic
        await ProcessWithCancellationAsync(file, text, cancellationToken);
    })
    .WithMaxDegreeOfParallelism(Environment.ProcessorCount);

try
{
    await builder.Build().ProcessFilesAsync(cts.Token);
    Console.WriteLine("Processing completed successfully");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Processing was cancelled by user");
}
finally
{
    cts.Dispose();
}
```

## Integration with ASP.NET Core

```csharp
// In an ASP.NET Core controller or minimal API endpoint
app.MapPost("/process-uploads", async (HttpContext context, CancellationToken cancellationToken) =>
{
    var uploadPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    Directory.CreateDirectory(uploadPath);

    try
    {
        var results = new List<string>();

        var builder = new FileProcessorBuilder()
            .FromFolder(uploadPath, FolderSelect.RootAndSubFolders)
            .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
            .ProcessAsTextAsync(async (file, text, ct) =>
            {
                // Use the request's cancellation token
                var processed = await ProcessUploadedFileAsync(file, text, ct);
                results.Add(processed);
            });

        // Pass the HTTP request cancellation token
        await builder.Build().ProcessFilesAsync(cancellationToken);

        return Results.Ok(new { ProcessedCount = results.Count, Files = results });
    }
    catch (OperationCanceledException)
    {
        return Results.Problem("Request was cancelled", statusCode: 499);
    }
    finally
    {
        // Cleanup
        Directory.Delete(uploadPath, recursive: true);
    }
});
```

## Key Benefits of Async Support

1. **True Asynchronous I/O**: Utilize `async`/`await` for file reading and processing
2. **Cancellation Support**: All async methods accept `CancellationToken` for graceful cancellation
3. **Better Resource Utilization**: Non-blocking operations allow better thread pool usage
4. **Scalability**: Handle larger workloads with better memory and thread management
5. **Parallel Processing**: Enhanced parallel execution with proper async coordination using `SemaphoreSlim`
6. **Integration**: Seamlessly integrate with other async APIs (databases, web services, etc.)
7. **ConfigureAwait**: Library uses `ConfigureAwait(false)` internally to avoid deadlocks

## Best Practices

### 1. Always Use CancellationToken

```csharp
// ✅ GOOD: Pass cancellation token through the call chain
await builder.Build().ProcessFilesAsync(cancellationToken);

// ❌ BAD: Ignoring cancellation
await builder.Build().ProcessFilesAsync();
```

### 2. Respect Cancellation in Your Handlers

```csharp
// ✅ GOOD: Check cancellation before expensive operations
.ProcessAsTextAsync(async (file, text, ct) =>
{
    ct.ThrowIfCancellationRequested();
    await ExpensiveOperationAsync(text, ct);
})

// ❌ BAD: Ignoring the cancellation token parameter
.ProcessAsTextAsync(async (file, text, ct) =>
{
    await ExpensiveOperationAsync(text); // No cancellation!
})
```

### 3. Use Thread-Safe Collections for Parallel Processing

```csharp
// ✅ GOOD: Thread-safe collection
var results = new ConcurrentDictionary<string, Data>();
.ProcessAsTextAsync(async (file, text, ct) =>
{
    results.TryAdd(file.FileName, data);
})
.WithMaxDegreeOfParallelism(8);

// ❌ BAD: Non-thread-safe collection with parallel processing
var results = new Dictionary<string, Data>(); // Race conditions!
```

### 4. Don't Swallow OperationCanceledException

```csharp
// ✅ GOOD: Let cancellation propagate
.HandleExceptionWith((file, ex) =>
{
    if (ex is OperationCanceledException)
        return false; // Don't handle, let it propagate

    LogError(file, ex);
    return true;
})

// ❌ BAD: Swallowing cancellation
.HandleExceptionWith((file, ex) =>
{
    LogError(file, ex); // Logs even cancellation!
    return true; // Cancellation won't work properly
})
```

## Important Notes

- You cannot mix sync and async processing methods in the same builder
- Use `ProcessFilesAsync()` instead of `ProcessFiles()` when using async delegates
- Async methods automatically handle concurrency when `WithMaxDegreeOfParallelism()` is used
- All existing sync functionality remains unchanged and fully supported
- The library uses `ConfigureAwait(false)` internally - no risk of deadlocks in UI or ASP.NET contexts
- File streams are properly disposed by the library - focus on your processing logic
