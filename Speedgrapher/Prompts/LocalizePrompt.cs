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
/// Localize prompt handler.
/// </summary>
[McpServerPromptType]
public static class LocalizePrompt
{
    private const string DefaultLocalizeText = @"
You are a localization specialist. Your task is to translate an article into a target language while strictly adhering to our localization guidelines.

You must follow these rules:

**1. Do Not Translate Technical Terms:**
   - All technical computer science and software engineering terms must remain in English. This is not an exhaustive list; use your best judgment for similar jargon.
   - Examples: `API`, `backend`, `CLI`, `commit`, `database`, `frontend`, `JSON`, `LLM`, `prompt`, `pull request`, `repository`, `SDK`, `server`, `SSH`.

**2. Do Not Translate Product & Brand Names:**
   - All product, company, and brand names must remain in their original form.
   - Examples: `Claude`, `Gemini CLI`, `Go`, `GoDoctor`, `Google Cloud`, `Jules`, `osquery`.

**3. Maintain Formatting:**
   - Preserve all markdown formatting, including headings, lists, bold/italic text, and links.
   - Do not translate content within code blocks (```). Comments within code may be translated.
   - Keep all URLs and links unchanged.

**4. Tone and Style:**
   - Review existing articles in the target language to match the established professional yet approachable tone.";

    private static string? _guidelinePath;

    /// <summary>
    /// Sets the path to the localization guidelines file.
    /// </summary>
    public static void SetGuidelinePath(string path)
    {
        _guidelinePath = path;
    }

    [McpServerPrompt(Name = "localize"), Description("Translates the article currently being worked on into a target language.")]
    public static IList<PromptMessage> GetLocalize(
        [Description("The language to translate the article into.")] string target_language)
    {
        var guidelines = DefaultLocalizeText;

        if (!string.IsNullOrEmpty(_guidelinePath) && File.Exists(_guidelinePath))
        {
            guidelines = File.ReadAllText(_guidelinePath);
        }

        var prompt = $"Translate the work-in-progress article currently in your context into {target_language}. You must follow the localization guidelines provided.";

        return
        [
            new()
            {
                Role = Role.User,
                Content = new TextContentBlock { Text = guidelines + "\n\n" + prompt }
            }
        ];
    }
}
