using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseHookTest
{
    public class Config
    {
        public List<ConfigDetail> Xconfig { get; set; } = new List<ConfigDetail>();
        public List<ConfigDetail> Yconfig { get; set; } = new List<ConfigDetail>();
    }

    public class ConfigDetail
    {
        public int Offset { get; set; } = 0;

        public int Rate { get; set; } = 1;
    }
}
