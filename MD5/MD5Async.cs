namespace Test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class that computes check sum of the directory in multiple threads.
/// </summary>
public static class MD5Async
{
    /// <summary>
    /// Computes check sum for directory or file by the path.
    /// </summary>
    /// <param name="path">Path to the directory or file.</param>
    /// <returns>Task, that computes the check sum for directory or file.</returns>
    public static async Task<byte[]> ComputeSum(string path)
    {
        if (Path.HasExtension(path))
        {
            return await MD5Async.ComputeFile(path);
        }

        return await MD5Async.ComputeDirectory(path);
    }

    private static async Task<byte[]> ComputeDirectory(string path)
    {
        var files = Directory.GetFiles(path);
        var direcoties = Directory.GetDirectories(path);
        Array.Sort(files);
        Array.Sort(direcoties);
        List<Task<byte[]>> computingHashes = new ();
        foreach (var dirPath in direcoties)
        {
            var bytes = ComputeDirectory(dirPath);
            computingHashes.Add(bytes);
        }

        foreach (var filePath in files)
        {
            var bytes = ComputeFile(filePath);
            computingHashes.Add(bytes);
        }

        int size = 0;

        List<byte[]> hashes = new ();
        foreach (var computingHash in computingHashes)
        {
            var hash = await computingHash;
            size += hash.Length;
            hashes.Add(hash);
        }

        var fileInfo = new FileInfo(path);
        var MD5Hash = MD5.Create();
        var computedHash = MD5Hash.ComputeHash(Encoding.ASCII.GetBytes(fileInfo.Name));
        size += computedHash.Length;

        var toHash = new byte[size];

        Buffer.BlockCopy(computedHash, 0, toHash, 0, computedHash.Length);
        int offset = computedHash.Length;
        foreach (var hash in hashes)
        {
            Buffer.BlockCopy(hash, 0, toHash, offset, hash.Length);
            offset += hash.Length;
        }

        return MD5Hash.ComputeHash(toHash);
    }

    private static async Task<byte[]> ComputeFile(string path)
    {
        var hash = MD5.Create();
        return await hash.ComputeHashAsync(File.OpenRead(path));
    }
}
