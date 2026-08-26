using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt314_HskExamQuestionBUS
    {
        TPLogger logger;
        private static dt314_HskExamQuestionBUS instance;
        public static dt314_HskExamQuestionBUS Instance
        {
            get { if (instance == null) instance = new dt314_HskExamQuestionBUS(); return instance; }
            private set { instance = value; }
        }

        private dt314_HskExamQuestionBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt314_HskExamQuestion> GetListByExamCode(string examCode)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                    return _context.dt314_HskExamQuestion.Where(r => r.ExamCode == examCode).OrderBy(r => r.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddRange(List<dt314_HskExamQuestion> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt314_HskExamQuestion.AddRange(items);
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
