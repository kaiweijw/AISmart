namespace AISmart.GAgent.Autogen.Common;

public class AgentDescription
{
    public string AgentName { get; set; }
    public string AgentDiscription { get; set; }
    public Type AgentEventType { get; set; }
    
    public List<AgentEventTypeFieldDescription> EventParameters { get; set; }
}

public class AgentEventTypeFieldDescription
{
    public string FieldName { get; set; }
    public string FieldDescription { get; set; }
    public Type FieldType { get; set; }
}