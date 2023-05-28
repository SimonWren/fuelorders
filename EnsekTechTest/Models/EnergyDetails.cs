using System;
using System.Collections.Generic;
using System.Text;

namespace EnsekTechTest.Models
{
    public class EnergyDetails
    {
        public string energy_id { get; set; }
        public decimal price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }

    }
}
