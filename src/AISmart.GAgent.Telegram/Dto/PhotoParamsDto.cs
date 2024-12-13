using System.Collections.Generic;
using Newtonsoft.Json;

namespace AISmart.Dto;

public class PhotoParamsDto
{
    [JsonProperty("business_connection_id")]
    public string? BusinessConnectionId { get; set; }

    [JsonProperty("chat_id")]
    public string ChatId { get; set; }

    [JsonProperty("message_thread_id")]
    public int? MessageThreadId { get; set; }

    [JsonProperty("photo")]
    public string Photo { get; set; }

    [JsonProperty("caption")]
    public string? Caption { get; set; }

    [JsonProperty("parse_mode")]
    public string? ParseMode { get; set; }

    [JsonProperty("caption_entities")]
    public List<MessageEntity>? CaptionEntities { get; set; }

    [JsonProperty("show_caption_above_media")]
    public bool? ShowCaptionAboveMedia { get; set; }

    [JsonProperty("has_spoiler")]
    public bool? HasSpoiler { get; set; }

    [JsonProperty("disable_notification")]
    public bool? DisableNotification { get; set; }

    [JsonProperty("protect_content")]
    public bool? ProtectContent { get; set; }

    [JsonProperty("allow_paid_broadcast")]
    public bool? AllowPaidBroadcast { get; set; }

    [JsonProperty("message_effect_id")]
    public string? MessageEffectId { get; set; }

    [JsonProperty("reply_parameters")]
    public ReplyParamDto? ReplyParameters { get; set; }
}