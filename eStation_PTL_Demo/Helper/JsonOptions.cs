using System.Text.Encodings.Web;
using System.Text.Json;

namespace eStation_PTL_Demo.Helper
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}