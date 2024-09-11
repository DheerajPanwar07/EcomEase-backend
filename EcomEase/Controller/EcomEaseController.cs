using EcomEaseModel.Model;
using EcomEaseRepo.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Numerics;
using Azure;

namespace EcomEase.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	public class EcomEaseController : ControllerBase
	{
		private readonly IEcomEase _ecomEase;

		public EcomEaseController(IEcomEase ecomEase)
        {
			_ecomEase = ecomEase;
		}

		[HttpPost]
		[Route("fillRegistration")]
		public string setData(OprateUserRegsitration data)
		{
			return _ecomEase.SetUser(data);

		}

		//login 
		[HttpPost]
		[Route("login")]
		public ActionResult Login(UserLogin values)
		{
			string res = _ecomEase.LogIn(values);
			if (res != "You are deactivated please wait for admin Activation")
			{
				Respond obj = new Respond()
				{
					success = true,
					message = "successful login",
					Data = res,

				};
				return Ok(obj);
			}
			else
			{
				Respond obj = new Respond()
				{
					success = false,
					message = "login failed",
					Data = res,
				};
				return Ok(obj);
			}
			
		}





		[HttpPost]
		[Route("AddProduct")]
		public string AddProduct(Product data)
		{
			return _ecomEase.AddProduct(data);

		}

		[HttpGet]
		[Route("getProduct")]
		public List<Product> Productlist()
		{
			return _ecomEase.GetProduct();
		}

		[HttpPut("Update/{id}")]
		public string PutProduct(int id,Product product)
		{  
			return _ecomEase.UpdateProduct(id,product);
		}



		[HttpGet]
		[Route("getRegistrationDetails")]
		public List<RegistrationDetials> Registrationlist()
		{
			return _ecomEase.GetallResistrationRecordes();
		}

		[HttpPut("updatestatus/{userId}")]
		public async Task<IActionResult> UpdateStatus(int userId, [FromBody] UpdateStatusRequest request)
		{
			var result = await _ecomEase.UpdateUserStatus(userId, request.IsActive);
			if (!result)
			{
				return NotFound(new { Message = "User not found" });
			}

			return Ok(new { Message = "Status updated successfully" });
		}



		

		[HttpGet("Profile")]
		[Authorize]
		public ActionResult Profile()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userIdClaim))
			{
				return Unauthorized(new Respond
				{
					success = false,
					message = "Invalid token. User ID not found."
				});
			}

			int id = int.Parse(userIdClaim);
			var record = _ecomEase.GetUserRecord(id);

			if (record != null)
			{
				return Ok(new Respond
				{
					success = true,
					message = "User authorized.",
					Data = record
				});
			}
			else
			{
				return NotFound(new Respond
				{
					success = false,
					message = "User ID not found."
				});
			}
		}




	}

	

  
}

