using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TaskUsersBUS
    {
        private readonly TPLogger logger;
        private static dt318_TaskUsersBUS instance;

        public static dt318_TaskUsersBUS Instance
        {
            get { if (instance == null) instance = new dt318_TaskUsersBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TaskUsersBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt318_TaskUsers> GetListByTask(int idCongViec)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskUsers
                        .Where(r => r.IdCongViec == idCongViec && r.NgayXoa == null)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskUsers> GetListByUser(string idNguoiDung)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskUsers
                        .Where(r => r.IdNguoiDung == idNguoiDung && r.NgayXoa == null)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskUsers GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskUsers.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt318_TaskUsers item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskUsers.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt318_TaskUsers> items)
        {
            try
            {
                if (items == null || items.Count == 0) return false;

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskUsers.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt318_TaskUsers item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskUsers.AddOrUpdate(item);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Xoá mềm một người khỏi công việc.
        /// </summary>
        public bool RemoveById(int id, string idNguoiDungThucHien)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var item = context.dt318_TaskUsers.FirstOrDefault(r => r.Id == id);
                    if (item == null || item.NgayXoa != null) return false;

                    item.NgayXoa = DateTime.Now;
                    item.NguoiXoa = idNguoiDungThucHien;
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
