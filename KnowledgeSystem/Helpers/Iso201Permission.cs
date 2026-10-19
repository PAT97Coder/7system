using BusinessLayer;
using System;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    internal static class Iso201Permission
    {
        public const string EditGroupName = "ISO文件【編輯及刪除】";
        public const string EditOnlyGroupName = "ISO文件【僅編輯】";

        public static bool CanEditCurrentUser()
        {
            return IsCurrentUserInGroups(EditGroupName, EditOnlyGroupName);
        }

        public static bool CanDeleteCurrentUser()
        {
            return IsCurrentUserInGroups(EditGroupName);
        }

        private static bool IsCurrentUserInGroups(params string[] groupNames)
        {
            try
            {
                var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
                var editGroups = groupNames.SelectMany(name => dm_GroupBUS.Instance.GetListByName(name));

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

            MsgTP.MsgError($"您不屬於「{EditGroupName}」或「{EditOnlyGroupName}」群組，無權編輯ISO文件！");
            return false;
        }

        public static bool EnsureCanDeleteCurrentUser()
        {
            if (CanDeleteCurrentUser()) return true;

            MsgTP.MsgError($"您不屬於「{EditGroupName}」群組，無權刪除ISO文件！");
            return false;
        }
    }
}
