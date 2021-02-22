using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IOutpatientService
    {
        List<HISDouble.Models.Outpatient> GetOutpatients(string doctCode, string deptCode, ref string Err);
        List<Models.Outpatient> GetOutpatients(string deptCode, ref string Err);
        JsonResult GetOutPatientResult(string doctCode, string deptCode);
        JsonResult GetOutPatientResult(string deptCode);
    }
}
