using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace TechSummit2018.ServerlessSmartSpeaker
{
    public class ProtcolBufJsonResult : IActionResult
    {
        private readonly object _obj;
        private readonly JsonFormatter _formatter;

        public ProtcolBufJsonResult(object obj, JsonFormatter formatter)
        {
            _obj = obj;
            _formatter = formatter;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("Content-Type", new Microsoft.Extensions.Primitives.StringValues("application/json; charset=utf-8"));
            var stringWriter = new StringWriter();
            _formatter.WriteValue(stringWriter, _obj);
            await context.HttpContext.Response.WriteAsync(stringWriter.ToString());
        }
    }
}
