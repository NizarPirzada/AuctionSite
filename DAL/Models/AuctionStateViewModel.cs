using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionSite.DAL.Models
{
    public class AuctionStateViewModel
    {
        public string StateName { get; set; }
        public int Count { get; set; }
        public bool Filtered { get; set; }
    }
}
