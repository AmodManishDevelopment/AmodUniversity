using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Amod.RestChannelPlatformProvider
{
    public static class SerializationHelper
    {
        #region SOAP

        private static readonly SoapFormatter SoapFormatter = new SoapFormatter();

        /// <summary>
        /// Returns the SOAP string representation of this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToSoap(this object obj)
        {
            Check.NotNull(obj, "object");

            string str;

            using (var stream = new MemoryStream())
            {
                SoapFormatter.Serialize(stream, obj);
                str = ReadContentsFromStream(stream, false);
            }

            return str;
        }

        /// <summary>
        /// Returns an object from this SOAP string.
        /// </summary>
        /// <param name="soap"></param>
        /// <returns></returns>
        public static object ToObjectFromSoap(this string soap)
        {
            Check.NotEmpty(soap, "soap string");

            MemoryStream stream = null;
            object obj;

            try
            {
                stream = CreateMemoryStream(soap, Encoding.Default);
                obj = SoapFormatter.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return obj;
        }

        #endregion

        /// <summary>
        /// Creates an object from a Data Contract serialized string.
        /// </summary>
        /// <param name="dataContract"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToObjectFromDataContract(this Type type, string dataContract)
        {
            Check.NotEmpty(dataContract, "dataContract");

            object obj;

            using (var xmlTextReader = new XmlTextReader(CreateMemoryStream(dataContract, Encoding.UTF8)))
            {
                var dataContractSerializer = new DataContractSerializer(type);
                obj = dataContractSerializer.ReadObject(xmlTextReader);
            }

            return obj;
        }

        /// <summary>
        /// Returns the Data Contract serialization of the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDataContract(this object obj)
        {
            Check.NotNull(obj, "object");

            using (var stream = new MemoryStream())
            {
                var dataContractSerializer = new DataContractSerializer(obj.GetType());
                dataContractSerializer.WriteObject(stream, obj);

                return ReadContentsFromStream(stream, false);
            }
        }

        /// <summary>
        /// Writes XML Serialization to stream and returns the string..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultNameSpace"></param>
        /// <returns></returns>
        public static string ToXmlSerializer<T>(this T obj, string defaultNameSpace = "")
        {

            return obj.ToXml(defaultNameSpace);

        }

        /// <summary>
        /// Writes XML Serialization to stream and returns the string..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FromXmlSerializer<T>(this string data)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(data)))
            {
                return (T)serializer.Deserialize(stream);
            }
        }


        /// <summary>
        /// Perform a deep Copy of the object.
        /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// Provides a method for performing a deep copy of an object.
        /// Binary Serialization is used to perform the copy.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // return (T)ToDataContractXml(source);
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        #region XML

        /// <summary>
        /// Returns the XML string representaion on this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            return obj.ToXml(string.Empty);
        }

        /// <summary>Returns the XML string representaion on this object using this default namespace.</summary>
        /// <param name="obj">Object to convert</param>
        /// <param name="defaultNamespace">Default XML namespace</param>
        /// <returns>XML Formatted string represenataion of object</returns>
        public static string ToXml(this object obj, string defaultNamespace)
        {
            if (null == obj)
                return string.Empty;
            using (var stream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(obj.GetType(), defaultNamespace);
                xmlSerializer.Serialize(stream, obj);
                return ReadContentsFromStream(stream, false);
            }
        }

        /// <summary>
        /// Serialize an object to a string
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>serialized XML string</returns>
        public static string ToDataContractXml(this object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            string data = string.Empty;
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                using (var sr = new StreamReader(ms))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }


        /// <summary>
        /// Returns deserialized object from JSON string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToDataContract<T>(this string str)
        {
            T obj_rval = Activator.CreateInstance<T>();
            DataContractSerializer serializer = new DataContractSerializer(obj_rval.GetType());

            try
            {
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(str)))
                {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Read Contents From Stream
        /// </summary>
        /// <param name="stream">Stream object</param>
        /// <param name="keepOpen">boolean to choose if stream need to be closed</param>
        /// <returns>string contents of stream</returns>
        public static string ReadContentsFromStream(this Stream stream, bool keepOpen)
        {
            if (null == stream || !stream.CanRead)
                return string.Empty;

            string str;

            try
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                var streamReader = new StreamReader(stream, Encoding.UTF8, true);
                str = streamReader.ReadToEnd();
                streamReader.Close();
            }
            finally
            {
                if (!keepOpen)
                    stream.Close();
            }

            return str;
        }

        /// <summary>
        /// Create Memory Stream
        /// </summary>
        /// <param name="contents">Contents of stream</param>
        /// <param name="encoding">Encoding of stream</param>
        /// <returns>
        /// Non-resizable Memory Stream loaded with contents
        /// </returns>
        public static MemoryStream CreateMemoryStream(this string contents, Encoding encoding)
        {
            Check.NotNull(contents, "contents");

            byte[] bytes = encoding.GetBytes(contents);
            return new MemoryStream(bytes);
        }
        #endregion

        #region JSON Serialization

        /// <summary>
        /// Returns JSON version
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                return jsonString;
            }
        }


        /// <summary>
        /// Returns deserialized object from JSON string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToNewtonSoftJSON<T>(this T obj)
        {
            string retunVal;

            try
            {
                retunVal = JsonConvert.SerializeObject(obj);

            }
            catch
            {
                throw;
            }
            return retunVal;
        }


        /// <summary>
        /// Returns deserialized object from JSON string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FromNewtonSoftJSON<T>(this string data)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            var retunVal = JsonConvert.DeserializeObject<T>(data, settings);
            return retunVal;
        }



        /// <summary>
        /// Returns deserialized object from JSON string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T FromJSON<T>(this string str)
        {
            T obj_rval = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj_rval.GetType());

            try
            {
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(str)))
                {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns deserialized object from JSON string.
        /// </summary>
        public static T FromJSON<T>(this object obj)
        {
            return FromJSON<T>(obj.ToString());
        }




        #endregion

        #region XML Validation

        /// <summary>
        /// Validates xml against provided schema
        /// </summary>
        /// <param name="xelement"></param>
        /// <param name="xmlSchemaPath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ValidateXmlSchema(XElement xelement, string xmlSchemaPath, out string error)
        {
            error = string.Empty;
            bool validXml = false;

            if (null == xelement)
                throw new ArgumentNullException("xelement");

            if (string.IsNullOrEmpty(xmlSchemaPath))
                throw new ArgumentNullException("xmlSchemaPath");

            try
            {
                // validate the xml against the schema
                var schemas = new XmlSchemaSet();
                schemas.Add(null, xmlSchemaPath);
                new XDocument(xelement).Validate(schemas, null);

                validXml = true;
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return validXml;
        }


        #endregion


    }
}
