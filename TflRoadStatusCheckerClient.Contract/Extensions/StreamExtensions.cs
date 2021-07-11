using Newtonsoft.Json;
using System;
using System.IO;

namespace TflRoadStatusCheckerClient.Contract.Extensions
{
    public static class StreamExtensions
    {
        public static T DeserializeFromJson<T>(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead) throw new NotSupportedException("can't read from stream");

            using (var sread = new StreamReader(stream))
            using (var jsonText = new JsonTextReader(sread))
            {
                var serialiser = new JsonSerializer();
                return serialiser.Deserialize<T>(jsonText);
            }
        }
    }
}