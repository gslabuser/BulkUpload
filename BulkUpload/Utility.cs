using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUpload
{
    public class Utility
    {

        public static Person createPerson(string[] tuple)
        {
            Person person = new Person();
            person.firstName = tuple[0];
            person.lastName = tuple[1];
            person.cn = tuple[2];
            person.displayName = tuple[3];
            person.description = tuple[4];
            person.office = tuple[5];
            person.telephoneNumber = tuple[6];
            person.email = tuple[7];
            person.street = tuple[8];
            person.poBox = tuple[9];
            person.city = tuple[10];
            person.state = tuple[11];
            person.zipCode = tuple[12];
            person.country = tuple[13];
            person.jobTitle = tuple[14];
            person.department = tuple[15];
            person.company = tuple[16];
            person.manager = tuple[17];
            person.directReports = tuple[18];
            person.memberOf = tuple[19];
            person.empType = tuple[20];
            person.empNumber = tuple[21];
            person.userPrincipleName = tuple[22];
            person.expiryDate = tuple[23];

            return person;
        }
    }
}
