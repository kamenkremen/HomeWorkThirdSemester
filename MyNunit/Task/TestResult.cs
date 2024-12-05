namespace MyNunit;

public struct TestResult 
{
    public string TestName { get; set; }
    public bool Ok { get; set; }
  //  public bool Ignored { get; set; }
    public string? WhyIgnored { get; set; }
   // public bool IsExceptionThrowed { get; set; }
    public string? ExceptionMessage { get; set; }
    public long EllapsedTime { get; set; }

    public TestResult(string testName)
    {
        this.TestName = testName;
    }

}