using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    public class DurationTable
    {
        public int ID { get; set; }
        public string Frequency { get; set; }
        public int Duration { get; set; }
        public string ProductPaymentFrequency { get; set; }
    }
}
