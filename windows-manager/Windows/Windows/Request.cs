using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows
{
    public class Request
    {
        private string type;

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public string Content
        {
            get
            {
                return content;
            }

            set
            {
                content = value;
            }
        }

        public string FullRequest { get => fullRequest; set => fullRequest = value; }

        private string fullRequest;

        private string content;

        public const string Type_Info = "-i";
        public const string Type_State = "-s";
        public const string Type_HeatingPower = "-p";
    }
}
