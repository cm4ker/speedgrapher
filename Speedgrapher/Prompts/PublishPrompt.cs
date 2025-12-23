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
/// Publish prompt handler.
/// </summary>
public static class PublishPrompt
{
    private const string PublishText = @"Act as an expert technical editor. The work-in-progress article currently in your context is ready to be published. Please initiate the publishing process.

**Your Task:**
1.  **Determine Workflow:** Inspect the project's 'README.md' or other documentation to identify the established publishing or deployment workflow.
2.  **Create a Plan:** Based on your findings, create a step-by-step plan to publish the article. This often involves git operations (add, commit, push).
3.  **Seek Confirmation:** Present this plan to me and ask for explicit confirmation before executing any of the steps, especially those that modify the remote repository.

Please proceed with determining the workflow and creating the plan.";

    [McpServerPrompt(Name = "publish"), Description("Publishes the final version of the article.")]
    public static IList<PromptMessage> GetPublish()
    {
        return
        [
            new()
            {
                Role = Role.User,
                Content = new TextContentBlock { Text = PublishText }
            }
        ];
    }
}
