using System;
using System.IO;
using ProtoBuf;

namespace SubstringFramework.Util
{
    public static class ProtobufUtil
    {
        public static bool TrySerialize<T>(T data, out byte[] result)
        {
            result = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, data);
                    result = stream.ToArray();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool TryDeserialize<T>(byte[] data, out T result)
        {
            result = default(T);
            try
            {
                using (var stream = new MemoryStream(data))
                {
                    result = Serializer.Deserialize<T>(stream);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public static byte[] Serialize<T>(T data)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, data);
                result = stream.ToArray();
            }
            return result;
        }

        public static T Deserialize<T>(byte[] data)
        {
            T result;

            using (var stream = new MemoryStream(data))
            {
                result = Serializer.Deserialize<T>(stream);
            }

            return result;
        }
    }
}

