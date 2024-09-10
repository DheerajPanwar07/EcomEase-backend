using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomEaseModel.Model
{
	public class Respond
	{
		public bool success { get; set; }
		public string message { get; set; }

		public object Data { get; set; }
	}
}
