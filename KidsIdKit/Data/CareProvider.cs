using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsIdKit.Data
{
    public class CareProvider : Person
    {
        public string ClinicName { get; set; }
        public string CareRoleDescription { get; set; }
        public string ProviderName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
