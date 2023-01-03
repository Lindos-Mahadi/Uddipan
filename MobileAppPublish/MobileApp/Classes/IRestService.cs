using GBPMS.Droid.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Droid
{
    public interface IRestService
    {
		Task<List<LookupItem>> GetOfficeListAsync();
		
	}    
    public class TodoItem
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool Done { get; set; }
    }
}
