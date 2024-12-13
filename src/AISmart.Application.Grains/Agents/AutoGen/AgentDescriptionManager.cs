using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Volo.Abp.DependencyInjection;

namespace AISmart.Application.Grains.Agents.AutoGen;

public class AgentDescriptionManager : ISingletonDependency
{
    private readonly Dictionary<string, AgentDescription> _agentDescription =
        new Dictionary<string, AgentDescription>();

    private readonly string _autoGenAgentEventDescription = string.Empty;

    public AgentDescriptionManager()
    {
        _agentDescription = GenerateAgentDescription();
        _autoGenAgentEventDescription = AssembleAgentEventDescription();
    }

    public ReadOnlyDictionary<string, AgentDescription> GetAgentDescription()
    {
        return new ReadOnlyDictionary<string, AgentDescription>(_agentDescription);
    }

    public string GetAutoGenEventDescription()
    {
        return _autoGenAgentEventDescription;
    }

    private Dictionary<string, AgentDescription> GenerateAgentDescription()
    {
        var agentList = Assembly.GetAssembly(typeof(GAgent<,>));
        var result = new Dictionary<string, AgentDescription>();

        foreach (var classType in agentList.GetTypes())
        {
            if (!classType.IsClass && !classType.IsSealed)
            {
                continue;
            }

            var agentDescription = new AgentDescription();
            var description = classType.GetCustomAttribute<DescriptionAttribute>();
            if (description == null)
            {
                throw new Exception($"class:{classType.Name} does not exist description attribute");
            }

            agentDescription.AgentName = classType.Name;
            agentDescription.AgentDiscription = description.Description;

            var agentGenericList = classType.GetGenericArguments();
            if (agentGenericList.Length == 2)
            {
                throw new Exception($"ClassName:{classType.Name} generic error");
            }

            var eventType = agentGenericList[1];

            // get all field name
            FieldInfo[] fields = eventType.GetFields(BindingFlags.Public);
            var fieldList = new List<AgentEventTypeFieldDescription>();
            foreach (var fieldType in fields)
            {
                var descriptionAttribute = fieldType.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute == null)
                {
                    throw new Exception($"ClassName:{eventType.Name} field:{fieldType.Name} description not exsit");
                }

                var fieldDescription = new AgentEventTypeFieldDescription();
                fieldDescription.FieldName = fieldType.Name;
                fieldDescription.FieldDescription = descriptionAttribute.Description;
                fieldDescription.FieldType = fieldType.FieldType;
                fieldList.Add(fieldDescription);
            }

            agentDescription.EventParameters = fieldList;

            result.Add(agentDescription.AgentName, agentDescription);
        }

        return result;
    }

    private string AssembleAgentEventDescription()
    {
        var result = new StringBuilder();

        foreach (var (key, value) in _agentDescription)
        {
            StringBuilder agentDescription = new StringBuilder();
            agentDescription.AppendLine("{");
            agentDescription.AppendLine($"  EventName:\"{value.AgentName}\",");
            agentDescription.AppendLine($"  EventDescription:\"{value.AgentDiscription}\",");
            agentDescription.AppendLine($"  EventParameters:[");
            agentDescription.AppendLine($"{AssembleAgentEventParameterDescription(value.EventParameters)}");
            agentDescription.AppendLine($"  ]");
            agentDescription.AppendLine("}");
            result.Append(agentDescription.ToString());
        }

        return result.ToString();
    }

    private string AssembleAgentEventParameterDescription(List<AgentEventTypeFieldDescription> fieldDescriptionList)
    {
        var result = new StringBuilder();
        var firstFlag = fieldDescriptionList.Count > 1;
        foreach (var item in fieldDescriptionList)
        {
            var fieldDescription = new StringBuilder();
            fieldDescription.AppendLine("   {");
            fieldDescription.AppendLine($"      ParameterType:\"{nameof(item.FieldType)}\",");
            fieldDescription.AppendLine($"      ParameterName:\"{item.FieldName}\",");
            fieldDescription.AppendLine($"      ParameterDescription:\"{item.FieldDescription}\",");
            fieldDescription.AppendLine($"      ParameterIsRequired:true");
            if (firstFlag)
            {
                fieldDescription.AppendLine("   },");
                firstFlag = false;
            }
            else
            {
                fieldDescription.AppendLine("   }");
            }

            result.AppendLine(fieldDescription.ToString());
        }

        return result.ToString();
    }
}