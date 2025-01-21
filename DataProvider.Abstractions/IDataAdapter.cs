using System;
using System.Threading.Tasks;

namespace DataProvider.Abstractions
{
    /// <summary>
    /// Interface of a data adapter
    /// </summary>
    public interface IDataAdapter
    {
        /// <summary>
        /// Event of connection state changing
        /// </summary>
        event Action ChangeConnectEvent;

        /// <summary>
        /// Path to file with data
        /// </summary>
        string DataFile { get; }

        /// <summary>
        /// True - connection with store is failed
        /// </summary>
        bool Offline { get; }

        /// <summary>
        /// Authenticate
        /// </summary>
        Task AuthAsync();

        /// <summary>
        /// Download data from store
        /// </summary>
        Task<byte[]> DownloadDataAsync();

        /// <summary>
        /// Save data to store
        /// </summary>
        Task SaveDataAsync(byte[] data);
    }
}