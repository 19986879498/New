using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models.Base
{
     public class Params
    {
        public virtual Referral referral { get; set; }
        public virtual DiagnosisParams Diagnosis { get; set; }
        public virtual OrderParams Order { get; set; }
    }
    public class DiagnosisParams
    {
        public virtual List<Diagnosis> diagnosis { get; set; }
        public virtual int id { get; set; }
    }
    public class OrderParams
    {
        public virtual List<Order> order { get; set; }
        public virtual int id { get; set; }
    }
    public class DiviceParams
    {
        public virtual string DeptCode { get; set; }
    }
    public class CheckDataParams
    {
        public virtual PreInspection preInspection { get; set; }
      
    }
}
