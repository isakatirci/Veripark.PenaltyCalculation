using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veripark.PenaltyCalculation.MvcUI.Models.ViewModels
{
    public class MyJsonResult
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public MyJsonResult()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}