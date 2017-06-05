using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace BulkUpload
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("User")]
    public class UserPrincipalEx : UserPrincipal
    {
        // Inplement the constructor using the base class constructor. 
        public UserPrincipalEx(PrincipalContext context) : base(context)
        { }

        // Implement the constructor with initialization parameters.    
        public UserPrincipalEx(PrincipalContext context,
                             string samAccountName,
                             string password,
                             bool enabled) : base(context, samAccountName, password, enabled)
        { }

        // Create the "Department" property.    
        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if (ExtensionGet("department").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("department")[0];
            }
            set { ExtensionSet("department", value); }
        }

        // Create the "Manager" property.    
        [DirectoryProperty("manager")]
        public string Manager
        {
            get
            {
                if (ExtensionGet("manager").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("manager")[0];
            }
            set { ExtensionSet("manager", value); }
        }

        // Create the "street" property.    
        [DirectoryProperty("street")]
        public string Street
        {
            get
            {
                if (ExtensionGet("street").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("street")[0];
            }
            set { ExtensionSet("street", value); }
        }

        // Create the "PO_BOX" property.    
        [DirectoryProperty("street")]
        public string PostOfficeBox
        {
            get
            {
                if (ExtensionGet("postOfficeBox").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("postOfficeBox")[0];
            }
            set { ExtensionSet("postOfficeBox", value); }
        }

        // Create the "ZIP_Code" property.    
        [DirectoryProperty("street")]
        public string PostalCode
        {
            get
            {
                if (ExtensionGet("postalCode").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("postalCode")[0];
            }
            set { ExtensionSet("postalCode", value); }
        }

        // Create the "title" property.    
        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if (ExtensionGet("title").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("title")[0];
            }
            set { ExtensionSet("title", value); }
        }

        // Create the "company" property.    
        [DirectoryProperty("title")]
        public string Company
        {
            get
            {
                if (ExtensionGet("company").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("company")[0];
            }
            set { ExtensionSet("company", value); }
        }


        // Create the "employeeType" property.    
        [DirectoryProperty("employeeType")]
        public string EmployeeType
        {
            get
            {
                if (ExtensionGet("employeeType").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("employeeType")[0];
            }
            set { ExtensionSet("employeeType", value); }
        }


        // Create the "employeeType" property.    
        [DirectoryProperty("employeeNumber")]
        public string EmployeeNumber
        {
            get
            {
                if (ExtensionGet("employeeNumber").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("employeeNumber")[0];
            }
            set { ExtensionSet("employeeNumber", value); }
        }

        // Create the "State" property.    
        [DirectoryProperty("st")]
        public string State
        {
            get
            {
                if (ExtensionGet("st").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("st")[0];
            }
            set { ExtensionSet("st", value); }
        }

        // Create the "City" property.    
        [DirectoryProperty("l")]
        public string City
        {
            get
            {
                if (ExtensionGet("l").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("l")[0];
            }
            set { ExtensionSet("l", value); }
        }

        // Create the "City" property.    
        [DirectoryProperty("physicalDeliveryOfficeName")]
        public string Office
        {
            get
            {
                if (ExtensionGet("physicalDeliveryOfficeName").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("physicalDeliveryOfficeName")[0];
            }
            set { ExtensionSet("physicalDeliveryOfficeName", value); }
        }

        // Create the "memberOf" property.    
        [DirectoryProperty("department")]
        public string MemberOf
        {
            get
            {
                if (ExtensionGet("memberOf").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("memberOf")[0];
            }
            set { ExtensionSet("memberOf", value); }
        }

        // Create the "directReports" property.    
        [DirectoryProperty("department")]
        public string DirectReports
        {
            get
            {
                if (ExtensionGet("directReports").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("directReports")[0];
            }
            set { ExtensionSet("directReports", value); }
        }


        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityValue);
        }

        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
        }


    }
}
