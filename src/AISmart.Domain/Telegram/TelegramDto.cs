using System.Collections.Generic;
using AISmart.Dto;
using Newtonsoft.Json;

namespace AISmart.Telegram;

public class GetUpdatedDto
{
    public bool Ok { get; set; }
    public List<TelegramUpdateDto> Result { get; set; }
}
public class TelegramUpdateDto 
{
    [JsonProperty("update_id")]
    public long UpdateId { get; set; } // Update's unique identifier
    
    [JsonProperty("message")]
    public Message? Message { get; set; } // New incoming message (optional)
    
    [JsonProperty("edited_message")]
    public Message? EditedMessage { get; set; } // Edited message (optional)
    
    [JsonProperty("channel_post")]
    public Message? ChannelPost { get; set; } // New incoming channel post (optional)
    
    [JsonProperty("edited_channel_post")]
    public Message? EditedChannelPost { get; set; } // Edited channel post (optional)
    
    [JsonProperty("business_connection")]
    public BusinessConnection? BusinessConnection { get; set; } // Bot connected to/disconnected from a business account (optional)
    
    [JsonProperty("business_message")]
    public Message? BusinessMessage { get; set; } // New message from a connected business account (optional)
    
    [JsonProperty("edited_business_message")]
    public Message? EditedBusinessMessage { get; set; } // Edited message from a connected business account (optional)
    
    [JsonProperty("deleted_business_messages")]
    public BusinessMessagesDeleted? DeletedBusinessMessages { get; set; } // Messages deleted from a business account (optional)
    
    [JsonProperty("message_reaction")]
    public MessageReactionUpdated? MessageReaction { get; set; } // Reaction to a message updated (optional)
    
    [JsonProperty("message_reaction_count")]
    public MessageReactionCountUpdated? MessageReactionCount { get; set; } // Reactions to a message updated (optional)
    
    [JsonProperty("inline_query")]
    public InlineQuery? InlineQuery { get; set; } // New incoming inline query (optional)
    
    [JsonProperty("chosen_inline_result")]
    public ChosenInlineResult? ChosenInlineResult { get; set; } // Result of an inline query chosen by a user (optional)
    
    [JsonProperty("callback_query")]
    public CallbackQuery? CallbackQuery { get; set; } // New incoming callback query (optional)
    
    [JsonProperty("shipping_query")]
    public ShippingQuery? ShippingQuery { get; set; } // Incoming shipping query (optional)
    
    [JsonProperty("pre_checkout_query")]
    public PreCheckoutQuery? PreCheckoutQuery { get; set; } // Incoming pre-checkout query (optional)
    
    [JsonProperty("purchased_paid_media")]
    public PaidMediaPurchased? PurchasedPaidMedia { get; set; } // User purchased paid media (optional)
    
    [JsonProperty("poll")]
    public Poll? Poll { get; set; } // New poll state (optional)
    
    [JsonProperty("poll_answer")]
    public PollAnswer? PollAnswer { get; set; } // User changed their answer in a non-anonymous poll (optional)
    
    [JsonProperty("my_chat_member")]
    public ChatMemberUpdated? MyChatMember { get; set; } // Bot's chat member status was updated (optional)
    
    [JsonProperty("chat_member")]
    public ChatMemberUpdated? ChatMember { get; set; } // A chat member's status was updated (optional)
    
    [JsonProperty("chat_join_request")]
    public ChatJoinRequest? ChatJoinRequest { get; set; } // Request to join the chat (optional)
    
    [JsonProperty("chat_boost")]
    public ChatBoostUpdated? ChatBoost { get; set; } // A chat boost was added or changed (optional)

    [JsonProperty("removed_chat_boost")] 
    public ChatBoostRemoved? RemovedChatBoost { get; set; }
}
