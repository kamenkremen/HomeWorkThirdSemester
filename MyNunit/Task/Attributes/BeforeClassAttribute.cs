namespace MyNunit;

/// <summary>
/// Attribute, that marks method, that is going to be used before all of the tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BeforeClass : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeClass"/> class.
    /// </summary>
    public BeforeClass()
    {
    }
}