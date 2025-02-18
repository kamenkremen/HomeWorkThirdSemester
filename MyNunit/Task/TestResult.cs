// Copyright 2024 Ivan Zakarlyuka.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace MyNunit;

/// <summary>
/// Structure for information about test result.
/// </summary>
public struct TestResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> struct.
    /// </summary>
    /// <param name="testName">Name of the test.</param>
    public TestResult(string testName)
    {
        this.TestName = testName;
    }

    /// <summary>
    /// Gets or sets name of the test.
    /// </summary>
    public string TestName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether test was succesfull or not.
    /// </summary>
    public bool Ok { get; set; }

    /// <summary>
    /// Gets or sets reason for ignoring this test.
    /// </summary>
    public string? WhyIgnored { get; set; }

    /// <summary>
    /// Gets or sets exception message for this test.
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// Gets or sets ellapsed time for this test.
    /// </summary>
    public long EllapsedTime { get; set; }
}
