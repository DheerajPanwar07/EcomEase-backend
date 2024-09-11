using EcomEaseContextDb.Context;
using EcomEaseModel.Model;
using EcomEaseRepo.Interface;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using EcomEaseContextDb.Migrations;
using Microsoft.EntityFrameworkCore;


namespace EcomEaseRepo.Services
{
	public class SEcomEase : IEcomEase
	{
		private readonly EcomEaseContext _ecomEaseContext;
		private readonly IConfiguration _config;
		private readonly MailModel _mailSettings;

		public SEcomEase(EcomEaseContext EcomEaseContext, IOptions<MailModel> mailSetting, IConfiguration config)
		{
			_ecomEaseContext = EcomEaseContext;
			_config = config;
			_mailSettings = mailSetting.Value;
		}
		public string SetUser(OprateUserRegsitration values)



		{   // !!! we select the id form database on the behalf of rolename!!!!!
			int roleId = _ecomEaseContext.EcomRoles.Where(r => r.RoleName == values.rolename).Select(r => r.RoleId).FirstOrDefault();

			// Generate a random password

			string generatedPassword = GenerateRandomPassword();


			_ecomEaseContext.UserRegistrations.Add(new UserRegsitration
			{
				name = values.name,
				email = values.email,
				password = generatedPassword, // Save the generated password
				contact = values.contact,
				address = values.address,
				city = values.city,
				state = values.state,
				country = values.country,
				roleId = roleId,
				isActive = values.isActive
			});




			// Save changes to the database
			_ecomEaseContext.SaveChanges();


			// Send the password to the user's  by  Email
			string body = $" Hii {values.name} welcome to EcomEase ,Your  email: {values.email} and Password is: {generatedPassword} Now you can enjoy our services Keep your email and password safely..";
			var sent = SendEmailAsync(values.email, generatedPassword, body);
			if (sent != null)
			{
				return "Your password has been sent to your Email!";
			}

			// Optionally return some success message
			return "User successfully registered with role: " + roleId;
		}

		private string GenerateRandomPassword()
		{
			const int passwordLength = 8;
			const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			StringBuilder randomPassword = new StringBuilder();
			Random random = new Random();

			for (int i = 0; i < passwordLength; i++)
			{
				randomPassword.Append(validChars[random.Next(validChars.Length)]);
			}

			return randomPassword.ToString();
		}



		// email Receive passGenrated = qaxp iyun ghot muxw

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_mailSettings.EmailId);
			email.To.Add(MailboxAddress.Parse(toEmail));
			email.Subject = subject;
			var builder = new BodyBuilder();

			builder.HtmlBody = body;
			email.Body = builder.ToMessageBody();
			using var smtp = new SmtpClient();
			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_mailSettings.EmailId, _mailSettings.Password);
			await smtp.SendAsync(email);
			smtp.Disconnect(true);

		}


		//  user Login
		public string LogIn(UserLogin values)
		{
			int rollId = _ecomEaseContext.UserRegistrations.Where(u => u.email == values.Email && u.password == values.Password).Select(Id => Id.roleId).FirstOrDefault();
			int userId = _ecomEaseContext.UserRegistrations.Where(u => u.email == values.Email && u.password == values.Password).Select(Id => Id.userid).FirstOrDefault();
			bool action= _ecomEaseContext.UserRegistrations.Where(u=>u.userid == userId).Select(u=>u.isActive).FirstOrDefault();	

			if (rollId != 0 && action==true)
			{
				var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
				var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

				var claims = new[]
				{
					new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
					  new Claim(ClaimTypes.Role, rollId.ToString())
				};

				var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
					  _config["Jwt:Issuer"],
					   claims,
					  expires: DateTime.Now.AddMinutes(120),
					  signingCredentials: credentials);

				var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
				// appeded tokent with roll id
				return token;
			}
			{
				return "You are deactivated please wait for admin Activation";
			}
		}



		// product add 

		public string  AddProduct(Product values)
		{  if (values != null)
			{
				_ecomEaseContext.ProductsItems.Add(new Product
				{

					Name = values.Name,
					price = values.price,
					description = values.description,
					quantity = values.quantity,
					sellerId = values.sellerId,

				});
				_ecomEaseContext.SaveChanges();

				return "product is succesfully added";

			}
			else
			{
				return "please Fill data ";
			}

		}

		public List<Product> GetProduct()
		{
			return _ecomEaseContext.ProductsItems.ToList();
		}

		public String  UpdateProduct(int Id,Product values)
		{
			var Linefirst = _ecomEaseContext.ProductsItems.FirstOrDefault(u => u.ProductId == Id);
			if (Linefirst != null)
			{
             Linefirst.Name = values.Name;
			 Linefirst.price = values.price;
			 Linefirst.description = values.description;
			 Linefirst.quantity = values.quantity;
			 Linefirst.sellerId = values.sellerId;

				_ecomEaseContext.SaveChanges();
				return "succesfully update";
			}
			else
			{
				return "please fill correct details";
			}

		}



		


	    public List<RegistrationDetials> GetallResistrationRecordes()

		{

			var list = new List<RegistrationDetials>();

			var details = from e in _ecomEaseContext.UserRegistrations
						  join a in _ecomEaseContext.EcomRoles
						  on e.roleId equals a.RoleId
						  where e.roleId==2||a.RoleId==3	
						  select new RegistrationDetials
						  {
							  UserID = e.userid,
							  UserName = e.name,
							  Email = e.email,
							  Contact = e.contact,
							  Addresh = e.address+","+e.city+","+e.country,
							  RoleName = a.RoleName,
							  IsActive = e.isActive,
						  };

			list.AddRange(details);

			return list;


		}


		

		public async Task<bool> UpdateUserStatus(int userId, bool isActive)
		{
			var user = await _ecomEaseContext.UserRegistrations.FindAsync(userId);
			if (user == null)
			{
				return false; // User not found
			}

			user.isActive = isActive;
			await _ecomEaseContext.SaveChangesAsync();
			return true; // Status updated successfully
		}

		public List<UserRegsitration> GetUserRecord(int userId)
		{
			var userRecord = _ecomEaseContext.UserRegistrations.Where(u => u.userid == userId).ToList();
			return userRecord;
		}
	}

}

