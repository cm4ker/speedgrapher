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
/// Readability prompt handler.
/// </summary>
public static class ReadabilityPrompt
{
    private const string ReadabilityText = @"**Objective: Evaluate Readability**

You are an expert editor. Your task is to analyze the work-in-progress article currently in your context and assess its readability using the Gunning Fog Index.

**Analysis Steps:**

1.  **Identify the Text:** Use the most recent, complete text block you generated in this session as the source material.
2.  **Assess Current Readability:** Use the `fog` tool to calculate the current Gunning Fog Index and classification for the text.

**Your Task:**

Now, execute the plan. First, call the `fog` tool on the text you just wrote. Then, provide your analysis.";

    [McpServerPrompt(Name = "readability"), Description("Analyzes the last generated text for readability using the Gunning Fog Index.")]
    public static IList<PromptMessage> GetReadability()
    {
        return
        [
            new()
            {
                Role = Role.Assistant,
                Content = new TextContentBlock { Text = ReadabilityText }
            }
        ];
    }
}
