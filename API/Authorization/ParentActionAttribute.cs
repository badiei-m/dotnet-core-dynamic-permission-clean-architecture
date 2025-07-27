namespace API.Authorization;

[AttributeUsage(AttributeTargets.Method)]
public class ParentActionAttribute : Attribute
{
    public string ParentController { get; }
    public string ParentAction { get; }

    public ParentActionAttribute(string parentController, string parentAction)
    {
        ParentController = parentController?.Replace("Controller", "", StringComparison.OrdinalIgnoreCase).ToLower();
        ParentAction = parentAction?.ToLower();
    }
}