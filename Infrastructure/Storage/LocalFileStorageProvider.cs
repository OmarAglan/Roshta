using Rosheta.Core.Application.Contracts.Infrastructure;

namespace Rosheta.Infrastructure.Storage
{
    /// <summary>
    /// Local file system implementation of IFileStorageProvider for desktop applications
    /// </summary>
    public class LocalFileStorageProvider : IFileStorageProvider
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteAllTextAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }

        public string GetApplicationDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public void EnsureDirectoryExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
