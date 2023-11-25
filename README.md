# ![Logo](src/ArchiveFlow/icon_128x128.png) ArchiveFlow

ArchiveFlow is a Fluent API for streamlined and efficient processing of zipped and unzipped file archives. It lets you focus on processing logic instead of file/zip handling code.

## Features

- Fluent interface for easy configuration and usage.
- Support for both zipped and unzipped file processing.
- Supports .zip, .7z, .rar and .tar archives
- Customizable file filtering based on extensions and custom predicates.
- Options for reading files as text, binary, or streams.
- Parallel processing capabilities with configurable degrees of parallelism.
- Extensible design for future enhancements.
- Exception handling for robust processing.

## Getting Started

### Installation

To use ArchiveFlow in your project, add the following package to your dependencies:

```shell
Install-Package ArchiveFlow
```

### Usage

#### Basic Example

Here's a a simple example to get you started with ArchiveFlow. This will process all files as text file in archive files in the specified folder. The default behaviour is to process all entries in archive files in the folder (non-recursive), and ignore non archive files.

```csharp
using ArchiveFlow;

var builder = new FileProcessorBuilder()
    .FromFolder("./your/path")
    .ProcessAsText((t) =>
    {
        // Your text processing logic here
    })

builder.Build().ProcessFiles();
```
   
## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Dominique Biesmans - [https://www.linkedin.com/in/dominiquebiesmans/](LinkedIn) 

Project Link: [https://github.com/domibies/ArchiveFlow](#)

