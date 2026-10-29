using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TaskHistoryBUS
    {
        private readonly TPLogger logger;
        private static dt318_TaskHistoryBUS instance;

        public static dt318_TaskHistoryBUS Instance
        {
            get { if (instance == null) instance = new dt318_TaskHistoryBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TaskHistoryBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt318_TaskHistory> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskHistory
                        .OrderByDescending(r => r.NgayThucHien)
                        .ThenByDescending(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskHistory> GetListByTask(int idCongViec)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskHistory
                        .Where(r => r.IdCongViec == idCongViec)
                        .OrderByDescending(r => r.NgayThucHien)
                        .ThenByDescending(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskHistory> GetListByTaskTeam(int idTaskTeam)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskHistory
                        .Where(r => r.IdTaskTeam == idTaskTeam)
                        .OrderByDescending(r => r.NgayThucHien)
                        .ThenByDescending(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskHistory> GetListByWorkflowStep(int idWorkflowStep)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskHistory
                        .Where(r => r.IdWorkflowStep == idWorkflowStep)
                        .OrderByDescending(r => r.NgayThucHien)
                        .ThenByDescending(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskHistory GetItemById(long id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskHistory.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public long Add(dt318_TaskHistory item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskHistory.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt318_TaskHistory> items)
        {
            try
            {
                if (items == null || items.Count == 0) return false;

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskHistory.AddRange(items);
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
