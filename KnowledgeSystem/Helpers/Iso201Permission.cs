using BusinessLayer;
using System;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    internal static class Iso201Permission
    {
        public const string EditGroupName = "ISO文件【編輯】";

        public static bool CanEditCurrentUser()
        {
            try
            {
                var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
                var editGroups = dm_GroupBUS.Instance.GetListByName(EditGroupName);

                return editGroups.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool EnsureCanEditCurrentUser()
        {
            if (CanEditCurrentUser()) return true;

            MsgTP.MsgError($"您不屬於「{EditGroupName}」群組，無權編輯或刪除ISO文件！");
            return false;
        }
    }
}
