
namespace Crossroads.Service.Finance.Models
{
    public class QueryParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public QueryParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
