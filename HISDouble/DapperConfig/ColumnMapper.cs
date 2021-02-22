using Dapper;
using HISDouble.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.DapperConfig
{
    public class ColumnMapper
    {
        public static void SetMapper()
        {
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Annex), new ColumnAttributeTypeMapper<Annex>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(CheckInDataList), new ColumnAttributeTypeMapper<CheckInDataList>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Diagnosis), new ColumnAttributeTypeMapper<Diagnosis>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Diagnosisdic), new ColumnAttributeTypeMapper<Diagnosisdic>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Hospitaldic), new ColumnAttributeTypeMapper<Hospitaldic>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Medicaldic), new ColumnAttributeTypeMapper<Medicaldic>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Order), new ColumnAttributeTypeMapper<Order>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Patientsinfo), new ColumnAttributeTypeMapper<Patientsinfo>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Referral), new ColumnAttributeTypeMapper<Referral>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(Userinfo), new ColumnAttributeTypeMapper<Userinfo>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_Beds), new ColumnAttributeTypeMapper<View_Beds>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_BedsInfo), new ColumnAttributeTypeMapper<View_BedsInfo>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_contractunit), new ColumnAttributeTypeMapper<View_contractunit>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_Diagnosis), new ColumnAttributeTypeMapper<View_Diagnosis>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_GetCheckindata), new ColumnAttributeTypeMapper<View_GetCheckindata>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_Order), new ColumnAttributeTypeMapper<View_Order>());
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(View_PreRegister), new ColumnAttributeTypeMapper<View_PreRegister>());
            SqlMapper.SetTypeMap(typeof(View_PatientsInfo), new ColumnAttributeTypeMapper<View_PatientsInfo>());
            //每个需要用到[colmun(Name="")]特性的model，都要在这里添加映射

        }

    }
}
