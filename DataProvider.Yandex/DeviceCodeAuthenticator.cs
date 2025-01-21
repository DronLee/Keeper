using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DataProvider.Yandex.Responses;
using DataProvider.Yandex.Views;
using DataProvider.Yandex.ViewModels;

namespace DataProvider.Yandex
{
    internal class DeviceCodeAuthenticator
    {
        private const string _clientId = "275027ec963844bd85b2140bee4b425d";
        private const string _clientSecret = "032bd4ff03eb49efbe3d348dc67457d6";
        private const string _oAuthYandexUrl = "https://oauth.yandex.ru";

        private bool _showCode = false;

        public async Task<TokenResponse> Authenticate()
        {
            var deviceCodeResponse = await GetDeviceCodeResponseAsync();
            return await GetToken(deviceCodeResponse);
        }

        private async Task<DeviceCodeResponse> GetDeviceCodeResponseAsync()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(_oAuthYandexUrl);
                var parameters = new Dictionary<string, string> { { "client_id", _clientId } };
                using (var encodedContent = new FormUrlEncodedContent(parameters))
                {
                    using (HttpResponseMessage response = client.PostAsync("/device/code", encodedContent).Result)
                    using (HttpContent content = response.Content)
                    {
                        var responseJson = await content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<DeviceCodeResponse>(responseJson);
                        return result;
                    }
                }
            }
        }

        private async Task<TokenResponse> GetToken(DeviceCodeResponse deviceCodeResponse)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_oAuthYandexUrl);

                var dict = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "grant_type", "device_code" },
                    { "code", deviceCodeResponse.device_code },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret }
                };

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));

                TokenResponse result = null;

                while (result == null)
                {
                    using (var formUrlEncodedContent = new FormUrlEncodedContent(dict))
                    using (var tokenResponse = await client.PostAsync("token", formUrlEncodedContent))
                    {
                        var responseJson = await tokenResponse.Content.ReadAsStringAsync();
                        if (tokenResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            result = JsonConvert.DeserializeObject<TokenResponse>(responseJson);
                        }
                        else
                        {
                            if (!_showCode)
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = deviceCodeResponse.verification_url,
                                    UseShellExecute = true
                                });

                                _showCode = true;
                                new UserCodeView(new UserCodeViewModel { UserCode = deviceCodeResponse.user_code }).ShowDialog();
                            }
                            await Task.Delay(1000);
                        }
                    }
                }

                return result;
            }
        }
    }
}
