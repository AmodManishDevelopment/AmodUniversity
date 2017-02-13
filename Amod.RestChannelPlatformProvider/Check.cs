using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.RestChannelPlatformProvider
{
    public static class Check
    {
        #region NotEmpty

        /// <summary>Not Empty Test - byte array cannot be empty</summary>
        /// <param name="array">Byte array to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(byte[] array, string paramName)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("The byte array, " + paramName + ", can't be empty.", paramName);
            }
        }

        /// <summary>Not Empty Test - Guid cannot be empty</summary>
        /// <param name="guid">Guid to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(Guid guid, string paramName)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("The guid, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>Not Empty Test - Integer cannot be empty</summary>
        /// <param name="x">nullable integer to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(int? x, string paramName)
        {
            if (!x.HasValue)
            {
                throw new ArgumentException("The int, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>Not Empty Test - Unnamed string cannot be empty</summary>
        /// <param name="str">string to test</param>
        public static void NotEmpty(string str)
        {
            NotEmpty(str, "[unnamed]");
        }

        /// <summary>Not Empty Test - String cannot be empty</summary>
        /// <param name="str">string to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        public static void NotEmpty(string str, string paramName)
        {
            NotEmpty(str, paramName, null);
        }

        /// <summary>Not Empty Test - String cannot be empty</summary>
        /// <param name="str">string to test</param>
        /// <param name="paramName">Parameter Name to test</param>
        /// <param name="message">Message to raise as exception if string is empty</param>
        public static void NotEmpty(string str, string paramName, string message)
        {
            if (str == null || str.Trim().Length <= 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Not Empty Test - collection cannot be empty
        /// </summary>
        /// <param name="col"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void NotEmpty(ICollection col, string paramName, string message)
        {
            if (col == null || col.Count == 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        #endregion

        #region Empty

        /// <summary>Not Empty Test - Unnamed string cannot be empty</summary>
        /// <param name="str">string to test</param>
        public static void Empty(string str, string paramName, string message)
        {
            if (!string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(message, paramName);
            }
        }

        #endregion

        #region NotNull

        /// <summary>Not Null Test - Object cannot be null</summary>
        /// <param name="obj">Object to test</param>
        /// <param name="objectName">Name of object to test</param>
        public static void NotNull(object obj, string objectName)
        {
            NotNull(obj, objectName, null);
        }

        /// <summary>Not Null Test - Object cannot be null</summary>
        /// <param name="obj">Object to test</param>
        /// <param name="objectName">Name of object to test</param>
        /// <param name="message">Message to raise as exception if object is empty</param>
        public static void NotNull(object obj, string objectName, string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(string.Format("The object, {0}, can not be null. {1}", objectName, message));
            }
        }

        #endregion
    }
}
