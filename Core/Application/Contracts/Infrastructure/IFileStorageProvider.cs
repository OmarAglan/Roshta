namespace Rosheta.Core.Application.Contracts.Infrastructure
{
    /// <summary>
    /// Abstraction for file storage operations to enable platform-agnostic file access
    /// </summary>
    public interface IFileStorageProvider
    {
        /// <summary>
        /// Checks if a file exists at the specified path
        /// </summary>
        bool FileExists(string path);

        /// <summary>
        /// Reads all text from a file
        /// </summary>
        Task<string> ReadAllTextAsync(string path);

        /// <summary>
        /// Writes text to a file, creating directories if needed
        /// </summary>
        Task WriteAllTextAsync(string path, string content);

        /// <summary>
        /// Gets the application data directory path for the current platform
        /// </summary>
        string GetApplicationDataPath();

        /// <summary>
        /// Combines path segments into a single path
        /// </summary>
        string CombinePath(params string[] paths);

        /// <summary>
        /// Ensures a directory exists, creating it if necessary
        /// </summary>
        void EnsureDirectoryExists(string path);
    }
}
