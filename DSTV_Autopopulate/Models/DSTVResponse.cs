Efficiently
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTV_Autopopulate.Models
{
    public class DSTVResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public Availablepricingoption[] availablePricingOptions { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Availablepricingoption
    {
        public int monthsPaidFor { get; set; }
        public int price { get; set; }
        public int invoicePeriod { get; set; }
    }

}
