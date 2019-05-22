using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows
{
    public class Request
    {
        private int type;

        public int Type
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

        private string content;

        public static int Type_Info = 1;
        public static int Type_State = 2;
        public static int Type_HeatingPower = 3;
    }
}
