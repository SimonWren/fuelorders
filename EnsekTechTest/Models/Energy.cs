using System;
using System.Collections.Generic;
using System.Text;

namespace EnsekTechTest.Models
{
    public class Energy
    {
        public EnergyDetails electric { get; set; }
        public EnergyDetails gas { get; set; }
        public EnergyDetails nuclear { get; set; }
        public EnergyDetails oil { get; set; }
    }
}
