namespace MyNunit;

public class Test : Attribute
{
    public Test()
    {
    }

    public Type? Expected { get; set; }

    public string? Ignore { get; set; }
}