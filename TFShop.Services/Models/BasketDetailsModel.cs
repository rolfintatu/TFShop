using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services.Models
{
    public class BasketDetailsModel
    {
        public BasketDetailsModel(double subtotal, double vAT, double total)
        {
            Subtotal = subtotal;
            VAT = vAT;
            Total = total;
        }

        public double Subtotal { get; set; }
        public double VAT { get; set; }
        public double Total { get; set; }
    }
}
