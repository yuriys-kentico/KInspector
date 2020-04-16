using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Results
{
    public class CmsUserResult : CmsUser
    {
        public CmsUserResult(CmsUser user)
        {
            UserID = user.UserID;
            UserName = user.UserName;

            FullName = string.IsNullOrEmpty(user.FullName) ? $"{user.FirstName} {user.MiddleName} {user.LastName}" : user.FullName;

            Email = user.Email;
            UserPrivilegeLevel = user.UserPrivilegeLevel;
        }
    }
}