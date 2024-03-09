using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ObjectSerializer
{
    /// <summary>
    /// Serialize an object to a string quickly
    /// </summary>
    public static string SerializeObject(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        List<string> keyValuePairs = new List<string>();

        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj);

            if (value is IEnumerable enumerable && !(value is string))
            {
                // If the property is an array or collection, serialize its elements
                List<string> arrayValues = new List<string>();
                foreach (var element in enumerable)
                {
                    arrayValues.Add(element.ToString());
                }
                keyValuePairs.Add($"{property.Name}:[{string.Join(",", arrayValues)}]");
            }
            else
            {
                // Serialize regular properties
                keyValuePairs.Add($"{property.Name}:{value}");
            }
        }

        string result = string.Join(";", keyValuePairs);

        return result; // serialized packet
    }

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

                if (property.PropertyType.IsArray)
                {
                    // If the property is an array, convert the string to an array of the property's type
                    string[] arrayValues = value.Trim('[', ']').Split(',');
                    Array array = Array.CreateInstance(property.PropertyType.GetElementType(), arrayValues.Length);

                    for (int i = 0; i < arrayValues.Length; i++)
                    {
                        array.SetValue(Convert.ChangeType(arrayValues[i], property.PropertyType.GetElementType()), i);
                    }

                    property.SetValue(obj, array);
                }
                else
                {
                    // Set the value for regular properties
                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                }
            }
        }

        return obj; // return the deserialized object
    }

    /// <summary>
    /// Deserialize a serialized string to a dictionary
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