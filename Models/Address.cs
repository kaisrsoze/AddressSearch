using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressSearch.Models
{
    public class Address
    {
        public string street { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        //zestimate
        
        public decimal amount { get; set; }
        public decimal valueChange { get; set; }
        public decimal low { get; set; }
        public decimal high { get; set; }
        //links
        public string homeDetails { get; set; }
        public string graphsAndData { get; set; }
        public string mapThisHome { get; set; }
        public string comparables { get; set; }
        //local real estate
        public string regionName { get; set; }
        public string regionType { get; set; }
        public double zindexValue { get; set; }
        public string forSaleByOwner { get; set; }
        public string forSale { get; set; }
    }
}
