using DataAccessLayer;
using Logger;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public sealed class dt308_ReportContactBUS
    {
        public static dt308_ReportContactBUS Instance { get; } = new dt308_ReportContactBUS();
        private readonly TPLogger logger = new TPLogger(typeof(dt308_ReportContactBUS).FullName);

        private dt308_ReportContactBUS() { }

        public dt308_ReportContact GetByDepartment(string departmentId)
        {
            if (string.IsNullOrWhiteSpace(departmentId) || departmentId.Length != 4) return null;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    const string sql = @"SELECT DepartmentId, CompanyVN, CompanyTW,
                        IndustryVN, IndustryTW, AddressVN, AddressTW, Email, ContactVN, ContactTW
                        FROM dbo.dt308_ReportContact WHERE DepartmentId = @departmentId";
                    var parameter = new SqlParameter("@departmentId", SqlDbType.VarChar, 4)
                    {
                        Value = departmentId
                    };
                    return context.Database.SqlQuery<dt308_ReportContact>(sql, parameter).SingleOrDefault();
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
