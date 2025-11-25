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
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Speedgrapher.Prompts;

/// <summary>
/// SEO prompt handler.
/// </summary>
public static class SeoPrompt
{
    private const string SeoText = @"**Objective: SEO Audit**

You are an SEO expert. Your task is to audit the content for technical SEO best practices using the `audit_seo` tool.

**Instructions:**

1.  **Identify the Source:**
    *   If a URL was provided in the arguments, use that URL.
    *   If no URL was provided, use the most recent, complete text block you generated in this session (treat it as the HTML content).

2.  **Target Keyword:**
    *   If a keyword was provided, use it to check for keyword optimization.

3.  **Action:**
    *   Call the `audit_seo` tool with the appropriate parameters (`url` or `html`, and `keyword`).

4.  **Report:**
    *   Present the score and a summary of the findings.
    *   Highlight any ""fail"" or ""warning"" items.
    *   Provide specific, actionable advice to improve the score.";

    [McpServerPrompt(Name = "seo"), Description("Analyzes a URL or the current text for SEO best practices.")]
    public static IList<PromptMessage> GetSeo(
        [Description("The URL to analyze (optional).")] string? url = null,
        [Description("The target keyword to check for (optional).")] string? keyword = null)
    {
        var contextInfo = string.Empty;

        if (!string.IsNullOrEmpty(url))
        {
            contextInfo += $"\n**Target URL:** {url}";
        }
        else
        {
            contextInfo += "\n**Target Source:** Current Context (Work-in-Progress Text)";
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            contextInfo += $"\n**Target Keyword:** {keyword}";
        }

        return
        [
            new()
            {
                Role = Role.User,
                Content = new TextContentBlock { Text = SeoText + contextInfo }
            }
        ];
    }
}
