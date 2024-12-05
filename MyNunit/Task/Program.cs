if (args.Length == 0)
{
    Console.WriteLine("Please, insert path to directory with tests.");
    return;
}

var path = args[0];
if (!Directory.Exists(path))
{
    Console.WriteLine("Invalid path!");
    return;
}

MyNunit.TestRunner.RunFromPath(path);