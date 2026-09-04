using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt316_PlanBUS
    {
        private readonly TPLogger logger;
        private static dt316_PlanBUS instance;

        public static dt316_PlanBUS Instance
        {
            get { if (instance == null) instance = new dt316_PlanBUS(); return instance; }
            private set { instance = value; }
        }

        private dt316_PlanBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt316_Plan> GetAll()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_Plan.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt316_Plan> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: lấy các kế hoạch chưa bị xóa mềm, mới nhất hiển thị trước.
                    return context.dt316_Plan
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

        public dt316_Plan GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt316_Plan.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool IsDisplayNameExists(string displayName, int? excludeId = null)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    // CSDL/LINQ: kiểm tra trùng tên trong các plan đang hoạt động;
                    // excludeId dùng khi sửa để bỏ qua chính bản ghi hiện tại.
                    return context.dt316_Plan.Any(r =>
                        r.RemoveAt == null &&
                        r.DisplayName == displayName &&
                        (!excludeId.HasValue || r.Id != excludeId.Value));
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt316_Plan item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Plan.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt316_Plan> items)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Plan.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt316_Plan item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt316_Plan.AddOrUpdate(item);
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
                    var item = context.dt316_Plan.FirstOrDefault(r => r.Id == id);
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
