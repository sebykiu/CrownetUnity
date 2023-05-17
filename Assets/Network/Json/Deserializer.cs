using System;
using Newtonsoft.Json;

namespace Network.Json
{
    public static class JsonDeserializer
    {
        public static Message Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }
    }

    [Serializable]
    public  class Message
    {
        public string Id { get; set; }
        public Coordinates Coordinates { get; set; }
    

    }

    [Serializable]
    public  class Coordinates
    {
        public double x { get; set; }
        public double y { get; set; }
        public double Z { get; set; }
    }
}