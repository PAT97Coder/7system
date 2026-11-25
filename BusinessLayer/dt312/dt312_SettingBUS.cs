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
    public class dt312_SettingBUS
    {
        TPLogger logger;

        private static dt312_SettingBUS instance;

        public static dt312_SettingBUS Instance
        {
            get { if (instance == null) instance = new dt312_SettingBUS(); return instance; }
            private set { instance = value; }
        }

        private dt312_SettingBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public static int ChoiceCount(int total, int choicePercent)
        {
            if (total <= 0) throw new ArgumentException("題目數量必須大於0。");
            if (choicePercent < 0 || choicePercent > 100) throw new ArgumentException("選擇題比例必須介於0至100%。");
            return (int)Math.Round(total * (decimal)choicePercent / 100m, MidpointRounding.AwayFromZero);
        }

        private static void Validate(dt312_Setting item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            ChoiceCount(item.QuesCount, item.ChoicePercent);
            if (item.ChoiceScorePercent.HasValue && (item.ChoiceScorePercent < 0 || item.ChoiceScorePercent > 100))
                throw new ArgumentException("分數權重必須介於0至100%。");
            if (item.TestDuration <= 0 || item.CountEmp <= 0 || item.PassingScore < 0 || item.PassingScore > 100)
                throw new ArgumentException("考試時間與參與人數必須大於0；及格分數必須介於0至100。");
        }

        public List<dt312_Setting> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_Setting.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt312_Setting GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_Setting.FirstOrDefault(r => r.id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt312_Setting item)
        {
            Validate(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Setting.Add(item);
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

        public bool AddRange(List<dt312_Setting> items)
        {
            foreach (var item in items) Validate(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Setting.AddRange(items);
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

        public bool AddOrUpdate(dt312_Setting item)
        {
            Validate(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_Setting.AddOrUpdate(item);
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
                    var itemRemove = _context.dt312_Setting.FirstOrDefault(r => r.id == id);
                    _context.dt312_Setting.Remove(itemRemove);

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
