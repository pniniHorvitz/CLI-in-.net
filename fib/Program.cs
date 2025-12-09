


using System.CommandLine;
var bundleOption = new Option<FileInfo>("--output", "File path and name");
var languagesOption = new Option<string[]>(
    name: "--languages",
    description: "List of programming languages to include (e.g. cs, js, py, all)",
    getDefaultValue: () => new[] { "all" }
)
{
    AllowMultipleArgumentsPerToken = true,
    Arity = ArgumentArity.OneOrMore,
};
var noteOption = new Option<bool>(
    name: "--note",
    description: "Include a comment in the bundle file with the source file path."
);
noteOption.AddAlias("-n");
var sortOption = new Option<string>(
    name: "--sort",
    description: "Sorting method: 'name' (alphabetically by file name) or 'type' (by file extension).",
    getDefaultValue: () => "name"
);
sortOption.AddAlias("-s");
var removeEmptyLinesOption = new Option<bool>(
    name: "--remove-empty-lines",
    description: "Remove empty lines from the source files before bundling.",
    getDefaultValue: () => false
);
removeEmptyLinesOption.AddAlias("-r");
var authorOption = new Option<string>(
    name: "--author",
    description: "Name of the file author. The name will be added as a comment at the top of the bundle file."
);
authorOption.AddAlias("-a");
languagesOption.AddValidator(result =>
{
    var values = result.GetValueOrDefault<string[]>();
    if (values == null || values.Length == 0)
    {
        result.ErrorMessage = "You must provide at least one language (cs, js, py, or all).";
        return;
    }
    var allowed = new[] { "cs", "js", "py", "all" };
    var flatValues = values
    .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
    .Select(v => v.Trim().ToLower())
    .ToList();
    var invalid = flatValues.Where(v => !allowed.Contains(v)).ToList();
    if (invalid.Any())
    {
        result.ErrorMessage = $"Invalid language(s): {string.Join(", ", invalid)}. Allowed values: cs, js, py, all.";
    }
});
sortOption.AddValidator(result =>
{
    var value = result.GetValueOrDefault<string>();
    var allowed = new[] { "name", "type" };
    if (!allowed.Contains(value))
    {
        result.ErrorMessage = $"Invalid sort value: {value}. Allowed values: name, type.";
    }
});
bundleOption.AddValidator(result =>
{
    var file = result.GetValueOrDefault<FileInfo>();
    if (file == null)
    {
        result.ErrorMessage = "Output file path is required.";
        return;
    }
    if (!Directory.Exists(file.DirectoryName))
    {
        result.ErrorMessage = $"Directory does not exist: {file.DirectoryName}";
    }
});
authorOption.AddValidator(result =>
{
    var value = result.GetValueOrDefault<string>();
    if (string.IsNullOrWhiteSpace(value))
    {
        result.ErrorMessage = "Author name cannot be empty.";
    }
});
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddOption(bundleOption);
bundleCommand.AddOption(languagesOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeEmptyLinesOption);
bundleCommand.AddOption(authorOption);
var rootCommand = new RootCommand("Root command for File Boundler CLI");
rootCommand.AddCommand(bundleCommand);
var createRspCommand = new Command("create-rsp", "Create a response file with a pre-built bundle command.");
createRspCommand.SetHandler(() =>
{
    string output;
    while (true)
    {
        Console.Write("Enter output file path:");
        output = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(output))
        {
            break;
        }
        Console.WriteLine("Error: Output file path cannot be empty.");
    }
    string languages;
    while (true)
    {
        Console.Write("Enter languages (cs, js, py, all): ");
        languages = Console.ReadLine();
        var allowedLanguages = new[] { "cs", "js", "py", "all" };
        var invalidLanguages = languages.Split(',').Where(l => !allowedLanguages.Contains(l)).ToList();
        if (!invalidLanguages.Any())
        {
            break;
        }
        Console.WriteLine($"Invalid languages: {string.Join(", ", invalidLanguages)}. Allowed values: cs, js, py, all.");
    }
    string note;
    while (true)
    {
        Console.Write("Include notes? (yes/no): ");
        note = Console.ReadLine().ToLower();
        if (note == "yes" || note == "no")
        {
            break;
        }
        Console.WriteLine("Error: Please enter 'yes' or 'no'.");
    }
    string sort;
    while (true)
    {
        Console.Write("Sort files? (name/type): ");
        sort = Console.ReadLine().ToLower();
        var allowedSorts = new[] { "name", "type" };
        if (allowedSorts.Contains(sort))
        {
            break;
        }
        Console.WriteLine($"Invalid sort value: {sort}. Allowed values: name, type.");
    }
    string removeEmptyLines;
    while (true)
    {
        Console.Write("Remove empty lines? (yes/no): ");
        removeEmptyLines = Console.ReadLine().ToLower();
        if (removeEmptyLines == "yes" || removeEmptyLines == "no")
        {
            break;
        }
        Console.WriteLine("Error: Please enter 'yes' or 'no'.");
    }
    string author;
    while (true)
    {
        Console.Write("Author name (leave empty if none): ");
        author = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(author) || author.Length > 0)
        {
            break;
        }
        Console.WriteLine("Error: Author name cannot be empty.");
    }
    var command = "fib bundle " +
                  $"--output \"{output}\" " +
                  $"--languages {languages} " +
                  $"{(note == "yes" ? "--note " : "")}" +
                  $"--sort {sort} " +
                  $"{(removeEmptyLines == "yes" ? "--remove-empty-lines " : "")}" +
                  $"{(!string.IsNullOrWhiteSpace(author) ? $"--author \"{author}\"" : "")}";
    string rspFile;
    while (true)
    {
        Console.Write("Enter response file name (e.g., mybundle.rsp): ");
        rspFile = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(rspFile))
        {
            break;
        }
        Console.WriteLine("Error: Response file name cannot be empty.");
    }
    File.WriteAllText(rspFile, command);
    Console.WriteLine($"Response file '{rspFile}' created successfully!");
});
rootCommand.AddCommand(createRspCommand);
bundleCommand.SetHandler(async (
    FileInfo output,
    string[] languages,
    bool note,
    string sort,
    bool removeEmptyLines,
    string author
) =>
{
    try
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        var extensions = new Dictionary<string, string[]>
        {
            { "cs", new[] { ".cs" } },
            { "js", new[] { ".js" } },
            { "py", new[] { ".py" } }
        };
        List<string> selectedExtensions = new();
        if (languages.Contains("all", StringComparer.OrdinalIgnoreCase))
            selectedExtensions.AddRange(extensions.Values.SelectMany(e => e));
        else
        {
            foreach (var lang in languages)
            {
                if (extensions.ContainsKey(lang.ToLower()))
                    selectedExtensions.AddRange(extensions[lang.ToLower()]);
            }
        }
        var files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories)
                             .Where(f => selectedExtensions.Contains(Path.GetExtension(f)))
                             .ToList();
        if (!files.Any())
        {
            Console.WriteLine("No files found matching the selected languages.");
            return;
        }
        files = sort.ToLower() switch
        {
            "type" => files.OrderBy(f => Path.GetExtension(f)).ToList(),
            _ => files.OrderBy(f => Path.GetFileName(f)).ToList()
        };
        using var writer = new StreamWriter(output.FullName, false);
        if (!string.IsNullOrWhiteSpace(author))
        {
            await writer.WriteLineAsync($"// Author: {author}");
            await writer.WriteLineAsync($"// Created: {DateTime.Now}");
            await writer.WriteLineAsync("// -------------------------------------------");
        }
        foreach (var file in files)
        {
            if (note)
            {
                await writer.WriteLineAsync($"// ===== File: {Path.GetFileName(file)} =====");
            }
            var lines = await File.ReadAllLinesAsync(file);
            if (removeEmptyLines)
                lines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            foreach (var line in lines)
                await writer.WriteLineAsync(line);
            await writer.WriteLineAsync(); // שורה ריקה בין קבצים
        }
        Console.WriteLine($"Bundled {files.Count} files into '{output.FullName}' successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}, bundleOption, languagesOption, noteOption, sortOption, removeEmptyLinesOption, authorOption);
rootCommand.Invoke(args);
