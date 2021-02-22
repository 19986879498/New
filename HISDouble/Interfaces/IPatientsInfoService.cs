using HISDouble.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IPatientsInfoService
    {
        Patientsinfo SelectByCardNo(string CardNo, string InpatientNo,ref Patientsinfo patientsinfo);
    }
}
