![logo](logo.jpeg)

# Speedgrapher

> This is not an officially supported Google product.

Speedgrapher is a local MCP (Model Context Protocol) server designed to assist writers, especially in the tech industry. It provides a suite of tools and resources to streamline the writing process, from research and drafting to editing and publishing.

**Note**: This is a C# port of the original Go implementation. The original repository can be found at [github.com/danicat/speedgrapher](https://github.com/danicat/speedgrapher).

## Requirements

- .NET 10.0 SDK or later

## Installation

### As a .NET Global Tool

You can install Speedgrapher as a global .NET tool:

```bash
dotnet tool install -g Speedgrapher
```

After installation, you can run it directly:

```bash
speedgrapher
```

### From Source

Clone the repository and build from source:

```bash
git clone https://github.com/cm4ker/speedgrapher.git
cd speedgrapher
dotnet build
```

## Running

### If installed as a global tool:

```bash
speedgrapher
```

### From source:

```bash
cd Speedgrapher
dotnet run
```

### Command-line Options

- `--version`, `-v`: Show version information
- `--editorial <path>`: Path to editorial guidelines file (default: EDITORIAL.md)
- `--localization <path>`: Path to localization guidelines file (default: LOCALIZATION.md)
- `--help`, `-h`: Show help message

## Running Tests

```bash
dotnet test
```

## Tools

- **fog**: Calculates the Gunning Fog Index to estimate the readability of an English text. Lower scores indicate easier reading.
- **audit_seo**: Audits a webpage URL or raw HTML content for technical SEO best practices, checking title, meta description, headings, and more.

## Prompts

- **haiku**: Creates a haiku about a given topic, or infers the topic from the current conversation.
- **interview**: Interviews an author to produce a technical blog post.
- **localize**: Translates the article currently being worked on into a target language.
- **readability**: Analyzes the last generated text for readability using the Gunning Fog Index.
- **reflect**: Analyzes the current session and proposes improvements to the development process.
- **review**: Reviews the article currently being worked on against the editorial guidelines.
- **context**: Loads the current work-in-progress article to context for further commands.
- **voice**: Analyzes the voice and tone of the user's writing to replicate it in generated text.
- **outline**: Generates a structured outline of the current draft, concept or interview report.
- **expand**: Expands a working outline or draft into a more detailed article.
- **publish**: Publishes the final version of the article.
- **seo**: Analyzes a URL or the current text for SEO best practices.

## Project Structure

```
├── Speedgrapher/           # Main MCP server application
│   ├── Program.cs          # Entry point
│   ├── Tools/              # MCP tools
│   │   ├── FogTool.cs      # Gunning Fog Index calculator
│   │   └── SeoTool.cs      # SEO audit tool
│   └── Prompts/            # MCP prompts
├── Speedgrapher.Tests/     # Unit tests
└── Speedgrapher.slnx       # Solution file
```

## MCP Configuration

To use this server with an MCP client, add the following configuration:

### If installed as a global tool:

```json
{
  "mcpServers": {
    "speedgrapher": {
      "command": "speedgrapher"
    }
  }
}
```

### If running from source:

```json
{
  "mcpServers": {
    "speedgrapher": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/speedgrapher/Speedgrapher/Speedgrapher.csproj"]
    }
  }
}
```

## Publishing to NuGet

### Manual Publishing

To publish Speedgrapher as a NuGet package manually, you need to:

1. **Create a NuGet API key** at [nuget.org](https://www.nuget.org/)

2. **Build the package**:
   ```bash
   dotnet pack -c Release
   ```

3. **Publish to NuGet**:
   ```bash
   dotnet nuget push bin/Release/Speedgrapher.*.nupkg --api-key <YOUR_API_KEY> --source https://api.nuget.org/v3/index.json
   ```

4. **For local testing** before publishing:
   ```bash
   dotnet tool install -g --add-source ./bin/Release Speedgrapher
   ```

### Automated Publishing via GitHub Actions

The repository includes a GitHub Actions workflow that automatically publishes to NuGet when a new release is created:

1. **Setup NuGet API Key**:
   - Go to [nuget.org](https://www.nuget.org/) and create an API key with push permissions
   - In your GitHub repository, go to Settings → Secrets and variables → Actions
   - Create a new secret named `NUGET_API_KEY` with your NuGet API key

2. **Create a Release**:
   - Go to your GitHub repository's Releases page
   - Click "Create a new release"
   - Create a new tag (e.g., `v1.0.0`)
   - Publish the release
   - The workflow will automatically build, test, pack, and publish to NuGet

3. **Manual Workflow Trigger** (optional):
   - You can also manually trigger the publish workflow from the Actions tab
   - Go to Actions → Publish to NuGet → Run workflow
