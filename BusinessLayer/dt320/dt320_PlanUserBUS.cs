using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt320_PlanUserBUS
    {
        private readonly TPLogger logger;
        private static dt320_PlanUserBUS instance;

        public static dt320_PlanUserBUS Instance
        {
            get { if (instance == null) instance = new dt320_PlanUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dt320_PlanUserBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt320_PlanUser> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanUser.Where(r => r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_PlanUser> GetListIsRemove()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanUser.Where(r => r.RemoveAt != null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt320_PlanUser> GetListByPlanDepartment(int idPlanDepartment)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanUser.Where(r => r.IdPlanDepartment == idPlanDepartment && r.RemoveAt == null).OrderBy(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt320_PlanUser GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt320_PlanUser.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt320_PlanUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanUser.Add(item);
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

        public bool AddRange(List<dt320_PlanUser> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanUser.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt320_PlanUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt320_PlanUser.AddOrUpdate(item);
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
                    var item = _context.dt320_PlanUser.FirstOrDefault(r => r.Id == id && r.RemoveAt == null);
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
                    var item = _context.dt320_PlanUser.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;
                    _context.dt320_PlanUser.Remove(item);
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
