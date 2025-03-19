using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PdfData
    {
        public PdfData(string text, List<string> images)
        {
            this.text = text;
            this.images = images;
        }

        public string text { get; set; }
        public List<string> images { get; set; }
    }
}
