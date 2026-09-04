using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt316_ReportBUS
    {
        private readonly TPLogger logger;
        private static dt316_ReportBUS instance;

        public static dt316_ReportBUS Instance
        {
            get { if (instance == null) instance = new dt316_ReportBUS(); return instance; }
            private set { instance = value; }
        }

        private dt316_ReportBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt316_Report> GetAll()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_Report.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_Report> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: chỉ lấy báo cáo chưa bị xóa mềm.
                    return context.dt316_Report
                        .Where(r => r.RemoveAt == null)
                        .OrderByDescending(r => r.CreateAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt316_Report GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_Report.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_Report> GetListByPlan(int idPlan)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lọc báo cáo đang hoạt động theo IdPlan.
                    return context.dt316_Report
                        .Where(r => r.RemoveAt == null && r.IdPlan == idPlan)
                        .OrderBy(r => r.IdDept)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_Report> GetListByDept(string idDept)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lọc báo cáo đang hoạt động theo phòng ban.
                    return context.dt316_Report
                        .Where(r => r.RemoveAt == null && r.IdDept == idDept)
                        .OrderByDescending(r => r.CreateAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt316_Report GetItemByPlanAndDept(int idPlan, string idDept)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: một plan + phòng ban dùng để tìm báo cáo tương ứng.
                    return context.dt316_Report.FirstOrDefault(r =>
                        r.RemoveAt == null && r.IdPlan == idPlan && r.IdDept == idDept);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt316_Report item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Report.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt316_Report> items)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Report.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt316_Report item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Report.AddOrUpdate(item);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveById(int id, string userRemove)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var item = context.dt316_Report.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;

                    item.RemoveAt = DateTime.Now;
                    item.RemoveBy = userRemove;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }
    }
}
