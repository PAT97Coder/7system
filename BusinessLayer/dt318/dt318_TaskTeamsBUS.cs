using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt318_TaskTeamsBUS
    {
        private readonly TPLogger logger;
        private static dt318_TaskTeamsBUS instance;

        public static dt318_TaskTeamsBUS Instance
        {
            get { if (instance == null) instance = new dt318_TaskTeamsBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TaskTeamsBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<dt318_TaskTeams> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams
                        .OrderBy(r => r.IdCongViec)
                        .ThenBy(r => r.MaTo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskTeams> GetListByTask(int idCongViec)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams
                        .Where(r => r.IdCongViec == idCongViec)
                        .OrderBy(r => r.MaTo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskTeams> GetListByTeam(string maTo)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams
                        .Where(r => r.MaTo == maTo)
                        .OrderBy(r => r.IdCongViec)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_TaskTeams> GetListByListTask(List<int> idsCongViec)
        {
            try
            {
                if (idsCongViec == null || idsCongViec.Count == 0)
                    return new List<dt318_TaskTeams>();

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams
                        .Where(r => idsCongViec.Contains(r.IdCongViec))
                        .OrderBy(r => r.IdCongViec)
                        .ThenBy(r => r.MaTo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskTeams GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_TaskTeams GetItemByTaskAndTeam(int idCongViec, string maTo)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_TaskTeams
                        .FirstOrDefault(r => r.IdCongViec == idCongViec && r.MaTo == maTo);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt318_TaskTeams item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskTeams.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt318_TaskTeams> items)
        {
            try
            {
                if (items == null || items.Count == 0) return false;

                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskTeams.AddRange(items);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt318_TaskTeams item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_TaskTeams.AddOrUpdate(item);
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
