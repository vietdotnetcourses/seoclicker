using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeoClicker.Models
{
    public class Settings
    {
        public DataServerSettings DataServerSettings { get; set; }
        public ProxySettings ProxySettings { get; set; }
        public TaskSettings TaskSettings { get; set; }
        public List<SelectionItem> Devices { get; set; }
    }
}
