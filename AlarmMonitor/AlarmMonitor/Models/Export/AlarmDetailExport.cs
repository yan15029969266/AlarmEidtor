using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlarmMonitor.Models.Export
{
    public class AlarmDetailExport
    {
        public string 责任类型 { get; set; }
        public string 停线工位 { get; set; }
        public string 位置信息 { get; set; }
        public string 呼叫类型 { get; set; }
        public string 责任部门 { get; set; }
        public string 停线原因 { get; set; }
        public DateTime? 开始时间 { get; set; }
        public DateTime? 结束时间 { get; set; }
        public string 持续时间 { get; set; }
        public string 编辑人 { get; set; }
    }
}