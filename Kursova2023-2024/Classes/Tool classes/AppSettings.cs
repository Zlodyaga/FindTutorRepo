using System.Runtime.Serialization;

namespace Kursova2023_2024.Classes.Tool_classes
{
    [DataContract]
    public class AppSettings
    {
        [DataMember]
        public string themeColor { get; set; }
    }
}
