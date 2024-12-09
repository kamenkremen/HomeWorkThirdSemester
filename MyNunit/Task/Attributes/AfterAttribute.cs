namespace MyNunit;

/// <summary>
/// Attribute, that marks method, that is going to be used after each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class After : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="After"/> class.
    /// </summary>
    public After()
    {
    }
}