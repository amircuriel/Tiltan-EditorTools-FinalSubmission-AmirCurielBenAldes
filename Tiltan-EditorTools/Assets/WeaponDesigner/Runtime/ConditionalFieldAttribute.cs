using UnityEngine;

/// Lets a field show only when another bool says it should.
public class ConditionalFieldAttribute : PropertyAttribute
{
    public readonly string conditionFieldName;
    public readonly bool expectedBool;
    public readonly bool inverse;

    public ConditionalFieldAttribute(string conditionFieldName, bool expectedBool = true, bool inverse = false)
    {
        this.conditionFieldName = conditionFieldName;
        this.expectedBool = expectedBool;
        this.inverse = inverse;
    }
}
