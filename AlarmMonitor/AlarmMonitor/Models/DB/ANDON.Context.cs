﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ANDONEntities : DbContext
    {
        public ANDONEntities()
            : base("name=ANDONEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ALARM_LOG> ALARM_LOG { get; set; }
        public virtual DbSet<DATA_LOG> DATA_LOG { get; set; }
        public virtual DbSet<EM_LOG> EM_LOG { get; set; }
        public virtual DbSet<EVENT_LOG> EVENT_LOG { get; set; }
        public virtual DbSet<alarm_record> alarm_record { get; set; }
        public virtual DbSet<alarm_record_upload> alarm_record_upload { get; set; }
        public virtual DbSet<dianchibao_signal> dianchibao_signal { get; set; }
        public virtual DbSet<luntaiku_signal> luntaiku_signal { get; set; }
        public virtual DbSet<station_info> station_info { get; set; }
        public virtual DbSet<AlarmClassView> AlarmClassView { get; set; }
        public virtual DbSet<user_log> user_log { get; set; }
        public virtual DbSet<Alarm_edit> Alarm_edit { get; set; }
        public virtual DbSet<Notice> Notice { get; set; }
    }
}
