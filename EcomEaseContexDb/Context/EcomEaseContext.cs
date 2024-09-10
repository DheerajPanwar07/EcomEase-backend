using EcomEaseModel.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomEaseContextDb.Context
{
	public class EcomEaseContext : DbContext
	{
		public EcomEaseContext(DbContextOptions options) : base(options)
		{

		}
      	public DbSet<UserRegsitration> UserRegistrations { get; set; }
		public DbSet<EcomRole> EcomRoles { get; set; }

		public DbSet<Product> ProductsItems { get; set; }
	}
}




