using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt314_HskExamUserBUS
    {
        TPLogger logger;
        private static dt314_HskExamUserBUS instance;
        public static dt314_HskExamUserBUS Instance
        {
            get { if (instance == null) instance = new dt314_HskExamUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dt314_HskExamUserBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt314_HskExamUser> GetListByExamCode(string examCode)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamUser.Where(r => r.ExamCode == examCode).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt314_HskExamUser> GetListByExamCodes(List<string> examCodes)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamUser.Where(r => examCodes.Contains(r.ExamCode)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int CountByExamCodeAndUserId(string examCode, string userId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamUser.Count(r => r.ExamCode == examCode && r.IdUser == userId);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt314_HskExamUser> GetListNeedDoByUserId(string userId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamUser.Where(r => r.IdUser == userId && r.SubmitTime == null).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt314_HskExamUser GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamUser.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddRange(List<dt314_HskExamUser> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskExamUser.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt314_HskExamUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskExamUser.AddOrUpdate(item);
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
