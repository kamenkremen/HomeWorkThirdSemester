using System.Diagnostics;
using System.Text;
using Test1;

Console.WriteLine("Enter path to the directory or file, or type Benchmark to start a benchmark");

string? path = Console.ReadLine();
if (path == "Benchmark")
{
    Console.WriteLine("Benchmarking...");
    path = "testFolder/testBecnhmark";
}

try
{
    var watch = Stopwatch.GetTimestamp();
    var sequentialResult = MD5Sequential.ComputeSum(path);
    var sequentialTime = Stopwatch.GetElapsedTime(watch);
    watch = Stopwatch.GetTimestamp();
    var parallelResult = MD5Async.ComputeSum(path).Result;
    var parallelTime = Stopwatch.GetElapsedTime(watch);
    Console.WriteLine(
    "Check sum = (parallel calculations){0} = (sequential calculations){1}, time for sequential MD5 = {2}, time for parallel MD5 = {3}.",
    Encoding.UTF8.GetString(parallelResult),
    Encoding.UTF8.GetString(sequentialResult),
    sequentialTime,
    parallelTime);
}
catch (Exception e)
{
    Console.WriteLine("Error occured, probably incorrect path. Error = {0}", e);
}
