using System.Collections.Generic;

namespace AISmart.Options;

public class TelegramOptions
{
    public string Webhook { get; set; } 
    public Dictionary<string, string> AccountDictionary { get; set; } = new();
}