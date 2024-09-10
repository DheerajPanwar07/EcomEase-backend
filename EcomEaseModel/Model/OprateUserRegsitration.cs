using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomEaseModel.Model
{
	public class OprateUserRegsitration
	{


		public string name { get; set; }
		public string email { get; set; }
		public string contact { get; set; }
		public string address { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string country { get; set; }
		public string rolename { get; set; }
		public bool isActive { get; set; }

	}
}
