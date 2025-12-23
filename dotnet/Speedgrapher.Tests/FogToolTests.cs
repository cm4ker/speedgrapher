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

public class FogToolTests
{
    [Theory]
    [InlineData("This is a sentence. This is another sentence.", 8, 3)]
    [InlineData("", 0, 0)]
    [InlineData("...", 0, 0)]
    [InlineData("  leading and trailing spaces  ", 4, 0)]
    [InlineData("This is a difficult and complicated sentence.", 7, 3)]
    public void CountWords_ReturnsCorrectCounts(string text, int expectedWords, int expectedComplexWords)
    {
        var (words, complexWords) = FogTool.CountWords(text);
        
        Assert.Equal(expectedWords, words);
        Assert.Equal(expectedComplexWords, complexWords);
    }

    [Theory]
    [InlineData("This is a sentence. This is another sentence.", 2)]
    [InlineData("", 0)]
    [InlineData("What is this? I don't know! Let's find out.", 3)]
    [InlineData("this is one long sentence", 0)]
    [InlineData("Hello... world?!", 2)]
    public void CountSentences_ReturnsCorrectCount(string text, int expected)
    {
        var count = FogTool.CountSentences(text);
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData("complex", false)]
    [InlineData("sentence", true)]
    [InlineData("difficult", true)]
    [InlineData("dog", false)]
    [InlineData("understanding", true)]
    [InlineData("beautiful", true)]
    [InlineData("requires", true)]
    public void IsComplexWord_ReturnsCorrectResult(string word, bool expected)
    {
        var result = FogTool.IsComplexWord(word);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("complex", 2)]
    [InlineData("sentence", 3)]
    [InlineData("difficult", 3)]
    [InlineData("dog", 1)]
    [InlineData("cat", 1)]
    [InlineData("created", 2)]
    [InlineData("beautiful", 3)]
    [InlineData("requires", 3)]
    [InlineData("understanding", 4)]
    public void CountSyllables_ReturnsCorrectCount(string word, int expected)
    {
        var count = FogTool.CountSyllables(word);
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData("This is a sentence. This is another sentence.", 16.6)]
    [InlineData("The quick brown fox jumps over the lazy dog.", 3.6)]
    [InlineData("Automated testing is a cornerstone of modern software development.", 21.38)]
    [InlineData("Difficult complicated understanding.", 41.2)]
    [InlineData("123 apples and 456 oranges.", 10.0)]
    public void CalculateFogIndex_ReturnsCorrectValue(string text, double expected)
    {
        var result = FogTool.CalculateFogIndex(text);
        Assert.Equal(expected, result, precision: 9);
    }

    [Fact]
    public void CalculateFogIndex_ThrowsForEmptyText()
    {
        var ex = Assert.Throws<ArgumentException>(() => FogTool.CalculateFogIndex(""));
        Assert.Contains("does not contain any words", ex.Message);
    }

    [Fact]
    public void CalculateFogIndex_ThrowsForNoSentences()
    {
        var ex = Assert.Throws<ArgumentException>(() => FogTool.CalculateFogIndex("just a bunch of words"));
        Assert.Contains("does not contain any sentences", ex.Message);
    }

    [Fact]
    public void CalculateFogIndex_ThrowsForOnlyPunctuation()
    {
        var ex = Assert.Throws<ArgumentException>(() => FogTool.CalculateFogIndex("!!! ?? .."));
        Assert.Contains("does not contain any words", ex.Message);
    }

    [Theory]
    [InlineData(23.0, FogTool.FogCategoryUnreadable)]
    [InlineData(22.0, FogTool.FogCategoryUnreadable)]
    [InlineData(20.0, FogTool.FogCategoryHardToRead)]
    [InlineData(18.0, FogTool.FogCategoryHardToRead)]
    [InlineData(15.0, FogTool.FogCategoryProfessional)]
    [InlineData(13.0, FogTool.FogCategoryProfessional)]
    [InlineData(10.0, FogTool.FogCategoryGeneral)]
    [InlineData(9.0, FogTool.FogCategoryGeneral)]
    [InlineData(8.0, FogTool.FogCategorySimplistic)]
    public void ClassifyFogIndex_ReturnsCorrectCategory(double index, string expected)
    {
        var result = FogTool.ClassifyFogIndex(index);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateFog_ReturnsValidResult()
    {
        var text = "This is a sentence. This is another sentence.";
        var result = FogTool.CalculateFogInternal(text);
        
        Assert.Equal(16.6, result.FogIndex);
        Assert.Equal(FogTool.FogCategoryProfessional, result.Classification);
        Assert.Equal(8, result.TotalWords);
        Assert.Equal(2, result.TotalSentences);
        Assert.Equal(4.0, result.AverageSentenceLength);
        Assert.Equal(37.5, result.PercentageComplexWords);
        Assert.Equal(3, result.ComplexWords);
    }

    [Fact]
    public void CalculateFog_WikipediaExample()
    {
        var text = "The quick brown fox jumps over the lazy dog.";
        var tool = new FogTool();
        var callResult = tool.CalculateFog(text);
        var json = ((ModelContextProtocol.Protocol.TextContentBlock)callResult.Content[0]).Text;
        var result = System.Text.Json.JsonSerializer.Deserialize<FogResult>(json);
        
        Assert.Equal(3.6, result.FogIndex);
        Assert.Equal(FogTool.FogCategorySimplistic, result.Classification);
        Assert.Equal(9, result.TotalWords);
        Assert.Equal(1, result.TotalSentences);
        Assert.Equal(9.0, result.AverageSentenceLength);
        Assert.Equal(0.0, result.PercentageComplexWords);
        Assert.Equal(0, result.ComplexWords);
    }
}
