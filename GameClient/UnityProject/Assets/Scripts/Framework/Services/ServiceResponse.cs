using Newtonsoft.Json.Linq;

public class ServiceResponse
{
    public int SerialID { get; private set; }
    public JObject Content;
}
