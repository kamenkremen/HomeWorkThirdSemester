namespace MyNunit;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class Before : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Before"/> class.
    /// </summary>
    public Before()
    {
    }
}