using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt314_HskExamMgmtBUS
    {
        TPLogger logger;
        private static dt314_HskExamMgmtBUS instance;
        public static dt314_HskExamMgmtBUS Instance
        {
            get { if (instance == null) instance = new dt314_HskExamMgmtBUS(); return instance; }
            private set { instance = value; }
        }

        private dt314_HskExamMgmtBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt314_HskExamMgmt> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamMgmt.OrderByDescending(r => r.CreateTime).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt314_HskExamMgmt> GetListProcessing()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamMgmt.Where(r => r.StartTime != null && r.FinishTime == null).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt314_HskExamMgmt GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamMgmt.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt314_HskExamMgmt GetItemByCode(string code)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamMgmt.FirstOrDefault(r => r.Code == code);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt314_HskExamMgmt item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskExamMgmt.Add(item);
                    return _context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddOrUpdate(dt314_HskExamMgmt item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskExamMgmt.AddOrUpdate(item);
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
