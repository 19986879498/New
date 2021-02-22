using HISDouble.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Models.Base;

namespace HISDouble.Interfaces
{
  public  interface IView_QueryByAuditService
    {
        JsonResult QueryByAudit(RequestQueryParam Param);
    }
}
