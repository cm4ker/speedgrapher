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
/// Haiku prompt handler.
/// </summary>
public static class HaikuPrompt
{
    [McpServerPrompt(Name = "haiku"), Description("Creates a haiku about a given topic, or infers the topic from the current conversation.")]
    public static IList<PromptMessage> GetHaiku(
        [Description("The topic for the haiku. If not provided, the model will infer it from the conversation.")] string? topic = null)
    {
        var prompt = string.IsNullOrEmpty(topic)
            ? "Write a haiku (following the strict 5-7-5 syllable structure) about the main subject of our conversation."
            : $"The user wants to have some fun and has requested a haiku (following the strict 5-7-5 syllable structure) about the following topic: {topic}";

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
