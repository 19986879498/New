using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_PatientsInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        List<View_PatientsInfo> zjPatientsQuery(string hoscode, string depcode);
        /// <summary>
        /// 下转
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        List<View_xzPatientsInfo> xzPatientsQuery(string hoscode, string depcode);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        //Microsoft.AspNetCore.Mvc.JsonResult PatientsQuery(string hoscode, string depcode, string level);
        JsonResult PatientsQuery(string hoscode, string depcode, string level);
    }
}
