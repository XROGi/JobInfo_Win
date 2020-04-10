using System.Xml.Serialization;

namespace Ji.Services
{
    [XmlRoot("user", Namespace = "http://localhost/xrogi", IsNullable = false)]

    public class User
    {
        //<user xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="iu.smirnov" xmlns="http://localhost/xrogi">
        [XmlAttribute(AttributeName = "name")]
        public string name; // Login or ID_TOKEN_BAR
        [XmlAttribute(AttributeName = "id")]
        public string id;
        public Device device;
    }
}
