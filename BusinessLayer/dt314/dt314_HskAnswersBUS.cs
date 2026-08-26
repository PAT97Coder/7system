using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt314_HskAnswersBUS
    {
        TPLogger logger;
        private static dt314_HskAnswersBUS instance;
        public static dt314_HskAnswersBUS Instance
        {
            get { if (instance == null) instance = new dt314_HskAnswersBUS(); return instance; }
            private set { instance = value; }
        }

        private dt314_HskAnswersBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt314_HskAnswers> GetListByQues(int idQues)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    _context.Configuration.LazyLoadingEnabled = false;
                    return _context.dt314_HskAnswers
                        .Where(r => r.QuesId == idQues && r.IsActive)
                        .OrderBy(r => r.DisplayOrder)
                        .ToList()
                        .Select(CloneAnswer)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt314_HskAnswers> GetListByListQues(List<int> idsQue)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    _context.Configuration.LazyLoadingEnabled = false;
                    return _context.dt314_HskAnswers
                        .Where(r => idsQue.Contains(r.QuesId) && r.IsActive)
                        .OrderBy(r => r.QuesId)
                        .ThenBy(r => r.DisplayOrder)
                        .ToList()
                        .Select(CloneAnswer)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddRange(List<dt314_HskAnswers> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskAnswers.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt314_HskAnswers item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskAnswers.AddOrUpdate(item);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        private static dt314_HskAnswers CloneAnswer(dt314_HskAnswers item)
        {
            return new dt314_HskAnswers()
            {
                Id = item.Id,
                QuesId = item.QuesId,
                DisplayText = item.DisplayText,
                ImageName = item.ImageName,
                TrueAns = item.TrueAns,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            };
        }
    }
}
