using System.Text.Json;
using System.Text.Json.Serialization;
using AutoGen.Core;

namespace AISmart.AutoGen.Tests.Common;

public class AelfBlockChain
{
    /// <summary>
    /// buy token
    /// </summary>
    /// <param name="symbol">token symbol</param>
    /// <returns></returns>
    [Function]
    public async Task<string> BuyToken(string symbol)
    {
        return "get 100 million elf";
    }
    
    public FunctionContract BuyTokenContract
    {
        get => new FunctionContract
        {
            ClassName = @"AelfBlockChain",
            Name = @"BuyToken",
            Description = @"buy token.",
            ReturnType = typeof(Task<string>),
            Parameters = new global::AutoGen.Core.FunctionParameterContract[]
            {
                new FunctionParameterContract
                {
                    Name = @"symbol",
                    Description = @"token symbol",
                    ParameterType = typeof(string),
                    IsRequired = true,
                },
            },
        };
    }
    
    public Task<string> BuyTokenWrapper(string arguments)
    {
        var schema = JsonSerializer.Deserialize<BuyTokenSchema>(
            arguments,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        return BuyToken(schema.symbol);
    }
    
    private class BuyTokenSchema
    {
        [JsonPropertyName(@"symbol")] public string symbol { get; set; }
    }
}