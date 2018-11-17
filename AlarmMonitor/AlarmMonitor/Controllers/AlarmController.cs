using AlarmMonitor.Common;
using AlarmMonitor.Extension;
using AlarmMonitor.Models.DB;
using AlarmMonitor.Models.Export;
using AlarmMonitor.Models.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlarmMonitor.Controllers
{
    [UserPermission]
    public class AlarmController : BaseController
    {
        // GET: Alarm
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AlarmEdit()
        {
            return View();
            //return new JsonResult { Data = null, JsonRequestBehavior=JsonRequestBehavior.AllowGet };
        }
        public ActionResult AlarmDetail()
        {
            return View();
        }
        public ActionResult AlarmClassReport()
        {
            return View();
        }
        public ActionResult AlarmTimeReport()
        {
            return View();
        }
        public ActionResult GetAlarmEditList(FilterForAlarmEdit filter)
        {
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false); ;
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.Start_time <= endTime);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_class) && !(filter.Alarm_class == "全部") && !(filter.Alarm_class == "非拉动停线"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_class == filter.Alarm_class);
                }
                if (filter.Alarm_class == "非拉动停线")
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_class != "拉动停线");
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                
                int totalCount = alarm_edits.Count();
                alarm_edits = alarm_edits.OrderByDescending(t => t.Start_time).Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);
                var result = from a in alarm_edits
                             select new ALarmEditViewModel
                             {
                                 ID = a.ID,
                                 Start_time = a.Start_time,
                                 End_time = a.End_time,
                                 Station = a.Station,
                                 Alarm_area = a.Alarm_area,
                                 Alarm_class = a.Alarm_class,
                                 Division_of_respon = a.Division_of_respon,
                                 Position = a.Position,
                                 Reason = a.Reason,
                                 Responsible = a.Responsible,
                                 Duration = a.Duration
                             };
                ;
                return Json(new
                {
                    iTotalDisplayRecords = totalCount,
                    aaData = result.ToList()
                }
                 , JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult AlarmEditExoprt(FilterForAlarmEdit filter)
        {
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false); ;
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.Start_time <= endTime);
                }
                //var s = alarm_edits.ToList();
                if (!String.IsNullOrEmpty(filter.Alarm_class) && !(filter.Alarm_class == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_class == filter.Alarm_class);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                alarm_edits = alarm_edits.OrderByDescending(t => t.Start_time);
                var result = (from a in alarm_edits
                             select new ALarmEditViewModel
                             {
                                 ID = a.ID,
                                 Start_time = a.Start_time,
                                 End_time = a.End_time,
                                 Station = a.Station,
                                 Alarm_area = a.Alarm_area,
                                 Alarm_class = a.Alarm_class,
                                 Division_of_respon = a.Division_of_respon,
                                 Position = a.Position,
                                 Reason = a.Reason,
                                 Responsible = a.Responsible,
                                 Duration = a.Duration
                             }).ToList();
                List<AlarmEidtExport> exportList = (from r in result
                                                    select new AlarmEidtExport
                                                    {
                                                        开始时间=r.Start_time,
                                                        结束时间=r.End_time,
                                                        持续时间=r.DurationStr,
                                                        呼叫类型=r.Alarm_class,
                                                        停线工位=r.Station,
                                                        停线区域=r.Alarm_area,
                                                        停线原因=r.Reason,
                                                        位置信息=r.Position,
                                                        责任部门=r.Responsible,
                                                        责任类型=r.Division_of_respon
                                                    }).ToList();
                DataTable dt = Untils.ToDataTable<AlarmEidtExport>(exportList);
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet  
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                //row1.RowStyle.FillBackgroundColor = "";  
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                }
                //将数据逐步写入sheet1各个行  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString().Trim());
                    }
                }
                string strdate = DateTime.Now.ToString("yyyyMMddhhmmss");//获取当前时间  
                // 写入到客户端   
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel", "AlarmDetail" + strdate + "Excel.xls");  
            }
        }

        public ActionResult GetAlarmDetailList(FilterForAlarmDetail filter)
        {
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false);
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.Start_time <= endTime);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                if (!String.IsNullOrEmpty(filter.Division_of_respon) && !(filter.Division_of_respon == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Division_of_respon == filter.Division_of_respon);
                }
                if (!String.IsNullOrEmpty(filter.Responsible) && !(filter.Responsible == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Responsible == filter.Responsible);
                }
                if (!String.IsNullOrEmpty(filter.Recorder))
                {
                    alarm_edits = alarm_edits.Where(t => t.Recorder.Contains(filter.Recorder));
                }

                int totalCount = alarm_edits.Count();
                alarm_edits = alarm_edits.OrderByDescending(t => t.Start_time).Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);
                var result = from a in alarm_edits
                             select new AlarmDetailViewModel
                             {
                                 Start_time = a.Start_time,
                                 End_time = a.End_time,
                                 Station = a.Station,
                                 Reason = a.Reason,
                                 Responsible = a.Responsible,
                                 Duration = a.Duration,
                                 Division_of_respon = a.Division_of_respon,
                                 Position = a.Position,
                                 Recorder = a.Recorder,
                                 Alarm_class = a.Alarm_class
                             };
                return Json(new
                {
                    iTotalDisplayRecords = totalCount,
                    aaData = result.ToList()
                }
                 , JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AlarmDetailExoprt(FilterForAlarmDetail filter)
        {
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false);
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.Start_time <= endTime);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                if (!String.IsNullOrEmpty(filter.Division_of_respon) && !(filter.Division_of_respon == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Division_of_respon == filter.Division_of_respon);
                }
                if (!String.IsNullOrEmpty(filter.Responsible) && !(filter.Responsible == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Responsible == filter.Responsible);
                }
                if (!String.IsNullOrEmpty(filter.Recorder))
                {
                    alarm_edits = alarm_edits.Where(t => t.Recorder.Contains(filter.Recorder));
                }

                alarm_edits = alarm_edits.OrderByDescending(t => t.Start_time);
                var result = (from a in alarm_edits
                             select new AlarmDetailViewModel
                             {
                                 Start_time = a.Start_time,
                                 End_time = a.End_time,
                                 Station = a.Station,
                                 Reason = a.Reason,
                                 Responsible = a.Responsible,
                                 Duration = a.Duration,
                                 Division_of_respon = a.Division_of_respon,
                                 Position = a.Position,
                                 Recorder = a.Recorder,
                                 Alarm_class = a.Alarm_class
                             }).ToList();
                List<AlarmDetailExport> exportList = (from r in result
                                                    select new AlarmDetailExport
                                                    {
                                                        开始时间 = r.Start_time,
                                                        结束时间 = r.End_time,
                                                        持续时间 = r.DurationStr,
                                                        呼叫类型 = r.Alarm_class,
                                                        停线工位 = r.Station,
                                                        编辑人 = r.Recorder,
                                                        停线原因 = r.Reason,
                                                        位置信息 = r.Position,
                                                        责任部门 = r.Responsible,
                                                        责任类型 = r.Division_of_respon
                                                    }).ToList();
                DataTable dt = Untils.ToDataTable<AlarmDetailExport>(exportList);
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet  
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
                //给sheet1添加第一行的头部标题  
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                //row1.RowStyle.FillBackgroundColor = "";  
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                }
                //将数据逐步写入sheet1各个行  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        rowtemp.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString().Trim());
                    }
                }
                string strdate = DateTime.Now.ToString("yyyyMMddhhmmss");//获取当前时间  
                // 写入到客户端   
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel", "AlarmDetail"+strdate + "Excel.xls");  
            }
        }


        public ActionResult AlarmEditSaveChange(ALarmEditViewModel model)
        {
            using (ANDONEntities entities = new ANDONEntities())
            {
                bool result = false;
                try
                {
                    Alarm_edit alarm = entities.Alarm_edit.Where(t => t.ID == model.ID).FirstOrDefault();
                    alarm.Start_time = model.Start_time;
                    alarm.End_time = model.End_time;
                    alarm.Reason = model.Reason;
                    alarm.Responsible = model.Responsible;
                    alarm.Division_of_respon = model.Division_of_respon;
                    UserModel user = GetCurrentUser();
                    alarm.Recorder = user.UName + "(" + user.UID + ")";
                    alarm.Alarm_class = model.Alarm_class;
                    alarm.Position = model.Position;
                    alarm.Station = model.Station;

                    DateTime startTime, endTime;
                    if (model.Start_time.HasValue && model.End_time.HasValue)
                    {
                        startTime = model.Start_time.Value;
                        endTime = model.End_time.Value;
                        TimeSpan ts = endTime - startTime;
                        int seconds = (int)ts.TotalSeconds;
                        alarm.Duration = seconds.ToString();
                    }

                    alarm.Update_flag = true;
                    entities.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {

                }

                return Json(result);
            }
        }
        public ActionResult DeleteAlarmEidt(int ID)
        {
            bool result = false;
            using (ANDONEntities entities = new ANDONEntities())
            {
                Alarm_edit a = entities.Alarm_edit.Find(ID);
                a.Delete_flag = true;
                entities.SaveChanges();
                result = true;
            }
            return Json(result);
        }

        public ActionResult GetAlarmClassData(FilterForAlarmDetail filter)
        {
            List<string> BarLabels = new List<string>();
            List<double> BarData = new List<double>();
            List<PieChartModel> PieData = new List<PieChartModel>();
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false);
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.End_time <= endTime);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                if (!String.IsNullOrEmpty(filter.Division_of_respon) && !(filter.Division_of_respon == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Division_of_respon == filter.Division_of_respon);
                }
                if (!String.IsNullOrEmpty(filter.Responsible) && !(filter.Responsible == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Responsible == filter.Responsible);
                }
                if (!String.IsNullOrEmpty(filter.Recorder))
                {
                    alarm_edits = alarm_edits.Where(t => t.Recorder.Contains(filter.Recorder));
                }

                int totalCount = alarm_edits.Count();

                var responsibleGroup = alarm_edits.GroupBy(t => t.Responsible).OrderByDescending(t => t.Count());
                int counter = 0;
                //double totalRatio = 0;
                int barOtherCount = 0;
                foreach (IGrouping<string, Alarm_edit> item in responsibleGroup)
                {
                    if (counter == 9)
                    {
                        break;
                    }
                    int count = item.Count();
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        BarLabels.Add("空");
                    }
                    else
                    {
                        BarLabels.Add(item.Key);
                    }
                    //double ratio = ((double)a / (double)totalCount) * 100;
                    //totalRatio += ratio;
                    //ratio=Math.Round(ratio, 2);
                    barOtherCount = totalCount - barOtherCount;
                    BarData.Add(count);
                    counter++;
                }
                BarLabels.Add("其他");
                //BarData.Add(Math.Round(100 - totalRatio, 2));
                BarData.Add(totalCount - barOtherCount);

                counter = 0;
                int otherCount = 0;
                var DGroup = alarm_edits.GroupBy(t => t.Division_of_respon).Select(t => (new { name = t.Key, count = t.Count() })).OrderByDescending(t => t.count);
                foreach (var item in DGroup)
                {
                    if (counter == 9)
                    {
                        break;
                    }
                    string name = "";
                    if (string.IsNullOrEmpty(item.name))
                    {
                        name = "空";
                    }
                    else
                    {
                        name = item.name;
                    }
                    PieChartModel model = new PieChartModel { name = name, value = item.count };
                    PieData.Add(model);
                    otherCount += item.count;
                    counter++;
                }
                PieChartModel pModel = new PieChartModel { name = "其他", value = totalCount - otherCount };
                PieData.Add(pModel);
            }

            return Json(new
            {
                BarLabels = BarLabels,
                BarData = BarData,
                PieData = PieData
            }
, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlarmTimeData(FilterForAlarmDetail filter)
        {
            List<string> BarLabels = new List<string>();
            List<double> BarData = new List<double>();
            int totalCount = 0;
            using (ANDONEntities entities = new ANDONEntities())
            {
                var alarm_edits = entities.Alarm_edit.Where(t => t.Delete_flag == false);
                if (filter.Start_time != null && filter.End_time != null)
                {
                    DateTime startTime = Convert.ToDateTime(filter.Start_time);
                    DateTime endTime = Convert.ToDateTime(filter.End_time);
                    alarm_edits = alarm_edits.Where(t => t.Start_time >= startTime && t.End_time <= endTime);
                }
                if (!String.IsNullOrEmpty(filter.Alarm_area) && !(filter.Alarm_area == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Alarm_area == filter.Alarm_area);
                }
                if (!String.IsNullOrEmpty(filter.Division_of_respon) && !(filter.Division_of_respon == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Division_of_respon == filter.Division_of_respon);
                }
                if (!String.IsNullOrEmpty(filter.Responsible) && !(filter.Responsible == "全部"))
                {
                    alarm_edits = alarm_edits.Where(t => t.Responsible == filter.Responsible);
                }
                if (!String.IsNullOrEmpty(filter.Recorder))
                {
                    alarm_edits = alarm_edits.Where(t => t.Recorder.Contains(filter.Recorder));
                }

                //totalCount = alarm_edits.Count();


                var Group = alarm_edits.GroupBy(t => t.Reason);
                List<TimeReportReuslt> timeList = new List<TimeReportReuslt>();
                //double totalRatio = 0;
                foreach (IGrouping<string, Alarm_edit> item in Group)
                {
                    TimeReportReuslt model = new TimeReportReuslt();
                    if (string.IsNullOrEmpty(item.Key))
                    {
                        model.Name = "空";
                        //BarLabels.Add("空");
                    }
                    else
                    {
                        model.Name = item.Key;
                        //BarLabels.Add(item.Key);
                    }
                    int timeCount = 0;
                    foreach (Alarm_edit e in item)
                    {
                        int seconds = string.IsNullOrEmpty(e.Duration) ? 0 : Convert.ToInt32(e.Duration);
                        timeCount += seconds;
                    }
                    totalCount += timeCount;
                    model.Secondes = timeCount;
                    //BarData.Add(timeCount);
                    timeList.Add(model);
                }
                foreach (var item in timeList.OrderByDescending(t => t.Secondes).Take(10))
                {
                    BarLabels.Add(item.Name);
                    BarData.Add(item.Secondes);
                }
                //BarLabels.Add("其他");
                //BarData.Add(Math.Round(100 - totalRatio, 2));
            }
            return Json(new
            {
                totalCount = totalCount,
                BarLabels = BarLabels,
                BarData = BarData,
            }
, JsonRequestBehavior.AllowGet);
        }
    }
}