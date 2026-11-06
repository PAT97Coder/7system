using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt320_ReportBUS
    {
        private readonly TPLogger logger;
        private static dt320_ReportBUS instance;

        public static dt320_ReportBUS Instance
        {
            get { if (instance == null) instance = new dt320_ReportBUS(); return instance; }
            private set { instance = value; }
        }

        private dt320_ReportBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt320_Report> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_Report.Where(r => r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_Report> GetListIsRemove()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_Report.Where(r => r.RemoveAt != null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_Report> GetListByPlanDepartment(int idPlanDepartment)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_Report.Where(r => r.IdPlanDepartment == idPlanDepartment && r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt320_Report GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_Report.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt320_Report item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_Report.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt320_Report> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_Report.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt320_Report item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_Report.AddOrUpdate(item);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        // Marks only this record as removed. Related records are handled by the caller.

        public bool SoftRemoveById(int id, string removeBy)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var item = _context.dt320_Report.FirstOrDefault(r => r.Id == id && r.RemoveAt == null);
                    if (item == null) return false;
                    item.RemoveAt = DateTime.Now;
                    item.RemoveBy = removeBy;
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        // Physical deletion, matching RemoveById in the existing BUS classes.

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var item = _context.dt320_Report.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;
                    _context.dt320_Report.Remove(item);
                    return _context.SaveChanges() > 0;
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
