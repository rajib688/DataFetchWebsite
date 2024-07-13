using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Entities
{
    public class WebsiteData
    {
        public int Id { get; set; }
        public string? TradingCode { get; set; }
        public string? LTP { get; set; }
        public string? High { get; set; }
        public string? Low { get; set; }
        public string? ClosePrice { get; set; }
        public string? YCP { get; set; }
        public string? Change { get; set; }
        public string? Trade { get; set; }
        public string? Value { get; set; }
        public string? Volume { get; set; }
    }
}
