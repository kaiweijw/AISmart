using System.Collections.Generic;
using Newtonsoft.Json;

namespace AISmart.Dto;

public class Message
{
    [JsonProperty("message_id")]
    public long MessageId { get; set; }

    [JsonProperty("message_thread_id")]
    public long? MessageThreadId { get; set; }

    [JsonProperty("from")]
    public User? From { get; set; }

    [JsonProperty("sender_chat")]
    public Chat? SenderChat { get; set; }

    [JsonProperty("sender_boost_count")]
    public int? SenderBoostCount { get; set; }

    [JsonProperty("sender_business_bot")]
    public User? SenderBusinessBot { get; set; }

    [JsonProperty("date")]
    public long Date { get; set; }

    [JsonProperty("business_connection_id")]
    public string? BusinessConnectionId { get; set; }

    [JsonProperty("chat")]
    public Chat Chat { get; set; }

    [JsonProperty("forward_origin")]
    public MessageOrigin? ForwardOrigin { get; set; }

    [JsonProperty("is_topic_message")]
    public bool? IsTopicMessage { get; set; }

    [JsonProperty("is_automatic_forward")]
    public bool? IsAutomaticForward { get; set; }

    [JsonProperty("reply_to_message")]
    public Message ReplyToMessage { get; set; }

    [JsonProperty("external_reply")]
    public ExternalReplyInfo ExternalReply { get; set; }

    [JsonProperty("quote")]
    public TextQuote Quote { get; set; }

    [JsonProperty("via_bot")]
    public User ViaBot { get; set; }

    [JsonProperty("edit_date")]
    public int? EditDate { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("entities")]
    public List<MessageEntity> Entities { get; set; }

    [JsonProperty("animation")]
    public Animation Animation { get; set; }

    [JsonProperty("audio")]
    public Audio Audio { get; set; }

    [JsonProperty("document")]
    public Document Document { get; set; }

    [JsonProperty("photo")]
    public List<PhotoSize> Photo { get; set; }

    [JsonProperty("sticker")]
    public Sticker Sticker { get; set; }

    [JsonProperty("video")]
    public Video Video { get; set; }

    [JsonProperty("caption")]
    public string Caption { get; set; }

    [JsonProperty("caption_entities")]
    public List<MessageEntity> CaptionEntities { get; set; }

    [JsonProperty("contact")]
    public Contact Contact { get; set; }

    [JsonProperty("poll")]
    public Poll Poll { get; set; }

    [JsonProperty("new_chat_members")]
    public List<User> NewChatMembers { get; set; }

    [JsonProperty("left_chat_member")]
    public User LeftChatMember { get; set; }

    [JsonProperty("new_chat_title")]
    public string NewChatTitle { get; set; }

    [JsonProperty("new_chat_photo")]
    public List<PhotoSize> NewChatPhoto { get; set; }

    [JsonProperty("delete_chat_photo")]
    public bool? DeleteChatPhoto { get; set; }

    [JsonProperty("group_chat_created")]
    public bool? GroupChatCreated { get; set; }

    [JsonProperty("supergroup_chat_created")]
    public bool? SupergroupChatCreated { get; set; }

    [JsonProperty("channel_chat_created")]
    public bool? ChannelChatCreated { get; set; }

    [JsonProperty("pinned_message")]
    public Message PinnedMessage { get; set; }
}

public class User
{
    [JsonProperty("id")]
    public long Id { get; set; }  // Unique identifier for this user or bot. Use long for safe handling of large values.

    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }  // True, if this user is a bot

    [JsonProperty("first_name")]
    public string FirstName { get; set; }  // User's or bot's first name

    [JsonProperty("last_name")]
    public string? LastName { get; set; }  // Optional. User's or bot's last name

    [JsonProperty("username")]
    public string? Username { get; set; }  // Optional. User's or bot's username

    [JsonProperty("language_code")]
    public string? LanguageCode { get; set; }  // Optional. IETF language tag of the user's language

    [JsonProperty("is_premium")]
    public bool? IsPremium { get; set; }  // Optional. True, if this user is a Telegram Premium user

    [JsonProperty("added_to_attachment_menu")]
    public bool? AddedToAttachmentMenu { get; set; }  // Optional. True, if this user added the bot to the attachment menu

    [JsonProperty("can_join_groups")]
    public bool? CanJoinGroups { get; set; }  // Optional. True, if the bot can be invited to groups. Returned only in getMe.

    [JsonProperty("can_read_all_group_messages")]
    public bool? CanReadAllGroupMessages { get; set; }  // Optional. True, if privacy mode is disabled for the bot. Returned only in getMe.

    [JsonProperty("supports_inline_queries")]
    public bool? SupportsInlineQueries { get; set; }  // Optional. True, if the bot supports inline queries. Returned only in getMe.

    [JsonProperty("can_connect_to_business")]
    public bool? CanConnectToBusiness { get; set; }  // Optional. True, if the bot can be connected to a Telegram Business account to receive its messages. Returned only in getMe.

    [JsonProperty("has_main_web_app")]
    public bool? HasMainWebApp { get; set; }  // Optional. True, if the bot has a main Web App. Returned only in getMe.
}

public class Chat
{
    [JsonProperty("id")]
    public long Id { get; set; }  // Unique identifier for this chat. Use long for safe handling of large values.

    [JsonProperty("type")]
    public string Type { get; set; }  // Type of the chat: "private", "group", "supergroup", or "channel"

    [JsonProperty("title")]
    public string? Title { get; set; }  // Optional. Title, for supergroups, channels, and group chats

    [JsonProperty("username")]
    public string? Username { get; set; }  // Optional. Username, for private chats, supergroups, and channels if available

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }  // Optional. First name of the other party in a private chat

    [JsonProperty("last_name")]
    public string? LastName { get; set; }  // Optional. Last name of the other party in a private chat

    [JsonProperty("is_forum")]
    public bool? IsForum { get; set; }  // Optional. True if the supergroup chat is a forum (has topics enabled)
}

public class ExternalReplyInfo
{
    [JsonProperty("origin")]
    public MessageOrigin Origin { get; set; }

    [JsonProperty("chat")]
    public Chat? Chat { get; set; }

    [JsonProperty("message_id")]
    public int? MessageId { get; set; } 

    [JsonProperty("link_preview_options")]
    public LinkPreviewOptions? LinkPreviewOptions { get; set; }

    [JsonProperty("animation")]
    public Animation? Animation { get; set; }

    [JsonProperty("audio")]
    public Audio? Audio { get; set; }

    [JsonProperty("document")]
    public Document? Document { get; set; }

    [JsonProperty("paid_media")]
    public PaidMediaInfo? PaidMedia { get; set; }

    [JsonProperty("photo")]
    public List<PhotoSize>? Photo { get; set; }

    [JsonProperty("sticker")]
    public Sticker? Sticker { get; set; }

    [JsonProperty("story")]
    public Story? Story { get; set; }

    [JsonProperty("video")]
    public Video? Video { get; set; }

    [JsonProperty("video_note")]
    public VideoNote? VideoNote { get; set; }

    [JsonProperty("voice")]
    public Voice? Voice { get; set; }

    [JsonProperty("has_media_spoiler")]
    public bool? HasMediaSpoiler { get; set; } // Optional: Nullable 值类型

    [JsonProperty("contact")]
    public Contact? Contact { get; set; }

    [JsonProperty("dice")]
    public Dice? Dice { get; set; }

    [JsonProperty("game")]
    public Game? Game { get; set; }

    [JsonProperty("giveaway")]
    public Giveaway? Giveaway { get; set; }

    [JsonProperty("giveaway_winners")]
    public GiveawayWinners? GiveawayWinners { get; set; }

    [JsonProperty("invoice")]
    public Invoice? Invoice { get; set; }

    [JsonProperty("location")]
    public Location? Location { get; set; }

    [JsonProperty("poll")]
    public Poll? Poll { get; set; }

    [JsonProperty("venue")]
    public Venue? Venue { get; set; }
}

public class TextQuote
{
    [JsonProperty("text")]
    public string Text { get; set; }  // Text of the quoted part of a message that is replied to by the given message

    [JsonProperty("entities")]
    public List<MessageEntity>? Entities { get; set; }  // Optional. Special entities that appear in the quote

    [JsonProperty("position")]
    public int Position { get; set; }  // Approximate quote position in the original message in UTF-16 code units

    [JsonProperty("is_manual")]
    public bool? IsManual { get; set; }  // Optional. True, if the quote was chosen manually by the message sender
}

public class LinkPreviewOptions
{
    [JsonProperty("is_disabled")]
    public bool? IsDisabled { get; set; } // Optional: True if the link preview is disabled

    [JsonProperty("url")]
    public string? Url { get; set; } // Optional: URL to use for the link preview

    [JsonProperty("prefer_small_media")]
    public bool? PreferSmallMedia { get; set; } // Optional: True if the media should be shrunk in the link preview

    [JsonProperty("prefer_large_media")]
    public bool? PreferLargeMedia { get; set; } // Optional: True if the media should be enlarged in the link preview

    [JsonProperty("show_above_text")]
    public bool? ShowAboveText { get; set; } // Optional: True if the link preview should appear above the message text
}

public class Animation
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for this file, used to download or reuse the file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for this file, constant across time and different bots

    [JsonProperty("width")]
    public int Width { get; set; } // Video width as defined by the sender

    [JsonProperty("height")]
    public int Height { get; set; } // Video height as defined by the sender

    [JsonProperty("duration")]
    public int Duration { get; set; } // Duration of the video in seconds as defined by the sender

    [JsonProperty("thumbnail")]
    public PhotoSize? Thumbnail { get; set; } // Optional: Animation thumbnail as defined by the sender

    [JsonProperty("file_name")]
    public string? FileName { get; set; } // Optional: Original animation filename as defined by the sender

    [JsonProperty("mime_type")]
    public string? MimeType { get; set; } // Optional: MIME type of the file as defined by the sender

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes, can exceed 2^31
}
public class Audio
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for this file, used to download or reuse the file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for this file, constant across time and different bots

    [JsonProperty("duration")]
    public long Duration { get; set; } // Duration of the audio in seconds as defined by the sender

    [JsonProperty("performer")]
    public string Performer { get; set; } // Optional: Performer of the audio as defined by the sender or by audio tags

    [JsonProperty("title")]
    public string Title { get; set; } // Optional: Title of the audio as defined by the sender or by audio tags

    [JsonProperty("file_name")]
    public string FileName { get; set; } // Optional: Original filename as defined by the sender

    [JsonProperty("mime_type")]
    public string MimeType { get; set; } // Optional: MIME type of the file as defined by the sender

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes, stored as long for large files

    [JsonProperty("thumbnail")]
    public PhotoSize Thumbnail { get; set; } // Optional: Thumbnail of the album cover to which the music file belongs
}

public class Document
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for this file, used to download or reuse the file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for this file, constant across time and different bots

    [JsonProperty("thumbnail")]
    public PhotoSize Thumbnail { get; set; } // Optional: Document thumbnail as defined by the sender

    [JsonProperty("file_name")]
    public string FileName { get; set; } // Optional: Original filename as defined by the sender

    [JsonProperty("mime_type")]
    public string MimeType { get; set; } // Optional: MIME type of the file as defined by the sender

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes, stored as long for large files
}
public class PaidMediaInfo
{
    [JsonProperty("star_count")]
    public long StarCount { get; set; } // Number of Telegram Stars required to access the media

    [JsonProperty("paid_media")]
    public List<PaidMedia> PaidMedia { get; set; } // Information about the paid media
}

public abstract class PaidMedia
{
    [JsonProperty("type")]
    public string Type { get; set; } // Type of the paid media (preview, photo, or video)
}

public class PaidMediaPreview : PaidMedia
{
    [JsonProperty("width")]
    public long? Width { get; set; } // Media width as defined by the sender (optional)

    [JsonProperty("height")]
    public long? Height { get; set; } // Media height as defined by the sender (optional)

    [JsonProperty("duration")]
    public long? Duration { get; set; } // Duration of the media in seconds as defined by the sender (optional)

    public PaidMediaPreview()
    {
        Type = "preview";
    }
}

public class PaidMediaPhoto : PaidMedia
{
    [JsonProperty("photo")]
    public List<PhotoSize> Photo { get; set; } // List of PhotoSize objects representing the photo

    public PaidMediaPhoto()
    {
        Type = "photo";
    }
}

public class PaidMediaVideo : PaidMedia
{
    [JsonProperty("video")]
    public Video Video { get; set; } // The video (Video object)

    public PaidMediaVideo()
    {
        Type = "video";
    }
}

public class Sticker
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for this sticker file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for this sticker file

    [JsonProperty("type")]
    public string Type { get; set; } // Type of the sticker, e.g., "regular", "mask", "custom_emoji"

    [JsonProperty("width")]
    public long Width { get; set; } // Sticker width

    [JsonProperty("height")]
    public long Height { get; set; } // Sticker height

    [JsonProperty("is_animated")]
    public bool IsAnimated { get; set; } // True, if the sticker is animated

    [JsonProperty("is_video")]
    public bool IsVideo { get; set; } // True, if the sticker is a video sticker

    [JsonProperty("thumbnail")]
    public PhotoSize? Thumbnail { get; set; } // Optional: Sticker thumbnail, nullable

    [JsonProperty("emoji")]
    public string Emoji { get; set; } // Optional: Emoji associated with the sticker

    [JsonProperty("set_name")]
    public string SetName { get; set; } // Optional: Name of the sticker set, nullable

    [JsonProperty("premium_animation")]
    public File PremiumAnimation { get; set; } // Optional: Premium animation for regular stickers, nullable

    [JsonProperty("mask_position")]
    public MaskPosition MaskPosition { get; set; } // Optional: Position for mask stickers, nullable

    [JsonProperty("custom_emoji_id")]
    public string CustomEmojiId { get; set; } // Optional: Unique identifier for custom emoji stickers, nullable

    [JsonProperty("needs_repainting")]
    public bool? NeedsRepainting { get; set; } // Optional: True if the sticker must be repainted, nullable

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes, nullable
}

public class Story
{
    [JsonProperty("chat")]
    public Chat Chat { get; set; }  // The chat that posted the story.

    [JsonProperty("id")]
    public long Id { get; set; }  // Unique identifier for the story in the chat.
}

public class MaskPosition
{
    [JsonProperty("point")]
    public string Point { get; set; }  // The part of the face relative to which the mask should be placed. One of "forehead", "eyes", "mouth", or "chin".

    [JsonProperty("x_shift")]
    public float XShift { get; set; }  // Shift by X-axis measured in widths of the mask scaled to the face size.

    [JsonProperty("y_shift")]
    public float YShift { get; set; }  // Shift by Y-axis measured in heights of the mask scaled to the face size.

    [JsonProperty("scale")]
    public float Scale { get; set; }  // Mask scaling coefficient.
}
public class File
{
    [JsonProperty("file_id")]
    public string FileId { get; set; }  // Identifier for this file, which can be used to download or reuse the file
    
    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; }  // Unique identifier for this file, which is supposed to be the same over time and for different bots
    
    [JsonProperty("file_size")]
    public long? FileSize { get; set; }  // Optional. File size in bytes. Can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. Uses long? for nullable value
    
    [JsonProperty("file_path")]
    public string? FilePath { get; set; }  // Optional. File path. Can be null
}

public class Video
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for the video file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for the video file

    [JsonProperty("width")]
    public long Width { get; set; } // Video width as defined by the sender

    [JsonProperty("height")]
    public long Height { get; set; } // Video height as defined by the sender

    [JsonProperty("duration")]
    public long Duration { get; set; } // Duration of the video in seconds as defined by the sender

    [JsonProperty("thumbnail")]
    public PhotoSize? Thumbnail { get; set; } // Optional: Video thumbnail, nullable

    [JsonProperty("file_name")]
    public string? FileName { get; set; } // Optional: Original filename, nullable

    [JsonProperty("mime_type")]
    public string? MimeType { get; set; } // Optional: MIME type of the file, nullable

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes, nullable
}

public class VideoNote
{
    [JsonProperty("file_id")]
    public string FileId { get; set; }  // Identifier for this file, which can be used to download or reuse the file.

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; }  // Unique identifier for this file, which is supposed to be the same over time and for different bots.

    [JsonProperty("length")]
    public int Length { get; set; }  // Video width and height (diameter of the video message) as defined by the sender.

    [JsonProperty("duration")]
    public int Duration { get; set; }  // Duration of the video in seconds as defined by the sender.

    [JsonProperty("thumbnail")]
    public PhotoSize? Thumbnail { get; set; }  // Optional. Video thumbnail.

    [JsonProperty("file_size")]
    public long? FileSize { get; set; }  // Optional. File size in bytes.
}

public class Voice
{
    [JsonProperty("file_id")]
    public string FileId { get; set; }  // Identifier for this file, which can be used to download or reuse the file.

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; }  // Unique identifier for this file, which is supposed to be the same over time and for different bots.

    [JsonProperty("duration")]
    public int Duration { get; set; }  // Duration of the audio in seconds as defined by the sender.

    [JsonProperty("mime_type")]
    public string? MimeType { get; set; }  // Optional. MIME type of the file as defined by the sender.

    [JsonProperty("file_size")]
    public long? FileSize { get; set; }  // Optional. File size in bytes.
}

public class Contact
{
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }  // Contact's phone number.

    [JsonProperty("first_name")]
    public string FirstName { get; set; }  // Contact's first name.

    [JsonProperty("last_name")]
    public string? LastName { get; set; }  // Optional. Contact's last name.

    [JsonProperty("user_id")]
    public long? UserId { get; set; }  // Optional. Contact's user identifier in Telegram. Can be larger than 32 bits, use a 64-bit integer.

    [JsonProperty("vcard")]
    public string? VCard { get; set; }  // Optional. Additional data about the contact in the form of a vCard.
}

public class Dice
{
    [JsonProperty("emoji")]
    public string Emoji { get; set; }  // Emoji on which the dice throw animation is based.

    [JsonProperty("value")]
    public int Value { get; set; }  // Value of the dice (1-6 for specific emojis, 1-64 for others).
}
public class Game
{
    [JsonProperty("title")]
    public string Title { get; set; }  // Title of the game

    [JsonProperty("description")]
    public string Description { get; set; }  // Description of the game

    [JsonProperty("photo")]
    public List<PhotoSize> Photo { get; set; }  // Photos displayed in the game message

    [JsonProperty("text")]
    public string? Text { get; set; }  // Optional description or high scores in the game message

    [JsonProperty("text_entities")]
    public List<MessageEntity>? TextEntities { get; set; }  // Optional special entities in text

    [JsonProperty("animation")]
    public Animation? Animation { get; set; }  // Optional animation for the game message
}

public class Giveaway
{
    [JsonProperty("chats")]
    public List<Chat> Chats { get; set; }  // List of chats that the user must join to participate in the giveaway

    [JsonProperty("winners_selection_date")]
    public long WinnersSelectionDate { get; set; }  // Unix timestamp of when winners will be selected

    [JsonProperty("winner_count")]
    public int WinnerCount { get; set; }  // Number of winners to be selected

    [JsonProperty("only_new_members")]
    public bool? OnlyNewMembers { get; set; }  // Optional. True, if only users who join the chats after the giveaway started should be eligible

    [JsonProperty("has_public_winners")]
    public bool? HasPublicWinners { get; set; }  // Optional. True, if the winners will be publicly visible

    [JsonProperty("prize_description")]
    public string? PrizeDescription { get; set; }  // Optional. Description of additional giveaway prize

    [JsonProperty("country_codes")]
    public List<string>? CountryCodes { get; set; }  // Optional. List of two-letter country codes indicating eligible countries

    [JsonProperty("prize_star_count")]
    public int? PrizeStarCount { get; set; }  // Optional. Number of Telegram Stars for Telegram Star giveaways

    [JsonProperty("premium_subscription_month_count")]
    public int? PremiumSubscriptionMonthCount { get; set; }  // Optional. Number of months of Telegram Premium subscription for Telegram Premium giveaways
}

public class GiveawayWinners
{
    [JsonProperty("chat")]
    public Chat Chat { get; set; }  // The chat that created the giveaway

    [JsonProperty("giveaway_message_id")]
    public int GiveawayMessageId { get; set; }  // Identifier of the message with the giveaway

    [JsonProperty("winners_selection_date")]
    public long WinnersSelectionDate { get; set; }  // Unix timestamp when winners were selected

    [JsonProperty("winner_count")]
    public int WinnerCount { get; set; }  // Total number of winners in the giveaway

    [JsonProperty("winners")]
    public List<User> Winners { get; set; }  // List of up to 100 winners

    [JsonProperty("additional_chat_count")]
    public int? AdditionalChatCount { get; set; }  // Optional. The number of other chats the user had to join to be eligible

    [JsonProperty("prize_star_count")]
    public int? PrizeStarCount { get; set; }  // Optional. Number of Telegram Stars split among winners (for Telegram Star giveaways)

    [JsonProperty("premium_subscription_month_count")]
    public int? PremiumSubscriptionMonthCount { get; set; }  // Optional. Number of months of Telegram Premium subscription won from the giveaway

    [JsonProperty("unclaimed_prize_count")]
    public int? UnclaimedPrizeCount { get; set; }  // Optional. Number of undistributed prizes

    [JsonProperty("only_new_members")]
    public bool? OnlyNewMembers { get; set; }  // Optional. True, if only users who joined the chats after the giveaway started were eligible

    [JsonProperty("was_refunded")]
    public bool? WasRefunded { get; set; }  // Optional. True, if the giveaway was canceled because the payment was refunded

    [JsonProperty("prize_description")]
    public string? PrizeDescription { get; set; }  // Optional. Description of the additional giveaway prize
}

public class Invoice
{
    [JsonProperty("title")]
    public string Title { get; set; }  // Product name

    [JsonProperty("description")]
    public string Description { get; set; }  // Product description

    [JsonProperty("start_parameter")]
    public string StartParameter { get; set; }  // Unique bot deep-linking parameter

    [JsonProperty("currency")]
    public string Currency { get; set; }  // Three-letter ISO 4217 currency code, or "XTR" for Telegram Stars

    [JsonProperty("total_amount")]
    public int TotalAmount { get; set; }  // Total price in the smallest units of the currency
}
public class Venue
{
    [JsonProperty("location")]
    public Location Location { get; set; }  // Venue location. Can't be a live location

    [JsonProperty("title")]
    public string Title { get; set; }  // Name of the venue

    [JsonProperty("address")]
    public string Address { get; set; }  // Address of the venue

    [JsonProperty("foursquare_id")]
    public string? FoursquareId { get; set; }  // Optional. Foursquare identifier of the venue

    [JsonProperty("foursquare_type")]
    public string? FoursquareType { get; set; }  // Optional. Foursquare type of the venue

    [JsonProperty("google_place_id")]
    public string? GooglePlaceId { get; set; }  // Optional. Google Places identifier of the venue

    [JsonProperty("google_place_type")]
    public string? GooglePlaceType { get; set; }  // Optional. Google Places type of the venue
}
public class PhotoSize
{
    [JsonProperty("file_id")]
    public string FileId { get; set; } // Identifier for this file, used to download or reuse the file

    [JsonProperty("file_unique_id")]
    public string FileUniqueId { get; set; } // Unique identifier for this file, constant across time and different bots

    [JsonProperty("width")]
    public long Width { get; set; } // Photo width

    [JsonProperty("height")]
    public long Height { get; set; } // Photo height

    [JsonProperty("file_size")]
    public long? FileSize { get; set; } // Optional: File size in bytes
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


public class MessageOrigin
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("date")]
    public int Date { get; set; }
}

public class MessageOriginUser : MessageOrigin
{
    [JsonProperty("sender_user")]
    public User SenderUser { get; set; }
}

public class MessageOriginHiddenUser : MessageOrigin
{
    [JsonProperty("sender_user_name")]
    public string SenderUserName { get; set; }
}

public class MessageOriginChat : MessageOrigin
{
    [JsonProperty("sender_chat")]
    public Chat SenderChat { get; set; }

    [JsonProperty("author_signature")]
    public string AuthorSignature { get; set; }
}

public class MessageOriginChannel : MessageOrigin
{
    [JsonProperty("chat")]
    public Chat Chat { get; set; }

    [JsonProperty("message_id")]
    public int MessageId { get; set; }

    [JsonProperty("author_signature")]
    public string AuthorSignature { get; set; }
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

