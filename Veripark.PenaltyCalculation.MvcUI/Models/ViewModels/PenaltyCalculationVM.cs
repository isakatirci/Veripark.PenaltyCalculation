using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veripark.PenaltyCalculation.MvcUI.Models.ViewModels
{
    public class PenaltyCalculationVM
    {
        public string CountryId { get; set; }
        public string CheckedOutDate { get; set; }
        public string ReturnedDate { get; set; }
    }
}