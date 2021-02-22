using Dapper.Contrib.Extensions;
using HISDouble.DapperConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    [Dapper.Contrib.Extensions.Table("zjhis.turn_REFERRAL")]
    public class Referral
    {
        //public Referral()
        //{
        //    Patientsinfo = new  Patientsinfo();
        //}
        [Column(Name ="ID")]
     [Key]
        public virtual int ID { get; set; } = 0;
        [Column(Name = "HOSPITALOUT")]
        public virtual string Hospitalout { get; set; } = "";
        [Column(Name = "DEPARTMENTSOUT")]
        public virtual string Departmentsout { get; set; } = "";
        [Column(Name = "HOSPITALCODEOUT")]
        public virtual string Hospitalcodeout { get; set; } = "";
        [Column(Name = "DEPARTMENTSCODEOUT")]
        public virtual string Departmentscodeout { get; set; } = "";
        [Column(Name = "TIMEOUT")]
        public  DateTime? Timeout { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        [Column(Name = "DIAGNOSIS")]
        public virtual string Diagnosis { get; set; } = "";
        [Column(Name = "HOSPITALINTO")]
        public virtual string Hospitalinto { get; set; } = "";
        [Column(Name = "DEPARTMENTSINTO")]
        public virtual string Departmentsinto { get; set; } = "";
        [Column(Name = "HOSPITALCODEINTO")]
        public virtual string Hospitalcodeinto { get; set; } = "";
        [Column(Name = "DEPARTMENTSCODEINTO")]
        public virtual string Departmentscodeinto { get; set; } = "";
        [Column(Name = "TIMEINTO")]
        public virtual DateTime? Timeinto { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        [Column(Name ="DOCTOR")]
        public virtual string Doctor { get; set; } = "";
        [Column(Name = "BACKDIAGNOSIS")]
        public virtual string BackDiagnosis { get; set; } = "";
        [Column(Name = "BACKDOCTOR")]
        public virtual string BackDoctor { get; set; } = "";
        [Column(Name = "PROPOSAL")]
        public virtual string Proposal { get; set; } = "";
        [Column(Name = "PATIENT")]
        public virtual string Patient { get; set; } = "";
        [Column(Name = "STATUS")]
        public virtual string Status { get; set; } = "";
        [Column(Name = "REASON")]
        public virtual string Reason { get; set; } = "";
        [Column(Name = "SIGN")]
        public virtual int Sign { get; set; } = 0;
        [Column(Name = "CARDNO")]
        public virtual string CardNo { get; set; } = "";
        [Column(Name ="INPATIENTNO")]
        public virtual string InpatientNo { get; set; } = "";
        [Column(Name = "ORIGINATOR")]
        public virtual string Originator { get; set; } = "";
        [Column(Name = "INDATE")]
        public virtual DateTime? INDATE { get; set; } = DateTime.Parse("0001-01-01 00:00:00");

        [Column(Name = "AUDIT")]
        public virtual string AUDIT { get; set; } = "";

        public Patientsinfo Patientsinfo = new Patientsinfo();
        //患者信息
        //private Referral() {
        //    Diagnosis = new List<Diagnosis>();
        //}
    }
}
