// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Text.Json;
using Speedgrapher.Prompts;
using Speedgrapher.Tools;

var version = "v0.4.0";
var editorialGuidelines = "EDITORIAL.md";
var localizationGuidelines = "LOCALIZATION.md";

// Parse command-line arguments
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--version":
        case "-v":
            Console.WriteLine(version);
            return;
        case "--editorial":
            if (i + 1 < args.Length)
            {
                editorialGuidelines = args[++i];
            }
            break;
        case "--localization":
            if (i + 1 < args.Length)
            {
                localizationGuidelines = args[++i];
            }
            break;
        case "--help":
        case "-h":
            Console.WriteLine("Speedgrapher - A local MCP server for writers");
            Console.WriteLine();
            Console.WriteLine("Usage: speedgrapher [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --version, -v           Show version information");
            Console.WriteLine("  --editorial <path>      Path to editorial guidelines file (default: EDITORIAL.md)");
            Console.WriteLine("  --localization <path>   Path to localization guidelines file (default: LOCALIZATION.md)");
            Console.WriteLine("  --help, -h              Show this help message");
            return;
    }
}

// Set the guideline paths for the prompts that need them
ReviewPrompt.SetGuidelinePath(editorialGuidelines);
LocalizePrompt.SetGuidelinePath(localizationGuidelines);

// Build and run the MCP server
var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(Program).Assembly)
    .WithPromptsFromAssembly(typeof(Program).Assembly);

var host = builder.Build();
await host.RunAsync();
