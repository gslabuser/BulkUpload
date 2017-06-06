using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;

namespace BulkUpload
{
    class Boot
    {
        public static List<Person> personList;
		
        static log4net.ILog log = log4net.LogManager.GetLogger("FileLogger");// log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		static log4net.ILog report = log4net.LogManager.GetLogger("ReportLogger");
        public static Properties properties;
        public static string Hostname;
        public static string Username;
        public static string Password;
        public static string BaseDN;
        public static string TopDN;
        public static Dictionary<Person, List<string>> reportMap;
		public static ArrayList statusList;


        static void Main(string[] args)
        {
            reportMap = new Dictionary<Person, List<string>>();
            statusList = new ArrayList();
            try
            {
                properties = new Properties("C:\\BulkUpload\\config.properties");
                if (properties != null)
                {
                    Hostname = properties.get("Hostname");
                    Username = properties.get("Username");
                    Password = properties.get("Password");
                    BaseDN = properties.get("BaseDN");
                    TopDN = properties.get("TopDN");
                }
                if ((Hostname==null || Hostname=="") || (Username == null || Username == "") || (Password == null || Password == "")
                    || (BaseDN == null || BaseDN == "") || (TopDN == null || TopDN == ""))
                {
                    Console.Write("\nOne or more entries not found in properties file");
                    Console.ReadKey();
                }
            }
            catch (Exception e) {
                log.Error("Exception occurred while reading from properties file: "+e.StackTrace);
                Console.Write("\n"+e.StackTrace);
                Console.ReadKey();
            }
            try
            {
                using (var fs = File.OpenRead(@"C:\BulkUpload\upload.csv"))
                using (var reader = new StreamReader(fs))
                {
                    personList = new List<Person>();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!line.StartsWith("firstName,"))
                        {
                            string[] values = line.Split(',');
                            personList.Add(Utility.createPerson(values));
                        }
                    }
                }
                createPersonEntries();
            }catch(Exception e)
            {
                log.Error("Exception occurred while reading from upload.csv file: " + e.StackTrace);
                Console.Write("\n"+e.Message);
                Console.ReadKey();
            }
        }

        static void createPersonEntries()
        {
            foreach(Person person in personList)
            {
                if (Search(person) == 200)
                {
                    log.Error("User '" + person.email + "' already exist in AD.");
					report.Error("Error while creating user '" + person.email + "'. User already exist.");
                }
                else
                {
                    try
                    {
                        CreateScalarAttributes(person);
                    }
                    catch (Exception e)
                    {
                        log.Error("Error while creating user '" + person.email + "' " + e.StackTrace);
						report.Error("Failed to create user '" + person.email + "'.");
					}
					log.Info("User '" + person.email + "' Created successfully!");
				}
            }
            foreach (Person person in personList)
            {
                CreateGroupAttributes(person);
            }
            Console.Write("\nBulk Upload is complete.\nPlease check report in logs: C:/BulkUpload/logs/CreateUserReport.log");
            Console.ReadKey();
		}

        private static void CreateScalarAttributes(Person person)
        {
            DirectoryEntry rootEntry = getUsersDN();

            DirectoryEntry usersEntry = rootEntry.Children.Add("CN=" + person.firstName, "user");
            usersEntry.Properties["sn"].Value = person.lastName;
            usersEntry.Properties["department"].Value = person.department;
            usersEntry.Properties["telephoneNumber"].Value = person.telephoneNumber;
            usersEntry.Properties["mail"].Value = person.email;
            usersEntry.Properties["displayName"].Value = person.displayName;
            usersEntry.Properties["description"].Value = person.description;
            usersEntry.Properties["physicalDeliveryOfficeName"].Value = person.office;
            usersEntry.Properties["streetAddress"].Value = person.street;
            usersEntry.Properties["l"].Value = person.city;
         //   usersEntry.Properties["postOfficeBox"].Value = person.poBox;
            usersEntry.Properties["title"].Value = person.jobTitle;
            usersEntry.Properties["company"].Value = person.company;
			usersEntry.Properties["postalCode"].Value = person.zipCode;
			usersEntry.Properties["st"].Value = person.state;
			usersEntry.Properties["userPrincipalName"].Value = person.email;

			string managerDN = GetDN(person.manager);
            if (managerDN != null)
            {
                usersEntry.Properties["manager"].Value = managerDN;
            }
            else
            {
                log.Error("Error while adding Manager for user '" + person.email + "'. Check if manager present in AD.");
			}
            usersEntry.Properties["employeeType"].Value = person.empType;
            usersEntry.Properties["employeeNumber"].Value = person.empNumber;

            if (person.expiryDate != null && person.expiryDate!="")
            {
                DateTime Expirationdate = DateTime.ParseExact(person.expiryDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                usersEntry.Properties["accountExpires"].Value = Convert.ToString((Int64)Expirationdate.ToFileTime());
            }
            else
            {
                log.Error("Expiration time not added for user '" + person.email + "'.");
            }
            usersEntry.Properties["userAccountControl"].Value = 544;
			try
			{
				usersEntry.CommitChanges();
				report.Debug("User '" + person.email + "' created successfully.");
			}
			catch (Exception e)
			{
				report.Error("Failed to create user '" + person.email + "'.");
			}
        }

        private static void CreateGroupAttributes(Person person)
        {
            List<string> messages = new List<string>();

            string path = "LDAP://" + Hostname + "/" + "CN=" + person.firstName + "," + BaseDN;
            Console.Write("\n"+path);

            DirectoryEntry usersEntry = new DirectoryEntry(path, Username, Password);

            manageDirectReports(person, usersEntry.Properties["distinguishedName"].Value.ToString());
            manageMemberOfAttribute(person, usersEntry.Properties["distinguishedName"].Value.ToString());
            
        }

        private static string GetDN(string Email)
        {
            try
            {
                System.DirectoryServices.AccountManagement.PrincipalContext adContext = EstablishConnection();
                System.DirectoryServices.AccountManagement.PrincipalSearcher searcher = new System.DirectoryServices.AccountManagement.PrincipalSearcher();
                UserPrincipalEx findUser = new UserPrincipalEx(adContext);
                findUser.EmailAddress = Email;
                searcher.QueryFilter = findUser;
                UserPrincipalEx foundUser = (UserPrincipalEx)searcher.FindOne();
                if (foundUser != null)
                {
                    using (DirectoryEntry baseEntry = foundUser.GetUnderlyingObject() as DirectoryEntry)
                    {
                        using (DirectoryEntry entry = new DirectoryEntry(baseEntry.Path, baseEntry.Username, Password))
                        {
                            return (string)entry.Properties["distinguishedName"].Value;
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                log.Error("Error while fetching DN of user '" + Email + "' from AD " + e.StackTrace);
                return e.Message;
            }
        }



        public static int Search(Person person)
        {
            int success = 200;
            int error = 404;
            int exception = 415;
            String path = "LDAP://" + Hostname + "/" + BaseDN; ;
            String username = Username;
            String password = Password;
            DirectoryEntry baseEntry = new DirectoryEntry(path, username, password);
            try
            {
                if (person.email == null)
                {
                    log.Error("Exception in search user. User email is null");
                    return exception;
                }
                System.DirectoryServices.AccountManagement.PrincipalContext adContext = EstablishConnection();
                if (adContext != null)
                {
                    using (adContext)
                    {
                        DirectorySearcher dirSearcher = new DirectorySearcher(baseEntry);
                        dirSearcher.Filter = "(&(objectClass=person))";
                        dirSearcher.SearchScope = SearchScope.Subtree;

                        SearchResultCollection results = dirSearcher.FindAll();

                        for (int i = 0; i < results.Count; i++)
                        {
                            DirectoryEntry entry = results[i].GetDirectoryEntry();
                            String mailValue = (string)entry.Properties["mail"].Value;
                            if ((mailValue != null) && mailValue.Equals(person.email))
                            {
                                log.Info("User found in AD '" + person.email + "'");
                                return success;
                            }
                        }
                        log.Info("User not found in AD '" + person.email + "'");
                        return error;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                log.Info("Exception in search user '" + person.email + "' " + e.StackTrace);
                return e.HResult;
            }
        }

        private static DirectoryEntry getUsersDN()
        {
            try
            {
                DirectoryEntry usersEntry = new DirectoryEntry("LDAP://" + Hostname + "/" + BaseDN, Username, Password);
                usersEntry.RefreshCache();
                return usersEntry;
            }
            catch (Exception e)
            {
                log.Error("Error while fetching baseDN: " + e.StackTrace);
                return null;
            }
        }

        private static void manageDirectReports(Person person, String userDN)
        {
            String directReportsStr = person.directReports;
            String[] strArray = directReportsStr.Split(';');
            ArrayList list = new ArrayList();
            if ((strArray != null) && (strArray.Length > 0))
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    list.Add(strArray[i]);

                }
                searchEntryByEmail(list, userDN);
            }
        }

        private static void searchEntryByEmail(ArrayList list, String userDN)
        {
            try
            {
                ArrayList returnList = new ArrayList();
                DirectoryEntry de = getUsersDN();
                DirectorySearcher ds = new DirectorySearcher(de);
                ds.Filter = "(&(objectClass=person))";
                ds.SearchScope = SearchScope.Subtree;

                SearchResultCollection results = ds.FindAll();

                for (int i = 0; i < results.Count; i++)
                {
                    DirectoryEntry entry = results[i].GetDirectoryEntry();
                    String mailValue = (string)entry.Properties["mail"].Value;
                    if ((mailValue != null) && (list.Contains(mailValue)))
                    {
                        returnList.Add(entry.Properties["distinguishedName"].Value);
                        entry.Properties["manager"].Value = userDN;
                        entry.CommitChanges();
                        entry.RefreshCache();
                        log.Info("Successfully added direct reports for user");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while assigning direct reports attribute to user " + e.StackTrace);
            }
        }

        private static void manageMemberOfAttribute(Person person, String userDN)
        {
            String memberOfString = person.memberOf;
            String[] strArray = memberOfString.Split(';');
            ArrayList list = new ArrayList();
            if ((strArray != null) && (strArray.Length > 0))
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    list.Add(strArray[i]);
                }
                getGroupsDN(list, userDN);
            }

        }

        private static void getGroupsDN(ArrayList list, String userDN)
        {
            try
            {
                ArrayList returnList = new ArrayList();
                DirectoryEntry de = getTopDN();
                DirectorySearcher ds = new DirectorySearcher(de);
                ds.Filter = "(&(objectClass=group))";
                ds.SearchScope = SearchScope.Subtree;

                SearchResultCollection results = ds.FindAll();

                for (int i = 0; i < results.Count; i++)
                {
                    DirectoryEntry entry = results[i].GetDirectoryEntry();
                    String cnValue = (string)entry.Properties["cn"].Value;
                    if ((cnValue != null) && (list.Contains(cnValue)))
                    {
                        returnList.Add(entry.Properties["distinguishedName"].Value);
						entry.Properties["member"].Add(userDN);
						entry.CommitChanges();
                        entry.RefreshCache();
                        log.Info("Successfully added memberOf attributes for user ");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while assigning MemberOf attribute to user " + e.StackTrace);
            }
        }

        private static DirectoryEntry getTopDN()
        {
            try
            {
                DirectoryEntry usersEntry = new DirectoryEntry("LDAP://" + Hostname + "/" + TopDN, Username, Password);
                usersEntry.RefreshCache();
                return usersEntry;
            }
            catch (Exception e)
            {
                log.Error("Error while fetching baseDN: " + e.StackTrace);
                return null;
            }
        }

        private static System.DirectoryServices.AccountManagement.PrincipalContext EstablishConnection()
        {
            try
            {
                System.DirectoryServices.AccountManagement.PrincipalContext adContext = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, Hostname, BaseDN, @Username, Password);
                Boolean result = adContext.ValidateCredentials(Username, Password);
                if (result)
                {
                    log.Info("Successfully Established connection to AD '" + Hostname + "' with username '" + Username + "'");
                    return adContext;
                }
                return null;
            }
            catch (Exception e)
            {
                log.Error("Exception in Establish Connection to AD '" + Hostname + "' with username '" + Username + "'");
                Console.Write("\nError in establish connection: "+e.Message);
                throw new Exception();
            }
        }

    }
}
