namespace AsyncNetworking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ObjectSerializer
    {
        /// <summary>
        /// Serialize an object to a string quickly
        /// </summary>
        public static string Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            List<string> keyValuePairs = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                keyValuePairs.Add($"{property.Name}:{value}");
            }

            string result = string.Join(";", keyValuePairs);

            return result; // serialized packet
        }

        /// <summary>
        /// deserialize a string to an object (This is costly so use ParseSerializedString if you can)
        /// </summary>
        public static T Deserialize<T>(string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
                throw new ArgumentNullException(nameof(serialized));

            Dictionary<string, string> keyValuePairs = ParseSerializedString(serialized);

            Type type = typeof(T);
            T obj = (T)Activator.CreateInstance(type);

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (keyValuePairs.ContainsKey(property.Name))
                {
                    string value = keyValuePairs[property.Name];
                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                }
            }

            return obj; // return the deserialized object
        }

        /// <summary>
        /// Deserialize a serialized string to a dictionary of key value pairs
        /// </summary>
        public static Dictionary<string, string> ParseSerializedString(string serialized)
        {
            string[] pairs = serialized.Split(';');

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                keyValuePairs.Add(keyValue[0], keyValue[1]);
            }

            return keyValuePairs;
        }
    }
}