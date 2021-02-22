using HISDouble.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
  public  interface IView_QueryAuditService
    {
        //Microsoft.AspNetCore.Mvc.JsonResult QueryAudit(string hoscode, string depcode);
        JsonResult QueryAudit(string hoscode, string depcode);
    }
}
