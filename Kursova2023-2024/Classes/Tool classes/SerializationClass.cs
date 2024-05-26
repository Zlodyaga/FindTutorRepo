using System.Runtime.Serialization.Json;

namespace Kursova2023_2024.Classes
{
    static class SerializationClass
    {
        public static void SerialiazeToJson<T>(ref T inObject, string inFileName)
        {
            File.Delete(inFileName);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (FileStream stream1 = new FileStream(inFileName, FileMode.OpenOrCreate))
            {
                 ser.WriteObject(stream1, inObject);
                 stream1.Close();
            }
        }

        public static void DeserializationFromJson<T>(ref T inObject, string inFileName)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (FileStream stream1 = new FileStream(inFileName, FileMode.Open))
            {
                  inObject = (T)ser.ReadObject(stream1);
                  stream1.Close();
            }
        }
    }
}
