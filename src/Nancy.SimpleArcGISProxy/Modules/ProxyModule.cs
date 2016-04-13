namespace Nancy.SimpleArcGISProxy.Modules
{
    using Nancy;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    public class ProxyModule : NancyModule
    {
        const string DefaultImageResponseContentType = "image/jpeg";
        const string JsonFormat = "json";

        public ProxyModule()
        {
            var httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (httpClientHandler.SupportsUseProxy()) httpClientHandler.UseProxy = true;
            if (httpClientHandler.SupportsAllowAutoRedirect()) httpClientHandler.AllowAutoRedirect = true;
            if (httpClientHandler.SupportsPreAuthenticate()) httpClientHandler.PreAuthenticate = true;

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonp"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));

            Get["/proxy", true] = async (parameters, ctx) =>
            {
                var parsedUrl = ParseQueryString(Request.Query);
                var requestForType = parsedUrl.Item1;
                var url = parsedUrl.Item2;

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (string.Equals(JsonFormat, requestForType, StringComparison.OrdinalIgnoreCase))
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return Response.FromStream(responseStream, DefaultImageResponseContentType);
            };

            Post["/proxy", true] = async (parameters, ctx) =>
            {
                var parsedUrl = ParseQueryString(Request.Query);
                var requestForType = parsedUrl.Item1;
                var url = parsedUrl.Item2.Split('?').FirstOrDefault();

                var parametersToSend = parsedUrl.Item3;
                HttpContent content = null;
                try
                {
                    content = new FormUrlEncodedContent(parametersToSend);
                }
                catch (FormatException fex)
                {
                    var tempContent = new MultipartFormDataContent();
                    foreach (var keyValuePair in parametersToSend)
                    {
                        tempContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }
                    content = tempContent;
                }

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                if (string.Equals(JsonFormat, requestForType, StringComparison.OrdinalIgnoreCase))
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return Response.FromStream(responseStream, DefaultImageResponseContentType);
            };
        }

        Tuple<string, string, Dictionary<string, string>> ParseQueryString(DynamicDictionary requestUrl)
        {
            var url = new StringBuilder();
            string requestForType = "";
            int skip = 0;

            var parts = AsDictionary(requestUrl);

            foreach (var part in parts)
            {
                var key = part.Key;
                var value = part.Value;

                if (skip > 0)
                {
                    url.Append("&");
                }

                if (string.Equals(key, value, StringComparison.OrdinalIgnoreCase))
                {
                    url.Append(key);
                }
                else
                {
                    url.AppendFormat("{0}={1}", key, value);
                }

                if (key == "f" || key.EndsWith("?f", StringComparison.OrdinalIgnoreCase))
                {
                    requestForType = value;
                }

                skip++;
            }

            return Tuple.Create(requestForType, url.ToString(), parts);
        }

        static Dictionary<string, string> AsDictionary(DynamicDictionary dictionary)
        {
            return dictionary.GetDynamicMemberNames().ToDictionary(
                    memberName => memberName,
                    memberName => (string)dictionary[memberName]);
        }       
    }
}
