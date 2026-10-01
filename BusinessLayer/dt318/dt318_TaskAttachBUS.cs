using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TaskAttachBUS
    {
        private readonly TPLogger logger;
        private static dt318_TaskAttachBUS instance;

        public static dt318_TaskAttachBUS Instance
        {
            get { if (instance == null) instance = new dt318_TaskAttachBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TaskAttachBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt318_TaskAttach> GetListByTask(int idCongViec)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskAttach
                        .Where(r => r.IdCongViec == idCongViec)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskAttach> GetListByTask(int idCongViec, string loaiDinhKem)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskAttach
                        .Where(r => r.IdCongViec == idCongViec && r.LoaiDinhKem == loaiDinhKem)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskAttach> GetListByListTask(List<int> idsCongViec)
        {
            try
            {
                if (idsCongViec == null || idsCongViec.Count == 0)
                    return new List<dt318_TaskAttach>();

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskAttach
                        .Where(r => idsCongViec.Contains(r.IdCongViec))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskAttach GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskAttach.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt318_TaskAttach item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskAttach.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt318_TaskAttach> items)
        {
            try
            {
                if (items == null || items.Count == 0) return false;

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskAttach.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var item = context.dt318_TaskAttach.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;

                    context.dt318_TaskAttach.Remove(item);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveByIdAttach(int idDinhKem)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var items = context.dt318_TaskAttach
                        .Where(r => r.IdDinhKem == idDinhKem)
                        .ToList();
                    if (items.Count == 0) return false;

                    context.dt318_TaskAttach.RemoveRange(items);
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
