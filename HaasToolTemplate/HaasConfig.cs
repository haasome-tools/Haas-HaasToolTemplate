using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HaasToolTemplate
{
    public class HaasConfig
    {
        public string IPAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8096;
        public string Secret { get; set; } = "SomeSecretHere";
        public string AccountGUID { get; set; } = "ReplaceMeWithGuid";

        public int BackTestLength { get; set; } = 1440;
        public string PrimaryCurrency { get; set; } = "BTC";
    }
}
