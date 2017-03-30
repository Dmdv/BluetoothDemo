using JetBrains.Annotations;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace BluetoothDemo.Extensions
{
    public static class ObjectExtensions
    {
        [NotNull]
        public static byte[] ToBinary([NotNull] this object target)
        {
            using (var serializationStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(serializationStream, target);
                return serializationStream.ToArray();
            }
        }

        [NotNull]
        public static byte[] ToBinary(this RSAParameters target)
        {
            return target.FromKey().ToBinary();
        }

        public static RSAParameters FromBinary(this byte[] array)
        {
            return array.FromBinary<string>().ToKey();
        }

        [NotNull]
        public static TTarget FromBinary<TTarget>([NotNull] this byte[] array)
        {
            using (var memoryStream = new MemoryStream(array))
            {
                var graph = new BinaryFormatter().Deserialize(memoryStream);

                if (graph is TTarget)
                {
                    return (TTarget) graph;
                }

                throw new ArgumentException($"Cannot convert from '{typeof(TTarget).Name}' to {graph.GetType()}");
            }
        }

        public static RSAParameters ToKey([NotNull] this string key)
        {
            using (var sr = new StringReader(key))
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                return (RSAParameters) xs.Deserialize(sr);
            }
        }

        [NotNull]
        public static string FromKey(this RSAParameters parameters)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, parameters);
                return sw.ToString();
            }
        }
    }
}