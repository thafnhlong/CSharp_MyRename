using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Do_An_2.Model
{
    public class ioInfo
    {
        public string Name { get; set; }
        public XmlNode Node { get; set; }
        public ioInfo Parent { get; set; }
    }
}
