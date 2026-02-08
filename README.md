# 📂 FileBundler CLI (fib) 🚀

**FileBundler CLI** (or `fib`) is a powerful, cross-platform command-line tool built with **.NET**. It is designed to streamline the workflow of developers and instructors by bundling multiple source code files into a single, organized file.

---

## 💡 Why FileBundler?
Manually navigating through dozens of nested folders to review code is time-consuming. `fib` automates this process by scanning project directories, filtering by language, and generating a clean, unified bundle in seconds.

---

## ✨ Key Features
* **Smart Bundling:** Consolidate code files into a single output file.
* [cite_start]**Language Filtering:** Bundle specific programming languages (e.g., `csharp`, `java`, `python`) or use `all`. 
* [cite_start]**Code Cleaning:** Automatically remove empty lines from source files to reduce clutter. 
* [cite_start]**Source Attribution:** Include the original relative file path as a comment for every code block. 
* [cite_start]**Author Branding:** Add a custom header with the author's name to the top of the bundle. 
* [cite_start]**Custom Sorting:** Order files alphabetically or by file extension/type. 
* [cite_start]**Safety First:** Automatically excludes system folders like `bin` and `debug` to keep the bundle clean. 

---

## 🛠 Tech Stack
* **Framework:** .NET 8.0+
* [cite_start]**Library:** [System.CommandLine](https://github.com/dotnet/command-line-api) - leveraging Microsoft's latest infrastructure for robust CLI parsing. [cite: 1]

---

## 🚀 Getting Started

### Installation
1.  Clone the repository:
    ```bash
    git clone [https://github.com/YourUsername/fib.git](https://github.com/YourUsername/fib.git)
    ```
2.  Publish the project to generate the executable.
3.  [cite_start]Add the publish directory to your system's **Path** environment variable to use `fib` from anywhere. [cite: 1]

### Usage Examples

#### 1. Basic Bundle
Bundling all C# files into one text file with source notes:
```bash
fib bundle --language csharp --output bundle.txt --note
