using HISDouble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IDiagnosisdicService
    {
        /// <summary>
        /// 获取转出诊断信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        List<Diagnosisdic> SelectAll(string code,string name);
       
    }
}
