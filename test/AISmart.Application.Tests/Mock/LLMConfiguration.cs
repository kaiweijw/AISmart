// Copyright (c) Microsoft Corporation. All rights reserved.
// LLMConfiguration.cs

using System;
using OpenAI;
using OpenAI.Chat;

namespace AutoGen.BasicSample;

internal static class LLMConfiguration
{
    public static ChatClient GetOpenAIGPT4o_mini()
    {
        var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("Please set OPENAI_API_KEY environment variable.");
        var modelId = "gpt-4o-mini";

        return new OpenAIClient(openAIKey).GetChatClient(modelId);
    }
}
