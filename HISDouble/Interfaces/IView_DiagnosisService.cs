using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
  public  interface IView_DiagnosisService
    {
        List<View_Diagnosis> DiagnosisQuery(string hoscode, string depcode);
        List<View_xzDiagnosis> xzDiagnosisQuery(string hoscode, string depcode);
        JsonResult getDiagnosisResult(string hoscode, string depcode, string level);
    }
}
