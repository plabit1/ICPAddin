using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;
using System.Security;
using System.Security.AccessControl;

namespace ICP.SP.Intranet.SyncPerms
{
    class Program
    {
        static void Main(string[] args)
        {
            string webUrl = "https://icpower.sharepoint.com/sites/Kallpa Generacion S.A_TI/";//Console.ReadLine();
            string userName = "sharepointadm@icpower.onmicrosoft.com";// Console.ReadLine();
            SecureString password = new SecureString();
            foreach (char c in "SharePoint2017".ToCharArray()) password.AppendChar(c);
            using (var context = new ClientContext(webUrl))
            {
                context.Credentials = new SharePointOnlineCredentials(userName, password);
                
                var list = context.Web.Lists.GetByTitle("ITDocs");
                // Create a query to get all items
                var camlQuery = CamlQuery.CreateAllFoldersQuery();
                var listItems = list.GetItems(camlQuery);
                context.Load(listItems, li => li.Include(i => i.HasUniqueRoleAssignments, i=> i.DisplayName));
                context.ExecuteQuery();
                foreach (ListItem item in listItems)
                {
                    Console.WriteLine("{0} {1}", item.HasUniqueRoleAssignments, item.DisplayName);
                    if (item.HasUniqueRoleAssignments)
                    {
                        context.Load(item.RoleAssignments, ra => ra.Include(r => r.Member, r => r.RoleDefinitionBindings));
                        context.ExecuteQuery();
                        foreach (RoleAssignment role in item.RoleAssignments)
                        {
                            Console.WriteLine("  - {0} {1}", role.Member.PrincipalType, role.Member.LoginName);
                            foreach(RoleDefinition rd in role.RoleDefinitionBindings)
                                Console.WriteLine("    + {0}", rd.Name);
                            if(role.Member.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.SharePointGroup)
                            {
                                Group group = context.Web.SiteGroups.GetById(role.Member.Id);
                                context.Load(group, gr => gr.Users);
                                context.ExecuteQuery();
                                foreach(User user in group.Users)
                                    Console.WriteLine("    :: {0}", user.LoginName);
                            }
                        }
                        Console.ReadLine();
                    }
                }
            }
            Console.WriteLine("shp");
            Console.ReadLine();

            string strDirectory = @"D:\Israel Chemicals Ltd-6973382-ICP\TI - 00 Politicas y Procedimientos TI\01 Politicas y Procedimientos 2016";
            DirectorySecurity fSecurity = System.IO.Directory.GetAccessControl(strDirectory);
            var perms = fSecurity.GetAccessRules(true, true, typeof( System.Security.Principal.NTAccount));
            foreach (AccessRule rule in perms)
                Console.WriteLine("{0} {1}", rule.IdentityReference, rule.AccessControlType);
            AddFileSecurity(strDirectory, @"inkia\carlos.magan", FileSystemRights.ReadData, AccessControlType.Allow);
             fSecurity = System.IO.Directory.GetAccessControl(strDirectory);
             perms = fSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            Console.WriteLine("Nuevos permisos:");
            foreach (AccessRule rule in perms)
                Console.WriteLine("{0} {1}", rule.IdentityReference, rule.AccessControlType);
            Console.WriteLine("fin");
            Console.ReadLine();
        }
        private static SecureString GetPasswordFromConsoleInput()
        {
            ConsoleKeyInfo info;

            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            do
            {
                info = Console.ReadKey(true);
                if (info.Key != ConsoleKey.Enter)
                {
                    securePassword.AppendChar(info.KeyChar);
                }
            }
            while (info.Key != ConsoleKey.Enter);
            return securePassword;
        }

        private static void AddFileSecurity(string fileName, string account,
           FileSystemRights rights, AccessControlType controlType)
        {


            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = System.IO.File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            System.IO.File.SetAccessControl(fileName, fSecurity);

        }
        // Removes an ACL entry on the specified file for the specified account.
        private static void RemoveFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {

            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = System.IO.File.GetAccessControl(fileName);

            // Remove the FileSystemAccessRule from the security settings.
            fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            System.IO.File.SetAccessControl(fileName, fSecurity);

        }
    }
}
