namespace MyNunit;

/// <summary>
/// Attribute, that marks method, that is going to be used after all of the tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AfterClass : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AfterClass"/> class.
    /// </summary>
    public AfterClass()
    {
    }
}