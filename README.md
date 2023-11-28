# ![Logo](https://raw.githubusercontent.com/domibies/archive-flow/main/src/ArchiveFlow/icon_128x128.png) ArchiveFlow

ArchiveFlow is a Fluent API for streamlined and efficient processing of zipped and unzipped file archives. It lets you focus on processing logic instead of file/zip handling code.

[![.NET](https://github.com/domibies/archive-flow/actions/workflows/build_on_push.yml/badge.svg)](https://github.com/domibies/archive-flow/actions/workflows/build_on_push.yml)
[![NuGet](https://img.shields.io/nuget/v/ArchiveFlow.svg)](https://www.nuget.org/packages/ArchiveFlow)


## Features

- Fluent interface for easy configuration and usage.
- Support for both zipped and unzipped file processing.
- Supports .zip, .7z, and .rar archives
- Customizable file filtering based on extensions and custom predicates.
- Options for reading files as text, binary, or streams.
- Parallel processing capabilities with configurable degrees of parallelism.
- Extensible design for future enhancements.
- Exception handling for robust processing.
- Support for a wide range of frameworks

## Getting Started

### Installation

To use ArchiveFlow in your project, add the following package to your dependencies:

```shell
dotnet add package ArchiveFlow
```

### Usage

#### Basic Example

Here's a a simple example to get you started with ArchiveFlow. This will process all files as text file in archive files in the specified folder. The default behaviour is to process all entries in archive files in the folder (non-recursive), and ignore non archive files.

```csharp
var builder = new FileProcessorBuilder()
    .FromFolder("./your/path")
    .ProcessAsText((f, t) =>
    {
        // Your text processing logic here
    })

builder.Build().ProcessFiles();
```

#### More Advanced Example

Here's an example that is a bit more advanced. It reads all xml files in the specified folder, recursively, including archives younger than 10 days, and processes the text as xml. It also sets the maximum degree of parallelism to the number of processors on the machine, and handles exceptions for corrupted zip files.

```csharp
// use a concurrent dictionary beacuse we are using multiple threads
var dict = new ConcurrentDictionary<string, byte>();
var builder = new FileProcessorBuilder()
    .FromFolder("/folder/with/xmlfiles_archived_or_not", FolderSelect.RootAndSubFolders)
    .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
    .FromZipWhere((z) => z.LastModified > DateTime.Now.AddDays(-10))
    .WhereFile((f) => !f.FileName.Contains("ReturnValue"))
    .ProcessAsText((f, t) =>
    {
        XDocument xdoc = XDocument.Parse(t);
        (string? id, string? name) =
            (xdoc.Descendants("Id").FirstOrDefault()?.Value, 
             xdoc.Descendants("Name").FirstOrDefault()?.Value);

        dict.TryAdd($"{id}_{name}", 0);
    })
    .WithMaxDegreeOfParallelism(Environment.ProcessorCount)
    .HandleExceptionWith((f, ex) =>
    {
        if (f.Extension == ".zip" && ex is InvalidOperationException)
        {
            // ignore these exceptions for zip files (corrupted zip)
            return true;
        }
        return false;
    });

builder.Build().ProcessFiles();
```

check out this fiddle for a working example: [https://dotnetfiddle.net/sIwHrW](https://dotnetfiddle.net/sIwHrW)

   
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

