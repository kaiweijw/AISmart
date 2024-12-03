namespace AISmart.Agents;

public class RoleDialogModel
{
    /// <summary>
    /// If Role is Assistant, it is same as user's message id.
    /// </summary>
    public string MessageId { get; set; }
    
    /// <summary>
    /// user, system, assistant, function
    /// </summary>
    public string Role { get; set; }
    
    /// <summary>
    /// User id when Role is User
    /// </summary>
    public string SenderId { get; set; }
    
    public string Content { get; set; }
}