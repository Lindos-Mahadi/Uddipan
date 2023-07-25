using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBanker.Data.CodeFirstMigration
{
    public  class ProductIdentification
    {
        public int ID { get; set; }
        public string Termdeposit { get; set; }
        public string VoluntarySavings { get; set; }
        public string GeneralSavings { get; set; }
        public string SpecialSavings { get; set; }
    }
}
