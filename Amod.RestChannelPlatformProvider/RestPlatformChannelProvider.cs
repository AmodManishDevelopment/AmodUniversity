using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using RestSharp.Deserializers;

namespace Amod.RestChannelPlatformProvider
{
    public sealed class RestPlatformChannelProvider : RestClient
    {

        #region Members
        RestRequest _currentRequest;
        private Uri _requestUri;
        private bool _bUseAuthenticator = true;
        #endregion

        #region Properties
        public RestRequest CurrentRestRequest { get; set; }
        public string ContentType { get; set; }
        #endregion

        #region Ctor

        static RestPlatformChannelProvider()
        {
        }

        public RestPlatformChannelProvider()
            : base()
        {

        }

        public RestPlatformChannelProvider(IAuthenticator authenticator)
            : base()
        {
            this.Authenticator = authenticator;
        }

        public RestPlatformChannelProvider(bool useAuthenticator)
            : base()
        {
            if (useAuthenticator == false)
                this.Authenticator = null;
            _bUseAuthenticator = useAuthenticator;
        }


        #endregion

        #region Public Methods

        #region Get Methods, Async and Typed

        /// <summary>
        /// Get request based on identfier
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Get(string urlIdentifier, params string[] urlReplacement)
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, urlReplacement);
            var response = client.Execute(_currentRequest) as RestResponse;
            return response;
        }

        /// <summary>
        /// Get Async Request
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public void GetAsync(string urlIdentifier, Action<string> callback, params string[] urlReplacement)
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, urlReplacement);
            client.ExecuteAsync(_currentRequest, (response) => callback(response.Content));
        }

        /// <summary>
        /// Get Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Get<T>(string urlIdentifier, params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, urlReplacement);
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Get<T>(string urlIdentifier, List<Parameter> parameters,
params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            return response;
        }

        /// <summary>
        /// Get Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> GetAsJsonString<T>(string urlIdentifier, params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, urlReplacement);
            var results = client.Execute<T>(_currentRequest) as RestResponse<T>;
            if (null != results && !string.IsNullOrEmpty(results.ContentType) && results.ContentType.Contains("application/json"))
                try
                {
                    results.Data = JsonConvert.DeserializeObject<T>(results.Content);
                }
                catch (Exception exception)
                {
                }

            return results;
        }

        /// <summary>
        /// Get async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public void GetAsync<T>(string urlIdentifier, Action<T> callback, params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.GET, urlIdentifier, urlReplacement);
            client.ExecuteAsync<T>(_currentRequest, (response) =>
            {
                callback(response.Data);
            });
        }


        #endregion

        #region Post Methods, Async and Typed

        /// <summary>
        /// Post Request based in identifier and gets T in the response
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Post(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement)
        {
            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.POST, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute(_currentRequest) as RestResponse;
            return response;
        }

        /// <summary>
        /// Post Request based on json data and  response
        /// </summary>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Post(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement)
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.Execute(_currentRequest) as RestResponse;

            return response;
        }

        /// <summary>
        /// Post Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Post<T>(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {

            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.POST, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && null != response.Content && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }

            }
            return response;
        }

        /// <summary>
        /// Overload with JSON Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Post<T>(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");
            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }

            return response;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Post<T>(DataFormat format, string urlIdentifier, object obj, List<Parameter> parameters, params string[] urlReplacement)
            where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");
            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, obj, parameters, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, obj, parameters, urlReplacement);
                    break;
            }

            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }

            return response;

        }

        /// <summary>
        /// Exceutes Async and returns the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        public void PostAsync<T>(DataFormat format, string urlIdentifier, object obj, Action<T> callback, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
            }
            client.ExecuteAsync<T>(_currentRequest, (response) =>
            {
                callback(response.Data);
            });
        }

        /// <summary>
        /// Exceutes Async and returns the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        public void PostAsync(DataFormat format, string urlIdentifier, object obj, Action<string> callback, params string[] urlReplacement)
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, obj, urlReplacement);
                    break;
            }
            client.ExecuteAsync(_currentRequest, (response) =>
            {
                callback(response.Content);
            });
        }


        public RestResponse<T> PostDataAsJsonString<T>(DataFormat format, string urlIdentifier, string data, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, data, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, data, urlReplacement);
                    break;
            }
            
            var results = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != results && null != results.Content && null == results.Data && results.ContentType.Contains("application/json"))
            {
                try
                {
                    results.Data = JsonConvert.DeserializeObject<T>(results.Content);
                }
                catch (Exception exception)
                {
                }

            }
            else
            {
            }

            return results; ;
        }

        public RestResponse<T> PostLegacyDataAsJsonString<T>(DataFormat format, string urlIdentifier, string data, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.POST, urlIdentifier, data, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.POST, urlIdentifier, data, urlReplacement);
                    break;
            }
            
            var results = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != results && null != results.Content && results.ContentType.Contains("application/json"))
            {
                try
                {
                    results.Data = results.Content.FromJSON<T>();
                }
                catch (Exception exception)
                {
                }

            }
            else
            {
            }


            return results; ;
        }

        #endregion

        #region Patch Methods, Async and Typed

        /// <summary>
        /// Post Request based in identifier and gets T in the response
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Patch(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement)
        {
            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.PATCH, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute(_currentRequest) as RestResponse;
            return response;
        }

        /// <summary>
        /// Post Request based on json data and  response
        /// </summary>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Patch(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement)
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.Execute(_currentRequest) as RestResponse;
            return response;
        }

        /// <summary>
        /// Post Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Patch<T>(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {

            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.PATCH, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }
            return response;
        }

        /// <summary>
        /// Overload with JSON Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Patch<T>(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");
            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }
            return response;

        }


        /// <summary>
        /// Exceutes Async and returns the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        public void PatchAsync<T>(DataFormat format, string urlIdentifier, object obj, Action<T> callback, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
            }
            client.ExecuteAsync<T>(_currentRequest, response =>
            {
                callback(response.Data);
            });
        }

        /// <summary>
        /// Exceutes Async and returns the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        public void PatchAsync(DataFormat format, string urlIdentifier, object obj, Action<string> callback, params string[] urlReplacement)
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PATCH, urlIdentifier, obj, urlReplacement);
                    break;
            }
            client.ExecuteAsync(_currentRequest, (response) =>
            {
                callback(response.Content);
            });
        }


        public RestResponse<T> PatchDataAsJsonString<T>(DataFormat format, string urlIdentifier, string data, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PATCH, urlIdentifier, data, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PATCH, urlIdentifier, data, urlReplacement);
                    break;
            }
            
            var results = client.Execute<T>(_currentRequest) as RestResponse<T>;
            if (null != results && null == results.Data && null != results.ContentType && results.ContentType.Contains("application/json"))
            {
                try
                {
                    results.Data = JsonConvert.DeserializeObject<T>(results.Content);
                }
                catch (Exception exception)
                {
                }
            }
            else
            {
            }


            return results; ;
        }

        #endregion

        #region Put Methods,Async and Typed

        /// <summary>
        /// PUT Request based in identifier
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Put(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement)
        {
            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.PUT, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute(_currentRequest) as RestResponse;
            return response;
        }

        /// <summary>
        /// Post Request based on json data and  response
        /// </summary>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Put(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement)
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.Execute(_currentRequest) as RestResponse;

            return response;
        }

        /// <summary>
        /// PUT Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Put<T>(string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {
            if (null == parameters)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(Method.PUT, urlIdentifier, parameters, urlReplacement);
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }

            return response;
        }

        /// <summary>
        /// Put based on Json data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Put<T>(DataFormat format, string urlIdentifier, object obj, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;

            }
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data && response.ContentType.Contains("application/json"))
            {
                try
                {
                    response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch (Exception exception)
                {
                }
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Put<T>(DataFormat format, string urlIdentifier, object obj, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PUT, urlIdentifier, obj, parameters, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PUT, urlIdentifier, obj, parameters, urlReplacement);
                    break;

            }
            var response = client.Execute<T>(_currentRequest) as RestResponse<T>;
            //Second Try if fails to Deserialize
            if (null != response && !string.IsNullOrEmpty(response.Content) && null == response.Data)
            {
                try
                {
                    if (response.ContentType.Contains("application/json"))
                    {
                        try
                        {
                            response.Data = JsonConvert.DeserializeObject<T>(response.Content);
                        }
                        catch (Exception exception)
                        {
                            response.Data = response.Content as T;
                        }
                    }
                    else
                    {
                        response.Data = response.Content as T;
                    }
                }
                catch (Exception exception)
                {
                }
            }

            return response;
        }


        /// <summary>
        /// Executes Asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <param name="urlReplacement"></param>
        public void PutAsAsync<T>(DataFormat format, string urlIdentifier, object obj, Action<T> callback, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PUT, urlIdentifier, obj, urlReplacement);
                    break;

            }
            client.ExecuteAsync<T>(_currentRequest, (response) =>
            {
                callback(response.Data);
            });
        }

        public RestResponse<T> PutDataAsJsonString<T>(DataFormat format, string urlIdentifier, string data, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(Method.PUT, urlIdentifier, data, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(Method.PUT, urlIdentifier, data, urlReplacement);
                    break;

            }
            var results = client.Execute<T>(_currentRequest) as RestResponse<T>;
            if (null != results && null == results.Data && !string.IsNullOrEmpty(results.ContentType) && results.ContentType.StartsWith("application/json"))
                try
                {
                    results.Data = JsonConvert.DeserializeObject<T>(results.Content);
                }
                catch (Exception exception)
                {
                }
            return results;
        }

        #endregion


        #region Generic Async Task

        /// <summary>
        /// Async Execution with Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="method"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="obj"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public Task<IRestResponse<T>> RestExecuteTaskAsync<T>(DataFormat format, Method method, string urlIdentifier, object obj, params string[] urlReplacement) where T : class, new()
        {
            RestClient client = null;
            if (null == obj)
                throw new ArgumentNullException("obj");
            switch (format)
            {
                case DataFormat.Xml:
                    client = PrepareRestRequestAsXml(method, urlIdentifier, obj, urlReplacement);
                    break;
                default:
                    client = PrepareRestRequestAsJson(method, urlIdentifier, obj, urlReplacement);
                    break;
            }
            var response = client.ExecuteTaskAsync<T>(_currentRequest);
            return response;
        }

        /// <summary>
        /// Async Execution with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(Method method, string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {
            if (null == parameters && method != Method.GET)
                throw new ArgumentNullException("parameters");
            var client = PrepareRestRequest(method, urlIdentifier, parameters, urlReplacement);
            var response = client.ExecuteTaskAsync<T>(_currentRequest);
            return response;
        }

        #endregion

        #region Delete

        /// <summary>
        /// DELETE Request based in identifier
        /// </summary>
        /// <param name="urlIdentifier"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse Delete(string urlIdentifier, params string[] urlReplacement)
        {
            var client = PrepareRestRequest(Method.DELETE, urlIdentifier, urlReplacement);
            return client.Execute(_currentRequest) as RestResponse;
        }

        /// <summary>
        /// Deletes Request based in identifier and gets T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="method"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Delete<T>(string urlIdentifier, Method method, params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.DELETE, urlIdentifier, urlReplacement);
            return client.Execute<T>(_currentRequest) as RestResponse<T>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlIdentifier"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestResponse<T> Delete<T>(string urlIdentifier, Method method, List<Parameter> parameters, params string[] urlReplacement) where T : class, new()
        {
            var client = PrepareRestRequest(Method.DELETE, urlIdentifier, parameters, urlReplacement);
            return client.Execute<T>(_currentRequest) as RestResponse<T>;
        }

        #endregion

        #region Prep the Request to be sent

        /// <summary>
        /// Prepares Request and returns the Rest Client opbject
        /// </summary>
        /// <param name="method"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestClient PrepareRestRequest(Method method, string urlIdentifier, params string[] urlReplacement)
        {
            return PrepareRestRequest(method, urlIdentifier, null, urlReplacement);
        }

        /// <summary>
        /// Prepares Request with list of params
        /// </summary>
        /// <param name="method"></param>
        /// <param name="urlIdentifier"></param>
        /// <param name="parameters"></param>
        /// <param name="urlReplacement"></param>
        /// <returns></returns>
        public RestClient PrepareRestRequest(Method method, string urlIdentifier, List<Parameter> parameters, params string[] urlReplacement)
        {
            RestClient client;

            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);
            //If the Parameters are passed.
            if (null != parameters && parameters.Count > 0)
                parameters.ForEach(p => _currentRequest.AddParameter(p.Name, p.Value, p.Type));
            //Log the request
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        #endregion

        #endregion

        #region Private Methods


        private RestClient PrepareRestRequestAsJson(Method method, string urlIdentifier, object data, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/json; charset=utf-8";

            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);
            if (null == data)
                throw new ArgumentNullException("data");
            //If the Parameters are passed.
            _currentRequest.JsonSerializer = new RestSharpJsonNetSerializer();
            var serData = _currentRequest.JsonSerializer.Serialize(data);
            _currentRequest.AddParameter("application/json; charset=utf-8", serData, ParameterType.RequestBody);
            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        private RestClient PrepareRestRequestAsJson(Method method, string urlIdentifier, object data, List<Parameter> parameters, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/json; charset=utf-8";

            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);
            if (null == data)
                throw new ArgumentNullException("data");
            //If the Parameters are passed.
            _currentRequest.JsonSerializer = new RestSharpJsonNetSerializer();
            var serData = _currentRequest.JsonSerializer.Serialize(data);
            _currentRequest.AddParameter("application/json; charset=utf-8", serData, ParameterType.RequestBody);

            //If the Parameters are passed.
            if (null != parameters && parameters.Count > 0)
                parameters.ForEach(p => _currentRequest.AddParameter(p.Name, p.Value, p.Type));

            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        private RestClient PrepareRestRequestAsJson(Method method, string urlIdentifier, string data, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/json; charset=utf-8";

            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);

            //If the Parameters are passed.

            _currentRequest.AddParameter("application/json; charset=utf-8", data, ParameterType.RequestBody);
            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        private RestClient PrepareRestRequestAsXml(Method method, string urlIdentifier, object data, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/xml; charset=utf-8";
            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);
            if (null == data)
                throw new ArgumentNullException("data");
            //If the Parameters are passed.

            var serData = data.ToXmlSerializer();
            _currentRequest.AddParameter("application/xml; charset=utf-8", serData, ParameterType.RequestBody);
            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        private RestClient PrepareRestRequestAsXml(Method method, string urlIdentifier, object data, List<Parameter> parameters, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/xml; charset=utf-8";
            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);
            if (null == data)
                throw new ArgumentNullException("data");
            //If the Parameters are passed.

            var serData = data.ToXmlSerializer();
            _currentRequest.AddParameter("application/xml; charset=utf-8", serData, ParameterType.RequestBody);

            //If the Parameters are passed.
            if (null != parameters && parameters.Count > 0)
                parameters.ForEach(p => _currentRequest.AddParameter(p.Name, p.Value, p.Type));

            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }

        private RestClient PrepareRestRequestAsXml(Method method, string urlIdentifier, string data, params string[] urlReplacement)
        {
            RestClient client;
            ContentType = "application/xml; charset=utf-8";
            _currentRequest = ConstructCurrentRequest(method, urlIdentifier, out client, urlReplacement);

            //If the Parameters are passed.

            _currentRequest.AddParameter("application/xml; charset=utf-8", data, ParameterType.RequestBody);
            _currentRequest.RequestFormat = DataFormat.Json;
            
            CurrentRestRequest = _currentRequest;
            return client;
        }


        private RestRequest ConstructCurrentRequest(Method method, string urlIdentifier, out RestClient client, params string[] urlReplacement)
        {
            string urlPath;

            if (!Uri.TryCreate(urlIdentifier, UriKind.Absolute, out _requestUri))
            {
                urlPath = urlIdentifier;
            }

            BaseUrl = string.Concat(_requestUri.Scheme, "://" + _requestUri.Authority);
            
            _currentRequest = new RestRequest(_requestUri.PathAndQuery, method);

            if (method == Method.POST || method == Method.PUT)
                _currentRequest.AddHeader("Content-type", string.IsNullOrEmpty(ContentType) ? @"application\x-www-form-urlencoded" : ContentType);

            client = this;
            return _currentRequest;
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for HTTP Objects.
    /// <remarks>
    /// See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
    /// for details of implementation decisions.
    /// </remarks>
    /// </summary>
    public static class RestExtensions
    {
        /// <summary>
        /// Dump the raw http request to a string. 
        /// </summary>
        /// <param name="request">The <see cref="RestRequest"/> that should be dumped.       </param>
        /// <param name="client"></param>
        /// <returns>The raw HTTP request.</returns>
        public static string ToRaw(this RestRequest request, RestClient client)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter())
            {
                WriteStartLine(request, client, writer);
                writer.Write(Environment.NewLine);
                WriteHeaders(request, writer);
                WriteBody(request, writer);
                sb.Append(writer);
            }
            return sb.ToString();
        }

        private static void WriteStartLine(RestRequest request, RestClient client, StringWriter writer)
        {
            const string space = " ";

            writer.Write(request.Method);
            writer.Write(space + client.BaseUrl + request.Resource);
        }

        private static void WriteHeaders(RestRequest request, StringWriter writer)
        {
            request.Parameters.Where(p => p.Type == ParameterType.HttpHeader).ToList().ForEach(header =>
            {
                writer.WriteLine("{0}: {1}", header.Name, header.Value);
                writer.Write(Environment.NewLine);
            });
        }

        private static void WriteBody(RestRequest request, StringWriter writer)
        {
            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if (body != null)
            {
                writer.WriteLine(body);
            }

        }
    }


    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class RestSharpJsonNetSerializer : ISerializer, IDeserializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        /// <summary>
        /// Default serializer
        /// </summary>
        public RestSharpJsonNetSerializer()
        {
            ContentType = "application/json; charset=utf-8";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public RestSharpJsonNetSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json; charset=utf-8";
            _serializer = serializer;
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        /// <summary>
        /// DeSerializes to the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T Deserialize<T>(IRestResponse response)
        {
            if (null != response)
                return JsonConvert.DeserializeObject<T>(response.Content);
            return default(T);
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }

    }
}
