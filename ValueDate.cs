using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Project1_01._08._2022
{
    public class GeoData
    {
        public List<double> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Permission
    {
        public string PermissionNumber { get; set; }
        public string PermissionBeginDate { get; set; }
        public string PermissionEndDate { get; set; }
        public string PermissionStatus { get; set; }
        public List<object> PermissionFile { get; set; }
    }

    public class DeveloperInfo
    {
        public string OrgForm { get; set; }
        public string OrgName { get; set; }
    }

    public class ValueDate
    {
        public long global_id { get; set; }
        public int ID { get; set; }
        public string ObjectName { get; set; }
        public string AdmArea { get; set; }
        public string District { get; set; }
        public string ObjectAddress { get; set; }
        public string Coordinates { get; set; }
        public string ConsrtuctionWorkType { get; set; }
        public string MainFunctional { get; set; }
        public string SourceOfFinance { get; set; }
        public float DevelopmentArea { get; set; }
        public string FloorsAmount { get; set; }
        public string ObjectStatus { get; set; }
        public List<DeveloperInfo> DeveloperInfo { get; set; }
        public string CadastralNumber { get; set; }
        public string AreaCoordinates { get; set; }
        public string GPZUDocumentNumber { get; set; }
        public string GPZUDocumentDate { get; set; }
        public string GPZUDocumentStatus { get; set; }
        public List<Permission> Permission { get; set; }
        public string EntryState { get; set; }
        public string EntryAddReason { get; set; }
        public string EntryModifyReason { get; set; }
        public string FunctionalForNG { get; set; }
        public GeoData geoData { get; set; }
    }
}
