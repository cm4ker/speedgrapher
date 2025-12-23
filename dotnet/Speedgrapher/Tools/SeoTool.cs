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

using System.ComponentModel;
using System.Text.Json;
using HtmlAgilityPack;
using Markdig;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Speedgrapher.Tools;

/// <summary>
/// Provides SEO auditing functionality for webpages and HTML content.
/// </summary>
[McpServerToolType]
public class SeoTool
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    /// <summary>
    /// Audits a webpage URL or raw HTML content for technical SEO best practices.
    /// </summary>
    /// <param name="url">The full URL of the webpage to audit. Either 'url' or 'html' must be provided.</param>
    /// <param name="html">The raw HTML content to audit. Use this if the content is not yet published. Supports Hugo Markdown with Front Matter.</param>
    /// <param name="keyword">The target keyword to check for optimization in the title, description, and headings.</param>
    /// <returns>An SEO audit result with score and detailed checks.</returns>
    [McpServerTool(Name = "audit_seo"), Description("Audits a webpage URL or raw HTML content for technical SEO best practices, checking title, meta description, headings, and more.")]
    public async Task<CallToolResult> AuditSeo(
        [Description("The full URL of the webpage to audit. Either 'url' or 'html' must be provided.")] string? url = null,
        [Description("The raw HTML content to audit. Use this if the content is not yet published. Supports Hugo Markdown with Front Matter.")] string? html = null,
        [Description("The target keyword to check for optimization in the title, description, and headings.")] string? keyword = null)
    {
        var result = await AuditSeoInternal(url, html, keyword);
        var json = JsonSerializer.Serialize(result);
        return new CallToolResult { Content = new List<ContentBlock> { new TextContentBlock { Text = json } } };
    }

    /// <summary>
    /// Internal method for auditing SEO.
    /// </summary>
    public static async Task<SeoResult> AuditSeoInternal(string? url = null, string? html = null, string? keyword = null)
    {
        HtmlDocument doc;

        if (!string.IsNullOrEmpty(url))
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch URL, status code: {(int)response.StatusCode}");
            }
            var content = await response.Content.ReadAsStringAsync();
            doc = new HtmlDocument();
            doc.LoadHtml(content);
        }
        else if (!string.IsNullOrEmpty(html))
        {
            // Check if input is Hugo Markdown (starts with ---)
            if (html.TrimStart().StartsWith("---"))
            {
                html = ConvertHugoMarkdownToHtml(html);
            }
            doc = new HtmlDocument();
            doc.LoadHtml(html);
        }
        else
        {
            throw new ArgumentException("Either url or html must be provided");
        }

        return AnalyzeSeo(doc, keyword ?? string.Empty);
    }

    /// <summary>
    /// Converts Hugo Markdown with YAML front matter to HTML.
    /// </summary>
    internal static string ConvertHugoMarkdownToHtml(string markdown)
    {
        var parts = markdown.Split(new[] { "---" }, 3, StringSplitOptions.None);
        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid Hugo Markdown format: missing front matter delimiters");
        }

        var frontMatterRaw = parts[1];
        var bodyMarkdown = parts[2];

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var fm = deserializer.Deserialize<FrontMatter>(frontMatterRaw);

        var pipeline = new MarkdownPipelineBuilder().Build();
        var bodyHtml = Markdown.ToHtml(bodyMarkdown, pipeline);

        return $@"
<html>
    <head>
        <title>{fm.Title}</title>
        <meta name=""description"" content=""{fm.Description}"">
        <link rel=""canonical"" href=""{fm.Canonical}"">
    </head>
    <body>
        {bodyHtml}
    </body>
</html>";
    }

    /// <summary>
    /// Analyzes the SEO of an HTML document.
    /// </summary>
    internal static SeoResult AnalyzeSeo(HtmlDocument doc, string keyword)
    {
        var checks = new List<SeoCheck>();
        var score = 100;
        keyword = keyword.ToLower();

        // 1. Title Check
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        var title = titleNode?.InnerText?.Trim() ?? string.Empty;
        var titleCheck = new SeoCheck
        {
            Name = "Title Tag",
            Status = "pass",
            Description = "Title tag exists and is within optimal length."
        };

        if (string.IsNullOrEmpty(title))
        {
            titleCheck.Status = "fail";
            titleCheck.Description = "Title tag is missing.";
            score -= 10;
        }
        else if (title.Length < 30 || title.Length > 60)
        {
            titleCheck.Status = "warning";
            titleCheck.Description = $"Title length ({title.Length}) is not optimal (30-60 chars).";
            score -= 5;
        }

        if (!string.IsNullOrEmpty(keyword) && !title.ToLower().Contains(keyword))
        {
            titleCheck.Status = "warning";
            titleCheck.Description += $" Keyword '{keyword}' not found in title.";
            score -= 5;
        }
        checks.Add(titleCheck);

        // 2. Meta Description Check
        var descNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
        var desc = descNode?.GetAttributeValue("content", string.Empty)?.Trim() ?? string.Empty;
        var descCheck = new SeoCheck
        {
            Name = "Meta Description",
            Status = "pass",
            Description = "Meta description exists and is within optimal length."
        };

        if (string.IsNullOrEmpty(desc))
        {
            descCheck.Status = "fail";
            descCheck.Description = "Meta description is missing.";
            score -= 10;
        }
        else if (desc.Length < 120 || desc.Length > 160)
        {
            descCheck.Status = "warning";
            descCheck.Description = $"Description length ({desc.Length}) is not optimal (120-160 chars).";
            score -= 5;
        }

        if (!string.IsNullOrEmpty(keyword) && !desc.ToLower().Contains(keyword))
        {
            descCheck.Status = "warning";
            descCheck.Description += $" Keyword '{keyword}' not found in description.";
            score -= 5;
        }
        checks.Add(descCheck);

        // 3. H1 Check
        var h1Nodes = doc.DocumentNode.SelectNodes("//h1");
        var h1Text = string.Empty;
        var h1Check = new SeoCheck
        {
            Name = "H1 Tag",
            Status = "pass",
            Description = "Exactly one H1 tag exists."
        };

        if (h1Nodes == null || h1Nodes.Count == 0)
        {
            h1Check.Status = "fail";
            h1Check.Description = "No H1 tag found.";
            score -= 10;
        }
        else if (h1Nodes.Count > 1)
        {
            h1Check.Status = "warning";
            h1Check.Description = $"Found {h1Nodes.Count} H1 tags. There should be exactly one.";
            score -= 5;
            h1Text = h1Nodes[0].InnerText?.Trim() ?? string.Empty;
        }
        else
        {
            h1Text = h1Nodes[0].InnerText?.Trim() ?? string.Empty;
        }

        if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(h1Text) && !h1Text.ToLower().Contains(keyword))
        {
            h1Check.Status = "warning";
            h1Check.Description += $" Keyword '{keyword}' not found in H1.";
            score -= 5;
        }
        checks.Add(h1Check);

        // 4. Images Alt Text
        var imgNodes = doc.DocumentNode.SelectNodes("//img");
        var missingAlt = 0;
        if (imgNodes != null)
        {
            foreach (var img in imgNodes)
            {
                if (!img.Attributes.Contains("alt") || string.IsNullOrWhiteSpace(img.GetAttributeValue("alt", string.Empty)))
                {
                    missingAlt++;
                }
            }
        }

        var imgCheck = new SeoCheck
        {
            Name = "Image Alt Text",
            Status = "pass",
            Description = "All images have alt text."
        };

        if (missingAlt > 0)
        {
            imgCheck.Status = "warning";
            imgCheck.Description = $"{missingAlt} images are missing alt text.";
            score -= 5;
        }
        checks.Add(imgCheck);

        // 5. Links
        var linkNodes = doc.DocumentNode.SelectNodes("//a");
        var linkCount = linkNodes?.Count ?? 0;
        var linkCheck = new SeoCheck
        {
            Name = "Links",
            Status = "pass",
            Description = $"Found {linkCount} links."
        };

        if (linkCount == 0)
        {
            linkCheck.Status = "warning";
            linkCheck.Description = "No links found on the page.";
            score -= 5;
        }
        checks.Add(linkCheck);

        // 6. Word Count
        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        var bodyText = bodyNode?.InnerText?.Trim() ?? string.Empty;
        var words = bodyText.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);
        var wordCount = words.Length;

        var wordCheck = new SeoCheck
        {
            Name = "Content Length",
            Status = "pass",
            Description = $"Content length is good ({wordCount} words)."
        };

        if (wordCount < 300)
        {
            wordCheck.Status = "warning";
            wordCheck.Description = $"Content is thin ({wordCount} words). Aim for at least 300 words.";
            score -= 10;
        }
        checks.Add(wordCheck);

        // 7. Canonical Tag
        var canonicalNode = doc.DocumentNode.SelectSingleNode("//link[@rel='canonical']");
        var canonical = canonicalNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
        var canonCheck = new SeoCheck
        {
            Name = "Canonical Tag",
            Status = "pass",
            Description = "Canonical tag exists."
        };

        if (string.IsNullOrEmpty(canonical))
        {
            canonCheck.Status = "warning";
            canonCheck.Description = "Canonical tag is missing.";
            score -= 5;
        }
        checks.Add(canonCheck);

        if (score < 0)
        {
            score = 0;
        }

        return new SeoResult
        {
            Score = score,
            Title = title,
            Description = desc,
            H1 = h1Text,
            WordCount = wordCount,
            Checks = checks
        };
    }
}

/// <summary>
/// Represents the front matter of a Hugo Markdown file.
/// </summary>
public class FrontMatter
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Canonical { get; set; } = string.Empty;
}

/// <summary>
/// Represents a single SEO check result.
/// </summary>
public class SeoCheck
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "pass", "fail", "warning"
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents the result of an SEO audit.
/// </summary>
public class SeoResult
{
    public int Score { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string H1 { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public List<SeoCheck> Checks { get; set; } = [];
}
