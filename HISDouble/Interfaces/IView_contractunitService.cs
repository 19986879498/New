using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
    public interface IView_contractunitService
    {
        /// <summary>
        /// 查询合同单位
        /// </summary>
        /// <returns></returns>
        List<View_contractunit> SelectAll();
        /// <summary>
        /// 查询合同单位的返回结果
        /// </summary>
        /// <returns></returns>
        JsonResult ContractUnitQuery();

    }
}
