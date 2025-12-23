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
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;

namespace Speedgrapher.Tools;

/// <summary>
/// Provides functionality for calculating the Gunning Fog Index of text.
/// The Gunning Fog Index is a readability test that estimates the years of formal
/// education a person needs to understand a text on the first reading.
/// </summary>
[McpServerToolType]
public partial class FogTool
{
    public const string FogCategoryUnreadable = "Unreadable: Likely incomprehensible to most readers.";
    public const string FogCategoryHardToRead = "Hard to Read: Requires significant effort, even for experts.";
    public const string FogCategoryProfessional = "Professional Audiences: Best for readers with specialized knowledge.";
    public const string FogCategoryGeneral = "General Audiences: Clear and accessible for most readers.";
    public const string FogCategorySimplistic = "Simplistic: May be perceived as childish or overly simple.";

    /// <summary>
    /// Calculates the Gunning Fog Index to estimate the readability of an English text.
    /// Lower scores indicate easier reading.
    /// </summary>
    /// <param name="text">The text to analyze for readability. Must contain at least one sentence.</param>
    /// <returns>A FogResult containing the fog index, classification, and detailed statistics.</returns>
    [McpServerTool(Name = "fog"), Description("Calculates the Gunning Fog Index to estimate the readability of an English text. Lower scores indicate easier reading.")]
    public CallToolResult CalculateFog(
        [Description("The text to analyze for readability. Must contain at least one sentence.")] string text)
    {
        var result = CalculateFogInternal(text);
        var json = JsonSerializer.Serialize(result);
        return new CallToolResult { Content = new List<ContentBlock> { new TextContentBlock { Text = json } } };
    }

    /// <summary>
    /// Internal method for calculating Fog Index.
    /// </summary>
    public static FogResult CalculateFogInternal(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        var (totalWords, complexWords) = CountWords(text);
        var totalSentences = CountSentences(text);

        if (totalWords == 0)
        {
            throw new ArgumentException("Text does not contain any words", nameof(text));
        }
        if (totalSentences == 0)
        {
            throw new ArgumentException("Text does not contain any sentences", nameof(text));
        }

        var averageSentenceLength = (double)totalWords / totalSentences;
        var percentageComplexWords = 100.0 * complexWords / totalWords;

        var index = 0.4 * (averageSentenceLength + percentageComplexWords);
        index = Math.Round(index * 100) / 100;

        var classification = ClassifyFogIndex(index);

        return new FogResult
        {
            FogIndex = index,
            Classification = classification,
            TotalWords = totalWords,
            TotalSentences = totalSentences,
            AverageSentenceLength = Math.Round(averageSentenceLength * 100) / 100,
            PercentageComplexWords = Math.Round(percentageComplexWords * 100) / 100,
            ComplexWords = complexWords
        };
    }

    /// <summary>
    /// Counts the number of words and complex words in a given text.
    /// Removes all punctuation and then counts words based on spaces.
    /// </summary>
    public static (int TotalWords, int ComplexWords) CountWords(string text)
    {
        // Remove all punctuation
        var cleanText = PunctuationRegex().Replace(text, "");
        var words = cleanText.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

        var complexWordCount = 0;
        foreach (var word in words)
        {
            if (IsComplexWord(word))
            {
                complexWordCount++;
            }
        }

        return (words.Length, complexWordCount);
    }

    /// <summary>
    /// Counts the number of sentences in a given text.
    /// It counts sentences by looking for sentence-ending punctuation (. ! ?).
    /// </summary>
    public static int CountSentences(string text)
    {
        var sentences = SentenceEndingRegex().Matches(text);
        return sentences.Count;
    }

    /// <summary>
    /// Determines if a word is "complex" by checking if it has three or more syllables.
    /// </summary>
    public static bool IsComplexWord(string word)
    {
        return CountSyllables(word) >= 3;
    }

    /// <summary>
    /// Estimates the number of syllables in a word by counting vowel groups.
    /// This is a simplified heuristic.
    /// </summary>
    public static int CountSyllables(string word)
    {
        word = word.ToLower();
        var vowelGroups = VowelGroupRegex().Matches(word);
        var syllableCount = vowelGroups.Count;

        if (syllableCount == 0)
        {
            return 1;
        }

        return syllableCount;
    }

    /// <summary>
    /// Calculates the Gunning Fog Index for a given text, rounded to two decimal places.
    /// </summary>
    public static double CalculateFogIndex(string text)
    {
        var (totalWords, complexWords) = CountWords(text);
        var totalSentences = CountSentences(text);

        if (totalWords == 0)
        {
            throw new ArgumentException("Text does not contain any words", nameof(text));
        }
        if (totalSentences == 0)
        {
            throw new ArgumentException("Text does not contain any sentences", nameof(text));
        }

        var averageSentenceLength = (double)totalWords / totalSentences;
        var percentageComplexWords = 100.0 * complexWords / totalWords;

        var index = 0.4 * (averageSentenceLength + percentageComplexWords);
        return Math.Round(index * 100) / 100;
    }

    /// <summary>
    /// Classifies the Gunning Fog Index into a readability category.
    /// </summary>
    public static string ClassifyFogIndex(double index)
    {
        return index switch
        {
            >= 22 => FogCategoryUnreadable,
            >= 18 => FogCategoryHardToRead,
            >= 13 => FogCategoryProfessional,
            >= 9 => FogCategoryGeneral,
            _ => FogCategorySimplistic
        };
    }

    [GeneratedRegex(@"[\p{P}]")]
    private static partial Regex PunctuationRegex();

    [GeneratedRegex(@"[.!?]+")]
    private static partial Regex SentenceEndingRegex();

    [GeneratedRegex("[aeiouyаеёиоуыэюя]+")]
    private static partial Regex VowelGroupRegex();
}

/// <summary>
/// Represents the result of a Gunning Fog Index calculation.
/// </summary>
public class FogResult
{
    public double FogIndex { get; set; }
    public string Classification { get; set; } = string.Empty;
    public int TotalWords { get; set; }
    public int TotalSentences { get; set; }
    public double AverageSentenceLength { get; set; }
    public double PercentageComplexWords { get; set; }
    public int ComplexWords { get; set; }
}
