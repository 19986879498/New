using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IOrderService
    {
        List<View_Order> OrderQuery(string hoscode, string depcode);
        List<View_xzOrder> xzOrderQuery(string hoscode,string depcode);
        JsonResult getOrderResult(string hoscode, string depcode,string level);
    }
}
