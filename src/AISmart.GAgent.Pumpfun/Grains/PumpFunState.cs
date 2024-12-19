using System;
using Orleans;

namespace AISmart.Grains;
[GenerateSerializer]
public class PumpFunState
{
    [Id(0)]  public  Guid Id { get; set; }
    [Id(1)] public string requestMessageId { get; set; }
    [Id(3)] public  string requestMessage { get; set; }
}