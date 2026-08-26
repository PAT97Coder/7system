using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt314_HskQuestionsBUS
    {
        TPLogger logger;
        private static dt314_HskQuestionsBUS instance;
        public static dt314_HskQuestionsBUS Instance
        {
            get { if (instance == null) instance = new dt314_HskQuestionsBUS(); return instance; }
            private set { instance = value; }
        }

        private dt314_HskQuestionsBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt314_HskQuestions> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    _context.Configuration.LazyLoadingEnabled = false;
                    return _context.dt314_HskQuestions
                        .OrderByDescending(r => r.Id)
                        .ToList()
                        .Select(CloneQuestion)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt314_HskQuestions> GetActiveList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    _context.Configuration.LazyLoadingEnabled = false;
                    return _context.dt314_HskQuestions
                        .Where(r => r.IsActive)
                        .ToList()
                        .Select(CloneQuestion)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt314_HskQuestions GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    _context.Configuration.LazyLoadingEnabled = false;
                    var item = _context.dt314_HskQuestions.FirstOrDefault(r => r.Id == id);
                    return item == null ? null : CloneQuestion(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt314_HskQuestions item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskQuestions.Add(item);
                    return _context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt314_HskQuestions> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskQuestions.AddRange(items);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt314_HskQuestions item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskQuestions.AddOrUpdate(item);
                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        private static dt314_HskQuestions CloneQuestion(dt314_HskQuestions item)
        {
            return new dt314_HskQuestions()
            {
                Id = item.Id,
                LevelCode = item.LevelCode,
                SectionCode = item.SectionCode,
                QuestionType = item.QuestionType,
                DisplayText = item.DisplayText,
                ImageName = item.ImageName,
                IsMultiAns = item.IsMultiAns,
                IsActive = item.IsActive,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                UpdatedBy = item.UpdatedBy,
                UpdatedDate = item.UpdatedDate,
                Remark = item.Remark
            };
        }
    }
}
