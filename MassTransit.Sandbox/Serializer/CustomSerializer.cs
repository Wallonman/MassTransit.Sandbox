using System;
using System.IO;
using System.Net.Mime;
using System.Runtime.Serialization;
using MassTransit.Metadata;
using MassTransit.Serialization;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Serializer
{
    public class CustomSerializer : IMessageSerializer
    {

        public static readonly ContentType CustomContentType = new ContentType("application/vnd.masstransit+jsoncustom");

        public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
        {
            try
            {
                context.ContentType = JsonMessageSerializer.JsonContentType;
                JsonMessageEnvelope jsonMessageEnvelope = new JsonMessageEnvelope((SendContext)context, (object)context.Message, TypeMetadataCache<T>.MessageTypeNames);
                using (StreamWriter streamWriter = new StreamWriter(stream, JsonMessageSerializer._encoding.Value, 1024, true))
                {
                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter)streamWriter))
                    {
                        jsonTextWriter.Formatting = Formatting.Indented;
                        JsonMessageSerializer._serializer.Value.Serialize((JsonWriter)jsonTextWriter, (object)jsonMessageEnvelope, typeof(MessageEnvelope));
                        jsonTextWriter.Flush();
                        streamWriter.Flush();
                    }
                }
            }
            catch (SerializationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public ContentType ContentType => CustomContentType;
    }
}