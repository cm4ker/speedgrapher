![logo](logo.jpeg)

# Speedgrapher

> This is not an officially supported Google product.

Speedgrapher is a local MCP (Model Context Protocol) server designed to assist writers, especially in the tech industry. It provides a suite of tools and resources to streamline the writing process, from research and drafting to editing and publishing.

## Requirements

- .NET 10.0 SDK or later

## Building

```bash
dotnet build
```

## Running

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
