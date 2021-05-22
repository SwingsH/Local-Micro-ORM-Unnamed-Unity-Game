using Newtonsoft.Json.Linq;

namespace TIZSoft.Services
{
    public class ServiceResponse
    {
        public int SerialID { get; private set; }
        public JObject Content;
    }
}