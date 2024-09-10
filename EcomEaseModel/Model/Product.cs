using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomEaseModel.Model
{
	public class Product
	{
		[Key]
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string price { get; set; }

		public string description { get; set; } 
		public string quantity { get; set; }

		public int sellerId { get; set; }
        
		

	}
}
