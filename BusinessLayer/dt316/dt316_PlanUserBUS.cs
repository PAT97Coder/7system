using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt316_PlanUserBUS
    {
        private readonly TPLogger logger;
        private static dt316_PlanUserBUS instance;

        public static dt316_PlanUserBUS Instance
        {
            get { if (instance == null) instance = new dt316_PlanUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dt316_PlanUserBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt316_PlanUser> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_PlanUser.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt316_PlanUser GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_PlanUser.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_PlanUser> GetListByPlan(int idPlan)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lấy toàn bộ người dùng được phân vào một kế hoạch.
                    return context.dt316_PlanUser
                        .Where(r => r.IdPlan == idPlan)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_PlanUser> GetListByUser(string idUser)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lấy các phân công kế hoạch của một người dùng.
                    return context.dt316_PlanUser
                        .Where(r => r.IdUser == idUser)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_PlanUser> GetListByDept(string idDept)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lấy phân công theo đơn vị/phòng ban.
                    return context.dt316_PlanUser
                        .Where(r => r.IdDept == idDept)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt316_PlanUser item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_PlanUser.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt316_PlanUser> items)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_PlanUser.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt316_PlanUser item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_PlanUser.AddOrUpdate(item);
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
                    // Bảng mapping không có RemoveAt nên thao tác này xóa vật lý.
                    var item = context.dt316_PlanUser.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;

                    context.dt316_PlanUser.Remove(item);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveByPlan(int idPlan)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: xóa toàn bộ mapping người dùng của plan được chọn.
                    var items = context.dt316_PlanUser
                        .Where(r => r.IdPlan == idPlan)
                        .ToList();

                    if (items.Count == 0) return false;
                    context.dt316_PlanUser.RemoveRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool ReplaceByPlan(int idPlan, List<dt316_PlanUser> items)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var transaction = context.Database.BeginTransaction())
                {
                    // CSDL/LINQ: thay toàn bộ người tham gia của một kế hoạch trong
                    // cùng transaction để không bị mất dữ liệu nếu bước thêm thất bại.
                    var oldItems = context.dt316_PlanUser
                        .Where(r => r.IdPlan == idPlan)
                        .ToList();
                    context.dt316_PlanUser.RemoveRange(oldItems);

                    if (items != null && items.Count > 0)
                        context.dt316_PlanUser.AddRange(items);

                    context.SaveChanges();
                    transaction.Commit();
                    return true;
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
