using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt312_QuestionsBUS
    {
        TPLogger logger;

        private static dt312_QuestionsBUS instance;

        public static dt312_QuestionsBUS Instance
        {
            get { if (instance == null) instance = new dt312_QuestionsBUS(); return instance; }
            private set { instance = value; }
        }

        private dt312_QuestionsBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public bool CanManageQuestions(string userId) => HasFunction(userId, "uc312_GroupMgmt");
        public bool CanPractice(string userId) => HasFunction(userId, "uc312_ExamMain") || CanManageQuestions(userId);
        private bool HasFunction(string userId, string controlName)
        {
            userId = (userId ?? "").Trim().ToUpperInvariant();
            if (userId.Length == 0) return false;
            using (var context = new DBDocumentManagementSystemEntities())
                return (from role in context.dm_UserRole
                        join grant in context.dm_FunctionRole on role.IdRole equals grant.IdRole
                        join function in context.dm_Function on grant.IdFunction equals function.Id
                        where role.IdUser == userId && (function.ControlName == controlName || function.ControlName.EndsWith("." + controlName))
                        select function.Id).Any();
        }
        public int SaveEssay(string userId, int groupId, int questionId, string question, string referenceAnswer)
        {
            if (!CanManageQuestions(userId)) throw new UnauthorizedAccessException("您沒有管理題庫的權限。");
            question = (question ?? "").Trim(); referenceAnswer = (referenceAnswer ?? "").Trim();
            if (question.Length == 0 || question.Length > 2048 || referenceAnswer.Length > 16000)
                throw new ArgumentException("題目為必填且不可超過2048字；參考答案不可超過16000字。");
            using (var context = new DBDocumentManagementSystemEntities())
            {
                if (!context.dt312_Groups.Any(g => g.Id == groupId && g.RemoveAt == null))
                    throw new InvalidOperationException("題目群組不存在或已停用。");
                var item = questionId == 0 ? new dt312_Questions { GroupId = groupId, IsEssay = true, IsmultiAns = false }
                    : context.dt312_Questions.FirstOrDefault(q => q.Id == questionId && q.GroupId == groupId && q.IsEssay);
                if (item == null) throw new InvalidOperationException("找不到此申論題。");
                item.DisplayName = question; item.ReferenceAnswer = referenceAnswer;
                if (questionId == 0) context.dt312_Questions.Add(item);
                context.SaveChanges();
                return item.Id;
            }
        }

        public List<dt312_Questions> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_Questions.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt312_Questions> GetListByIdGroup(int idGroup)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_Questions.Where(r => r.GroupId == idGroup).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt312_Questions GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_Questions.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt312_Questions item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Questions.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                    {
                        return item.Id;
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt312_Questions> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Questions.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt312_Questions item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Questions.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
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
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt312_Questions.FirstOrDefault(r => r.Id == id);
                    _context.dt312_Questions.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
