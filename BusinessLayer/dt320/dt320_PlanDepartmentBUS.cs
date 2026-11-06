using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt320_PlanDepartmentBUS
    {
        private readonly TPLogger logger;
        private static dt320_PlanDepartmentBUS instance;

        public static dt320_PlanDepartmentBUS Instance
        {
            get { if (instance == null) instance = new dt320_PlanDepartmentBUS(); return instance; }
            private set { instance = value; }
        }

        private dt320_PlanDepartmentBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt320_PlanDepartment> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanDepartment.Where(r => r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_PlanDepartment> GetListIsRemove()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanDepartment.Where(r => r.RemoveAt != null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_PlanDepartment> GetListByPlan(int idPlan)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanDepartment.Where(r => r.IdPlan == idPlan && r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_PlanDepartment> GetListByPlanAndType(int idPlan, byte participationType)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanDepartment.Where(r => r.IdPlan == idPlan && r.ParticipationType == participationType && r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt320_PlanDepartment GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanDepartment.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt320_PlanDepartment item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanDepartment.Add(item);
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

        public bool AddRange(List<dt320_PlanDepartment> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanDepartment.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt320_PlanDepartment item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanDepartment.AddOrUpdate(item);
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
                    var item = _context.dt320_PlanDepartment.FirstOrDefault(r => r.Id == id && r.RemoveAt == null);
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
                    var item = _context.dt320_PlanDepartment.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;
                    _context.dt320_PlanDepartment.Remove(item);
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
