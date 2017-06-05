using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkUpload
{
	class Status
	{
		public String personName;

		public String status;

		public String message;

		public String getPersonName()
		{
			return personName;
		}

		public String getStatus()
		{
			return status;
		}

		public String getMessage()
		{
			return message;
		}

		public void setPersonName(String name)
		{
			this.personName = name;
		}

		public void setStatus(String status)
		{
			this.status = status;
		}

		public void setMessage(String message)
		{
			this.message = message;
		}

	}
}
