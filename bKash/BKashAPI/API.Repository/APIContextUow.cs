using API.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Repository
{
    public class APIContextUow : gBankerBUROAPIEntities
    {
        private readonly gBankerBUROAPIEntities _context;

        public APIContextUow()
        {
            _context = new gBankerBUROAPIEntities();
        }

        public APIContextUow(gBankerBUROAPIEntities context)
        {
            _context = context;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public gBankerBUROAPIEntities Context
        {
            get { return _context; }
        }

        public new void Dispose()
        {
            _context.Dispose();
        }
    }
}
