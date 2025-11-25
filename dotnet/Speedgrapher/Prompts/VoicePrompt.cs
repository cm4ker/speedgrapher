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
/// Voice prompt handler.
/// </summary>
public static class VoicePrompt
{
    [McpServerPrompt(Name = "voice"), Description("Analyzes the voice and tone of the user's writing to replicate it in generated text.")]
    public static IList<PromptMessage> GetVoice(
        [Description("Optional hint to locate the reference content (file, path, glob, or URL).")] string? hint = null)
    {
        var prompt = string.IsNullOrEmpty(hint)
            ? "Please analyze the voice and tone of my writing by discovering my content within the current project. Look for my blog articles, book chapters, or other written materials. I want you to replicate my style in future generated text. Analyze a representative sample of my writing that you can find."
            : $"Please analyze the voice and tone of my writing. The user has provided the following hint to help you locate the content: '{hint}'. Please interpret this hint to find the relevant materials (it could be a file path, a glob pattern, a URL, or just a description). Analyze a representative sample of my writing that you can find based on this hint so you can replicate my style in future generated text.";

        return
        [
            new()
            {
                Role = Role.User,
                Content = new TextContentBlock { Text = prompt }
            }
        ];
    }
}
