//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlarmMonitor.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Alarm_edit
    {
        public int ID { get; set; }
        public string Seq_number { get; set; }
        public Nullable<System.DateTime> Start_time { get; set; }
        public Nullable<System.DateTime> End_time { get; set; }
        public string Duration { get; set; }
        public string Reason { get; set; }
        public string Responsible { get; set; }
        public string Position { get; set; }
        public string Station { get; set; }
        public string Recorder { get; set; }
        public Nullable<bool> Delete_flag { get; set; }
        public string Alarm_class { get; set; }
        public string Alarm_area { get; set; }
        public Nullable<bool> Upload_flag { get; set; }
        public string alarm_id { get; set; }
        public Nullable<System.DateTime> generation_utc { get; set; }
        public string Division_of_respon { get; set; }
        public Nullable<bool> Update_flag { get; set; }
    }
}
