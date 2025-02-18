namespace MyNunit;

/// <summary>
/// Attribute, that marks method as test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class Test : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Test"/> class.
    /// </summary>
    public Test()
    {
    }

    /// <summary>
    /// Gets or sets type of exception this method should throw.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets reason for ignoring this test.
    /// </summary>
    public string? Ignore { get; set; }
}