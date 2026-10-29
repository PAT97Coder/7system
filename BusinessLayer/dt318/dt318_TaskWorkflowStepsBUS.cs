using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TaskWorkflowStepsBUS
    {
        private readonly TPLogger logger;
        private static dt318_TaskWorkflowStepsBUS instance;

        public static dt318_TaskWorkflowStepsBUS Instance
        {
            get { if (instance == null) instance = new dt318_TaskWorkflowStepsBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TaskWorkflowStepsBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt318_TaskWorkflowSteps> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps
                        .OrderBy(r => r.IdTaskTeam)
                        .ThenBy(r => r.ThuTu)
                        .ThenBy(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskWorkflowSteps> GetListByTaskTeam(int idTaskTeam)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps
                        .Where(r => r.IdTaskTeam == idTaskTeam)
                        .OrderBy(r => r.ThuTu)
                        .ThenBy(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskWorkflowSteps> GetListByTaskTeam(int idTaskTeam, string trangThai)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps
                        .Where(r => r.IdTaskTeam == idTaskTeam && r.TrangThai == trangThai)
                        .OrderBy(r => r.ThuTu)
                        .ThenBy(r => r.Id)
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
        /// Lấy lưu trình của Án bằng LINQ join thông qua thông tin tổ.
        /// </summary>
        public List<dt318_TaskWorkflowSteps> GetListByTask(int idCongViec)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return (from step in context.dt318_TaskWorkflowSteps
                            join team in context.dt318_TaskTeams on step.IdTaskTeam equals team.Id
                            where team.IdCongViec == idCongViec
                            orderby team.MaTo, step.ThuTu, step.Id
                            select step).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskWorkflowSteps> GetListByUser(string idNguoiXuLy)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps
                        .Where(r => r.IdNguoiXuLy == idNguoiXuLy)
                        .OrderBy(r => r.IdTaskTeam)
                        .ThenBy(r => r.ThuTu)
                        .ThenBy(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskWorkflowSteps> GetListByGroup(int idNhomXuLy)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps
                        .Where(r => r.IdNhomXuLy == idNhomXuLy)
                        .OrderBy(r => r.IdTaskTeam)
                        .ThenBy(r => r.ThuTu)
                        .ThenBy(r => r.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskWorkflowSteps GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskWorkflowSteps.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt318_TaskWorkflowSteps item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskWorkflowSteps.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt318_TaskWorkflowSteps> items)
        {
            try
            {
                if (items == null || items.Count == 0) return false;

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskWorkflowSteps.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt318_TaskWorkflowSteps item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskWorkflowSteps.AddOrUpdate(item);
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
