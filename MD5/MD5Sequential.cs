namespace Test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class that computes check sum of the directory in single thread.
/// </summary>
public static class MD5Sequential
{
    /// <summary>
    /// Computes check sum for directory or file by the path.
    /// </summary>
    /// <param name="path">Path to the directory or file.</param>
    /// <returns>Task, that computes the check sum for directory or file.</returns>
    public static byte[] ComputeSum(string path)
    {
        if (Path.HasExtension(path))
        {
            return MD5Sequential.ComputeFile(path);
        }

        return MD5Sequential.ComputeDirectory(path);
    }

    private static byte[] ComputeDirectory(string path)
    {
        var files = Directory.GetFiles(path);
        var direcoties = Directory.GetDirectories(path);
        Array.Sort(files);
        Array.Sort(direcoties);
        List<byte[]> hashes = new ();
        int size = 0;
        foreach (var dirPath in direcoties)
        {
            var bytes = ComputeDirectory(dirPath);
            size += bytes.Length;
            hashes.Add(bytes);
        }

        foreach (var filePath in files)
        {
            var bytes = ComputeFile(filePath);
            size += bytes.Length;
            hashes.Add(bytes);
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

    private static byte[] ComputeFile(string path)
    {
        var hash = MD5.Create();
        return hash.ComputeHash(File.OpenRead(path));
    }
}