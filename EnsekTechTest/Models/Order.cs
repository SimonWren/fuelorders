using System;
using System.Collections.Generic;
using System.Text;

namespace EnsekTechTest.Models
{
    public class Order
    {
        public string?fuel { get; set; }
        public string? id { get; set; }
        public int quantity { get; set; }
        public string? time { get; set;  }
        public DateTime ts_time => DateTime.Parse(time??string.Empty);


    }
}
