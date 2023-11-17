# ArchiveFlow

ArchiveFlow is a Fluent API designed for streamlined and efficient processing of both zipped and unzipped file archives. It simplifies the complexities of file handling, offering a user-friendly interface for filtering, reading, and processing files with precision and ease.

## Features

- Fluent interface for easy configuration and usage.
- Support for both zipped and unzipped file processing.
- Customizable file filtering based on extensions and custom predicates.
- Options for reading files as text, binary, or streams.
- Parallel processing capabilities with configurable degrees of parallelism.
- Extensible design for future enhancements.

## Getting Started

### Installation

To use ArchiveFlow in your project, add the following package to your dependencies:

```shell
Install-Package ArchiveFlow -Version 1.0.0
```

### Basic Usage

Here's a quick example to get you started with ArchiveFlow:

```csharp
using ArchiveFlow;

var processor = new FileProcessorBuilder()
    .FromFolder("path/to/your/folder")
    .ReadAsText()
    .FilterByExtension(".txt", ".log")
    .ProcessStreamWith(stream => 
    {
        // Your stream processing logic here
    })
    .Build();

processor.ProcessFiles();
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

