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

using Speedgrapher.Tools;

namespace Speedgrapher.Tests;

public class SeoToolTests
{
    [Fact]
    public async Task AuditSeo_WithPerfectHtml_ReturnsScore100()
    {
        var html = @"
<html>
    <head>
        <title>Perfect SEO Title Example For Testing Keyword</title>
        <meta name=""description"" content=""This is a perfect meta description that is long enough to pass the check and contains the keyword we are looking for in this test case."">
        <link rel=""canonical"" href=""https://example.com/page"">
    </head>
    <body>
        <h1>Main Keyword Heading</h1>
        <p>This is some content. It needs to be long enough to pass the word count check.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
        Extra words to make sure we pass the count. Extra words to make sure we pass the count.
        </p>
        <img src=""image.jpg"" alt=""Description of image"">
        <a href=""/internal"">Internal Link</a>
    </body>
</html>";

        var result = await SeoTool.AuditSeo(html: html, keyword: "keyword");

        Assert.Equal(100, result.Score);
        Assert.Equal("Perfect SEO Title Example For Testing Keyword", result.Title);
    }

    [Fact]
    public async Task AuditSeo_WithFailures_ReturnsLowerScore()
    {
        var html = @"
<html>
    <head>
        <title>Short</title>
    </head>
    <body>
        <h1>No Keyword</h1>
        <img src=""image.jpg"">
    </body>
</html>";

        var result = await SeoTool.AuditSeo(html: html, keyword: "missing");

        Assert.True(result.Score < 100);
        Assert.Contains(result.Checks, c => c.Name == "Title Tag" && c.Status != "pass");
        Assert.Contains(result.Checks, c => c.Name == "H1 Tag" && c.Status != "pass");
        Assert.Contains(result.Checks, c => c.Name == "Image Alt Text" && c.Status != "pass");
    }

    [Fact]
    public void ConvertHugoMarkdownToHtml_WithValidMarkdown_ReturnsHtml()
    {
        var markdown = @"---
title: ""My Hugo Post Title""
description: ""This is a description for the Hugo post that is long enough.""
canonical: ""https://example.com/hugo-post""
---

# Heading 1

This is the body content.
[Link](https://example.com)
";

        var html = SeoTool.ConvertHugoMarkdownToHtml(markdown);

        Assert.Contains("<title>My Hugo Post Title</title>", html);
        Assert.Contains(@"content=""This is a description for the Hugo post that is long enough.""", html);
        Assert.Contains("<h1>Heading 1</h1>", html);
    }

    [Fact]
    public async Task AuditSeo_WithoutUrlOrHtml_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => SeoTool.AuditSeo());
    }
}
