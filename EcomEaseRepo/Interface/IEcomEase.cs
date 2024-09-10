using EcomEaseModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EcomEaseRepo.Services.SEcomEase;

namespace EcomEaseRepo.Interface
{
	public  interface IEcomEase
	{
		//qaxp iyun ghot muxw
		public string SetUser(OprateUserRegsitration values);
		
		Task SendEmailAsync(string toEmail, string subject, string body);
		//public string loginAdmin(AdminLogin values);
		public string LogIn(UserLogin values);

		public string AddProduct(Product values);
        public List<Product> GetProduct();
        public String UpdateProduct(int Id, Product values);
		public List<RegistrationDetials> GetallResistrationRecordes();
		public List<UserRegsitration> GetUserRecord(int userId);

		Task<bool> UpdateUserStatus(int userId, bool isActive);

	}
}
