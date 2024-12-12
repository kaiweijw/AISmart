using System.Collections.Generic;
using Newtonsoft.Json;

namespace AISmart.Dto;

public class Message
{
    [JsonProperty("message_id")]
    public long MessageId { get; set; }
    
    [JsonProperty("from")]
    public User? From { get; set; }
    
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }
    
    [JsonProperty("date")]
    public long Date { get; set; }
    
    [JsonProperty("text")]
    public string? Text { get; set; }
}

public class User
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }
    
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }
    
    [JsonProperty("username")]
    public string? Username { get; set; }
    
    [JsonProperty("language_code")]
    public string? LanguageCode { get; set; }
}

public class Chat
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }
    
    [JsonProperty("username")]
    public string? Username { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
}

public class MessageEntity
{
    [JsonProperty("type")] 
    public string? Type { get; set; }
    [JsonProperty("offset")] 
    public int Offset { get; set; }
    [JsonProperty("length")] 
    public int Length { get; set; }
    [JsonProperty("url")] 
    public string? Url { get; set; }
    [JsonProperty("user")] 
    public User? User { get; set; }
    [JsonProperty("language")] 
    public string? Language { get; set; }
    [JsonProperty("emoji")] 
    public string? CustomEmojiId { get; set; }
}


public class BusinessConnection
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("user")]
    public User? User { get; set; }

    [JsonProperty("user_chat_id")]
    public long UserChatId { get; set; }

    [JsonProperty("date")]
    public long Date { get; set; }

    [JsonProperty("can_reply")]
    public bool CanReply { get; set; }

    [JsonProperty("is_enabled")]
    public bool IsEnabled { get; set; }
}

public class BusinessMessagesDeleted
{
    [JsonProperty("business_connection_id")]
    public string? BusinessConnectionId { get; set; }

    [JsonProperty("chat")]
    public Chat? Chat { get; set; }

    [JsonProperty("message_ids")]
    public List<long>? MessageIds { get; set; }
}

public class MessageReactionUpdated
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }

    [JsonProperty("message_id")]
    public long MessageId { get; set; }

    [JsonProperty("user")]
    public User? User { get; set; } 

    [JsonProperty("actor_chat")]
    public Chat? ActorChat { get; set; } 

    [JsonProperty("date")]
    public long Date { get; set; } 
    
    [JsonProperty("old_reaction")]
    public List<ReactionType>? OldReaction { get; set; } 
    
    [JsonProperty("new_reaction")]
    public List<ReactionType>? NewReaction { get; set; } 
}

public class ReactionType
{
    [JsonProperty("type")]
    public string? Type { get; set; }
}

public class ReactionTypeEmoji : ReactionType
{
    [JsonProperty("type")]
    public new string Type { get; set; } = "emoji";
    
    [JsonProperty("emoji")]
    public string? Emoji { get; set; } 
}

public class ReactionTypeCustomEmoji : ReactionType
{
    [JsonProperty("type")]
    public new string Type { get; set; } = "custom_emoji";
    
    [JsonProperty("custom_emoji_id")]
    public string? CustomEmojiId { get; set; } 
}

public class ReactionTypePaid : ReactionType
{
    [JsonProperty("type")]
    public new string Type { get; set; } = "paid"; 
}

public class ReactionCountUpdate
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }

    [JsonProperty("message_id")]
    public long MessageId { get; set; }

    [JsonProperty("date")]
    public long Date { get; set; }
    
    [JsonProperty("reactions")]
    public List<ReactionCount>? Reactions { get; set; }
}

public class ReactionCount
{
    [JsonProperty("type")]
    public ReactionType? Type { get; set; }
    
    [JsonProperty("total_count")]
    public long TotalCount { get; set; }
}
public class MessageReactionCountUpdated
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; } 

    [JsonProperty("message_id")]
    public long MessageId { get; set; } 

    [JsonProperty("date")]
    public long Date { get; set; } 

    [JsonProperty("reactions")]
    public List<ReactionCount>? Reactions { get; set; } 
}

public class InlineQuery
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("from")]
    public User? From { get; set; }

    [JsonProperty("query")]
    public string? Query { get; set; }

    [JsonProperty("offset")]
    public string? Offset { get; set; }

    [JsonProperty("chat_type")]
    public string? ChatType { get; set; }

    [JsonProperty("location")]
    public Location? Location { get; set; }
}

public class Location
{
    [JsonProperty("latitude")]
    public float Latitude { get; set; } 

    [JsonProperty("longitude")]
    public float Longitude { get; set; }

    [JsonProperty("horizontal_accuracy")]
    public float? HorizontalAccuracy { get; set; }

    [JsonProperty("live_period")]
    public long? LivePeriod { get; set; }

    [JsonProperty("heading")]
    public long? Heading { get; set; }

    [JsonProperty("proximity_alert_radius")]
    public long? ProximityAlertRadius { get; set; } 
}

public class ChosenInlineResult
{
    [JsonProperty("result_id")]
    public string? ResultId { get; set; }

    [JsonProperty("from")]
    public User? From { get; set; } 

    [JsonProperty("location")]
    public Location Location { get; set; }

    [JsonProperty("inline_message_id")]
    public string? InlineMessageId { get; set; }

    [JsonProperty("query")]
    public string? Query { get; set; }
}
public class CallbackQuery
{
    [JsonProperty("id")]
    public string? Id { get; set; } 

    [JsonProperty("from")]
    public User? From { get; set; } 

    [JsonProperty("message")]
    public MaybeInaccessibleMessage? Message { get; set; }

    [JsonProperty("inline_message_id")]
    public string? InlineMessageId { get; set; }

    [JsonProperty("chat_instance")]
    public string? ChatInstance { get; set; } 

    [JsonProperty("data")]
    public string? Data { get; set; } 

    [JsonProperty("game_short_name")]
    public string? GameShortName { get; set; }  
}
public class MaybeInaccessibleMessage
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("offset")]
    public long Offset { get; set; } 

    [JsonProperty("length")]
    public long Length { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; } 

    [JsonProperty("user")]
    public User? User { get; set; }

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("custom_emoji_id")]
    public string? CustomEmojiId { get; set; } 
}
public class ShippingQuery
{
    [JsonProperty("id")]
    public string? Id { get; set; } 

    [JsonProperty("from")]
    public User? From { get; set; } 

    [JsonProperty("invoice_payload")]
    public string? InvoicePayload { get; set; } 

    [JsonProperty("shipping_address")]
    public ShippingAddress? ShippingAddress { get; set; }
}

public class ShippingAddress
{
    [JsonProperty("country_code")]
    public string? CountryCode { get; set; } 

    [JsonProperty("state")]
    public string? State { get; set; } 

    [JsonProperty("city")]
    public string? City { get; set; }

    [JsonProperty("street_line1")]
    public string? StreetLine1 { get; set; } 
    [JsonProperty("street_line2")]
    public string? StreetLine2 { get; set; } 

    [JsonProperty("post_code")]
    public string? PostCode { get; set; } 
}

public class PreCheckoutQuery
{
    [JsonProperty("id")]
    public string? Id { get; set; } 

    [JsonProperty("from")]
    public User? From { get; set; } 

    [JsonProperty("currency")]
    public string? Currency { get; set; } 

    [JsonProperty("total_amount")]
    public long TotalAmount { get; set; } 

    [JsonProperty("invoice_payload")]
    public string? InvoicePayload { get; set; } 

    [JsonProperty("shipping_option_id")]
    public string? ShippingOptionId { get; set; }

    [JsonProperty("order_info")]
    public OrderInfo? OrderInfo { get; set; }
}

public class OrderInfo
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("shipping_address")]
    public ShippingAddress? ShippingAddress { get; set; } 
}

public class PaidMediaPurchased
{
    [JsonProperty("from")]
    public User? From { get; set; } 

    [JsonProperty("paid_media_payload")]
    public string? PaidMediaPayload { get; set; } 
}

public class Poll
{
    [JsonProperty("id")]
    public string? Id { get; set; } 

    [JsonProperty("question")]
    public string? Question { get; set; } 

    [JsonProperty("question_entities")]
    public List<MessageEntity>? QuestionEntities { get; set; } 

    [JsonProperty("options")]
    public List<PollOption>? Options { get; set; }

    [JsonProperty("total_voter_count")]
    public long TotalVoterCount { get; set; } 

    [JsonProperty("is_closed")]
    public bool IsClosed { get; set; }

    [JsonProperty("is_anonymous")]
    public bool IsAnonymous { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; } 

    [JsonProperty("allows_multiple_answers")]
    public bool AllowsMultipleAnswers { get; set; }

    [JsonProperty("correct_option_id")]
    public long? CorrectOptionId { get; set; } 

    [JsonProperty("explanation")]
    public string? Explanation { get; set; }

    [JsonProperty("explanation_entities")]
    public List<MessageEntity>? ExplanationEntities { get; set; } 

    [JsonProperty("open_period")]
    public long? OpenPeriod { get; set; } 

    [JsonProperty("close_date")]
    public long? CloseDate { get; set; }
}

public class PollOption
{
    [JsonProperty("text")]
    public string? Text { get; set; } 

    [JsonProperty("text_entities")]
    public List<MessageEntity>? TextEntities { get; set; } 

    [JsonProperty("voter_count")]
    public long VoterCount { get; set; } 
}
public class PollAnswer
{
    [JsonProperty("poll_id")]
    public string? PollId { get; set; }

    [JsonProperty("voter_chat")]
    public Chat? VoterChat { get; set; }

    [JsonProperty("user")]
    public User? User { get; set; } 

    [JsonProperty("option_ids")]
    public List<long>? OptionIds { get; set; } 
}
public class ChatMemberUpdated
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }

    [JsonProperty("from")]
    public User? From { get; set; }

    [JsonProperty("date")]
    public int Date { get; set; } 

    [JsonProperty("old_chat_member")]
    public ChatMember? OldChatMember { get; set; } 

    [JsonProperty("new_chat_member")]
    public ChatMember? NewChatMember { get; set; } 

    [JsonProperty("invite_link")]
    public ChatInviteLink? InviteLink { get; set; }

    [JsonProperty("via_join_request")]
    public bool? ViaJoinRequest { get; set; } 

    [JsonProperty("via_chat_folder_invite_link")]
    public bool? ViaChatFolderInviteLink { get; set; } 
}

public class ChatMember
{
    [JsonProperty("status")]
    public string? Status { get; set; }  // The status of the member in the chat

    [JsonProperty("user")]
    public User? User { get; set; }  // Information about the user
}

public class ChatMemberOwner : ChatMember
{
    public ChatMemberOwner()
    {
        Status = "creator";  // The status of the member is always "creator" for the owner
    }

    [JsonProperty("is_anonymous")]
    public bool IsAnonymous { get; set; }  // True if the user's presence in the chat is hidden

    [JsonProperty("custom_title")]
    public string? CustomTitle { get; set; }  // Custom title for this user (optional)
}

public class ChatMemberAdministrator : ChatMember
{
    public ChatMemberAdministrator()
    {
        Status = "administrator";  // The status of the member is always "administrator" for admins
    }

    [JsonProperty("can_be_edited")]
    public bool CanBeEdited { get; set; }  // True if the bot is allowed to edit the admin's privileges

    [JsonProperty("is_anonymous")]
    public bool IsAnonymous { get; set; }  // True if the user's presence in the chat is hidden

    [JsonProperty("can_manage_chat")]
    public bool CanManageChat { get; set; }  // True if the admin can manage the chat (view logs, etc.)

    [JsonProperty("can_delete_messages")]
    public bool CanDeleteMessages { get; set; }  // True if the admin can delete other users' messages

    [JsonProperty("can_manage_video_chats")]
    public bool CanManageVideoChats { get; set; }  // True if the admin can manage video chats

    [JsonProperty("can_restrict_members")]
    public bool CanRestrictMembers { get; set; }  // True if the admin can restrict or ban users

    [JsonProperty("can_promote_members")]
    public bool CanPromoteMembers { get; set; }  // True if the admin can promote other members to admins

    [JsonProperty("can_change_info")]
    public bool CanChangeInfo { get; set; }  // True if the admin can change the chat info (title, photo, etc.)

    [JsonProperty("can_invite_users")]
    public bool CanInviteUsers { get; set; }  // True if the admin can invite new users to the chat

    [JsonProperty("can_post_stories")]
    public bool CanPostStories { get; set; }  // True if the admin can post stories in the chat

    [JsonProperty("can_edit_stories")]
    public bool CanEditStories { get; set; }  // True if the admin can edit others' stories

    [JsonProperty("can_delete_stories")]
    public bool CanDeleteStories { get; set; }  // True if the admin can delete others' stories

    [JsonProperty("can_post_messages")]
    public bool? CanPostMessages { get; set; }  // True if the admin can post messages in channels (only for channels)

    [JsonProperty("can_edit_messages")]
    public bool? CanEditMessages { get; set; }  // True if the admin can edit messages in channels (only for channels)

    [JsonProperty("can_pin_messages")]
    public bool? CanPinMessages { get; set; }  // True if the admin can pin messages (only for groups and supergroups)

    [JsonProperty("can_manage_topics")]
    public bool? CanManageTopics { get; set; }  // True if the admin can manage forum topics (only for supergroups)

    [JsonProperty("custom_title")]
    public string? CustomTitle { get; set; }  // Custom title for the administrator (optional)
}

public class ChatMemberMember : ChatMember
{
    public ChatMemberMember()
    {
        Status = "member";  // The status of the member is always "member" for regular members
    }

    [JsonProperty("until_date")]
    public int? UntilDate { get; set; }  // The Unix timestamp of when the user's subscription will expire (optional)
}

public class ChatMemberRestricted : ChatMember
{
    public ChatMemberRestricted()
    {
        Status = "restricted";  // The status of the member is "restricted" for members with restrictions
    }

    [JsonProperty("is_member")]
    public bool IsMember { get; set; }  // True if the user is still a member of the chat

    [JsonProperty("can_send_messages")]
    public bool CanSendMessages { get; set; }  // True if the user is allowed to send text messages

    [JsonProperty("can_send_audios")]
    public bool CanSendAudios { get; set; }  // True if the user can send audio files

    [JsonProperty("can_send_documents")]
    public bool CanSendDocuments { get; set; }  // True if the user can send documents

    [JsonProperty("can_send_photos")]
    public bool CanSendPhotos { get; set; }  // True if the user can send photos

    [JsonProperty("can_send_videos")]
    public bool CanSendVideos { get; set; }  // True if the user can send videos

    [JsonProperty("can_send_video_notes")]
    public bool CanSendVideoNotes { get; set; }  // True if the user can send video notes

    [JsonProperty("can_send_voice_notes")]
    public bool CanSendVoiceNotes { get; set; }  // True if the user can send voice notes

    [JsonProperty("can_send_polls")]
    public bool CanSendPolls { get; set; }  // True if the user can send polls

    [JsonProperty("can_send_other_messages")]
    public bool CanSendOtherMessages { get; set; }  // True if the user can send other types of messages (e.g., games, stickers)

    [JsonProperty("can_add_web_page_previews")]
    public bool CanAddWebPagePreviews { get; set; }  // True if the user can add web page previews to messages

    [JsonProperty("can_change_info")]
    public bool CanChangeInfo { get; set; }  // True if the user can change chat info (title, photo, etc.)

    [JsonProperty("can_invite_users")]
    public bool CanInviteUsers { get; set; }  // True if the user can invite new users to the chat

    [JsonProperty("can_pin_messages")]
    public bool CanPinMessages { get; set; }  // True if the user can pin messages

    [JsonProperty("can_manage_topics")]
    public bool CanManageTopics { get; set; }  // True if the user can manage topics in supergroups

    [JsonProperty("until_date")]
    public int UntilDate { get; set; }  // Unix timestamp when the restrictions will be lifted (0 means permanent restriction)
}

public class ChatMemberLeft : ChatMember
{
    public ChatMemberLeft()
    {
        Status = "left";  // The status of the member is "left" for users who have left the chat
    }
}

public class ChatMemberBanned : ChatMember
{
    public ChatMemberBanned()
    {
        Status = "kicked";  // The status of the member is "kicked" for users who are banned
    }

    [JsonProperty("until_date")]
    public int UntilDate { get; set; }  // The Unix timestamp when the ban will be lifted (0 means permanent ban)
}

public class ChatInviteLink
{
    [JsonProperty("invite_link")]
    public string? InviteLink { get; set; }  // The invite link. If created by another admin, the second part will be replaced with "…"

    [JsonProperty("creator")]
    public User? Creator { get; set; }  // The creator of the invite link

    [JsonProperty("creates_join_request")]
    public bool CreatesJoinRequest { get; set; }  // True if users need approval to join via this link

    [JsonProperty("is_primary")]
    public bool IsPrimary { get; set; }  // True if this is the primary invite link

    [JsonProperty("is_revoked")]
    public bool IsRevoked { get; set; }  // True if the invite link is revoked

    [JsonProperty("name")]
    public string? Name { get; set; }  // Optional: Name for the invite link

    [JsonProperty("expire_date")]
    public int? ExpireDate { get; set; }  // Optional: Unix timestamp of when the link will expire or has expired

    [JsonProperty("member_limit")]
    public int? MemberLimit { get; set; }  // Optional: Maximum number of users who can join the chat using this link

    [JsonProperty("pending_join_request_count")]
    public int? PendingJoinRequestCount { get; set; }  // Optional: The number of pending join requests using this link

    [JsonProperty("subscription_period")]
    public int? SubscriptionPeriod { get; set; }  // Optional: The subscription period in seconds for the link

    [JsonProperty("subscription_price")]
    public int? SubscriptionPrice { get; set; }  // Optional: The price in Telegram Stars for each subscription period
}

public class ChatJoinRequest
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }  // Chat to which the request was sent

    [JsonProperty("from")]
    public User? From { get; set; }  // User that sent the join request

    [JsonProperty("user_chat_id")]
    public long UserChatId { get; set; }  // Identifier of a private chat with the user who sent the join request

    [JsonProperty("date")]
    public int Date { get; set; }  // Date the request was sent in Unix time

    [JsonProperty("bio")]
    public string? Bio { get; set; }  // Optional bio of the user

    [JsonProperty("invite_link")]
    public ChatInviteLink? InviteLink { get; set; }  // Optional: The invite link used by the user to send the join request
}

public class ChatBoostUpdated
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }  // Chat which was boosted

    [JsonProperty("boost")]
    public ChatBoost? Boost { get; set; }  // Information about the chat boost
}

public class ChatBoostRemoved
{
    [JsonProperty("chat")]
    public Chat? Chat { get; set; }  // Chat which was boosted

    [JsonProperty("boost_id")]
    public string? BoostId { get; set; }  // Unique identifier of the boost

    [JsonProperty("remove_date")]
    public int RemoveDate { get; set; }  // Point in time (Unix timestamp) when the boost was removed

    [JsonProperty("source")]
    public ChatBoostSource? Source { get; set; }  // Source of the removed boost
}

public class ChatBoost
{
    [JsonProperty("boost_id")]
    public string? BoostId { get; set; }  // Unique identifier of the boost

    [JsonProperty("add_date")]
    public int AddDate { get; set; }  // Point in time (Unix timestamp) when the chat was boosted

    [JsonProperty("expiration_date")]
    public int ExpirationDate { get; set; }  // Point in time (Unix timestamp) when the boost will automatically expire

    [JsonProperty("source")]
    public ChatBoostSource? Source { get; set; }  // Source of the added boost
}

public class ChatBoostSource
{
    [JsonProperty("source")]
    public string? Source { get; set; }  // Source of the boost (e.g., "premium", "gift_code", "giveaway")

    [JsonProperty("user")]
    public User? User { get; set; }  // User related to the boost (varies by source type)
}

public class ChatBoostSourcePremium : ChatBoostSource
{
    public ChatBoostSourcePremium()
    {
        Source = "premium";  // Always "premium"
    }

    [JsonProperty("user")]
    public User? User { get; set; }  // User who boosted the chat via Telegram Premium
}

public class ChatBoostSourceGiftCode : ChatBoostSource
{
    public ChatBoostSourceGiftCode()
    {
        Source = "gift_code";  // Always "gift_code"
    }

    [JsonProperty("user")]
    public User? User { get; set; }  // User for whom the gift code was created
}

public class ChatBoostSourceGiveaway : ChatBoostSource
{
    public ChatBoostSourceGiveaway()
    {
        Source = "giveaway"; // Always "giveaway"
    }

    [JsonProperty("giveaway_message_id")]
    public int GiveawayMessageId { get; set; } // Identifier of a message related to the giveaway

    [JsonProperty("user")]
    public User? User { get; set; } // User who won the giveaway (optional, for Telegram Premium giveaways)

    [JsonProperty("prize_star_count")]
    public int PrizeStarCount { get; set; } // Number of Telegram Stars for Telegram Star giveaways

    [JsonProperty("is_unclaimed")]
    public bool? IsUnclaimed { get; set; } // True if the giveaway was completed, but no winner claimed the prize
}

