using System.Collections.Generic;
using Newtonsoft.Json;

namespace AISmart.Dto;

public class ReplyParamDto
{
    [JsonProperty("message_id")] public long MessageId { get; set; }
    [JsonProperty("chat_id")] public long? ChainId { get; set; }

    [JsonProperty("allow_sending_without_reply")]
    public bool? AllowSendingWithoutReply { get; set; }

    [JsonProperty("quote")] public string? Quote { get; set; }
    [JsonProperty("quote_parse_mode")] public string? QuoteParseMode { get; set; }
    [JsonProperty("quote_entities")] public List<MessageEntity>? QuoteEntities { get; set; }
    [JsonProperty("quote_position")] public int? QuotePosition { get; set; }
}

public class MessageEntity
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("offset")] public int Offset { get; set; }
    [JsonProperty("length")] public int Length { get; set; }
    [JsonProperty("url")] public string? Url { get; set; }
    [JsonProperty("user")] public User? User { get; set; }
    [JsonProperty("language")] public string? Language { get; set; }
    [JsonProperty("emoji")] public string? CustomEmojiId { get; set; }
}