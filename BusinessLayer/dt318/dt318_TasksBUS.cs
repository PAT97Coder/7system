using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TasksBUS
    {
        private readonly TPLogger logger;
        private static dt318_TasksBUS instance;

        public static dt318_TasksBUS Instance
        {
            get { if (instance == null) instance = new dt318_TasksBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TasksBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        /// <summary>
        /// Lấy các công việc chưa bị xoá, ưu tiên công việc gần đến hạn.
        /// </summary>
        public List<dt318_Tasks> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa == null)
                        .OrderBy(r => r.NgayDenHan)
                        .ThenByDescending(r => r.NgayTao)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_Tasks> GetListIsRemove()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa != null)
                        .OrderByDescending(r => r.NgayXoa)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_Tasks GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Dùng cho dịch vụ OCR lấy các công việc đang chờ nhận dạng.
        /// </summary>
        public List<dt318_Tasks> GetListByOcrStatus(string trangThaiOcr)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa == null && r.TrangThaiOCR == trangThaiOcr)
                        .OrderBy(r => r.NgayTao)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Thêm công việc và trả về Id vừa tạo.
        /// </summary>
        public int Add(dt318_Tasks item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_Tasks.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddOrUpdate(dt318_Tasks item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_Tasks.AddOrUpdate(item);
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
        /// Xoá mềm công việc để giữ lại lịch sử dữ liệu.
        /// </summary>
        public bool RemoveById(int id, string idNguoiDung)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var item = context.dt318_Tasks.FirstOrDefault(r => r.Id == id);
                    if (item == null || item.NgayXoa != null) return false;

                    item.NgayXoa = DateTime.Now;
                    item.NguoiXoa = idNguoiDung;
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
