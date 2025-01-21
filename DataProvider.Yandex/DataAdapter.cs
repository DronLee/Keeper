using System;
using System.IO;
using System.Threading.Tasks;
using DataProvider.Abstractions;
using DataProvider.Yandex.Responses;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Linq;
using Data.Abstractions.Exceptions;

namespace DataProvider.Yandex
{
    /// <summary>
    /// Data adapter of Yandex disk
    /// </summary>
    public class DataAdapter : IDataAdapter
    {
        private readonly string _dataFileName = "data";

        private string _accessToken;
        private bool _offline = false;

        /// <inheritdoc/>
        public event Action ChangeConnectEvent;

        public DataAdapter(string dataFileName)
        {
            _dataFileName = dataFileName;
        }

        /// <inheritdoc/>
        public string DataFile => Path.Combine(Environment.CurrentDirectory, _dataFileName);

        /// <inheritdoc/>
        public bool Offline
        {
            get
            {
                return _offline;
            }
            private set
            {
                _offline = value;
                ChangeConnectEvent?.Invoke();
            }
        }

        /// <inheritdoc/>
        public async Task AuthAsync()
        {
            var authenticator = new DeviceCodeAuthenticator();
            var tokenData = await authenticator.Authenticate();
            _accessToken = tokenData.access_token;
        }

        /// <inheritdoc/>
        public async Task< byte[]> DownloadDataAsync()
        {
            string downloadUrl = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://cloud-api.yandex.net");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", _accessToken);

                string responseJson = null;
                try
                {
                    responseJson = await client.GetStringAsync($"v1/disk/resources/download?path={_dataFileName}");
                }
                catch(HttpRequestException exc)
                {
                    if(exc.Message.ToUpper().Contains("NOT FOUND"))
                    {
                        throw new ServerDataFileNotFoundException(_dataFileName, $"File \"{_dataFileName}\" is not found on yandex disk");
                    }
                    throw;
                }

                var downloadFileResponse = JsonConvert.DeserializeObject<GetResourcesResponse>(responseJson);
                downloadUrl = downloadFileResponse.href;
            }

            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(downloadUrl);
            }
        }

        /// <inheritdoc/>
        public async Task SaveDataAsync(byte[] data)
        {
            string uploadUrl = null;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://cloud-api.yandex.net/");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", _accessToken);

                var responseJson = await client.GetStringAsync($"v1/disk/resources/upload?path={_dataFileName}&overwrite=true");
                var uploadFileResponse = JsonConvert.DeserializeObject<GetResourcesResponse>(responseJson);
                uploadUrl = uploadFileResponse.href;
            }

            var acceptedStatusCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted }; 

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(uploadUrl);

                using (HttpResponseMessage response = await client.PutAsync(string.Empty, new StreamContent(new MemoryStream(data))))
                {
                    if(!acceptedStatusCodes.Contains(response.StatusCode))
                    {
                        throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
                    }
                }
            }
        }
    }
}