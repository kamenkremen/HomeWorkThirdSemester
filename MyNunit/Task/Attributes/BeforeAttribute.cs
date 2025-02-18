namespace MyNunit;

/// <summary>
/// Attribute, that marks method, that is going to be used before each test.
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