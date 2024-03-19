using Marcet_Api_V2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.UserAddInformation;
using Services.IServices;

namespace Marcet_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _userService;

        public CustomerController(ICustomerService userService)
        {
            _userService = userService;
        }

        [HttpGet("getUserById")]
        public async Task<ActionResult<Customer>> GetUserById()
        {
            try
            {
                string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                Guid id = Guid.Parse(currentUserId);
                var customer = await _userService.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                Guid id = Guid.Parse(currentUserId);
                if (string.IsNullOrEmpty(customerDTO.FirstName) || string.IsNullOrEmpty(customerDTO.LastName) || string.IsNullOrEmpty(customerDTO.Address) ||
                    string.IsNullOrEmpty(customerDTO.Email) || string.IsNullOrEmpty(customerDTO.Phone))
                {
                    throw new ArgumentException("Please fill in all fields");
                }
                if (!char.IsUpper(customerDTO.FirstName[0]) || !char.IsUpper(customerDTO.LastName[0]) || !char.IsUpper(customerDTO.Address[0]))
                {
                    throw new ArgumentException("First name, last name, and address must start with a capital letter");
                }
                if (!customerDTO.FirstName.All(char.IsLetter) || !customerDTO.LastName.All(char.IsLetter))
                {
                    throw new ArgumentException("First name and last name must contain only letters");
                }

                var customer = new CustomerDTO 
                {
                    FirstName = customerDTO.FirstName,
                    LastName = customerDTO.LastName,
                    Address = customerDTO.Address,
                    Email = customerDTO.Email,
                    Phone = customerDTO.Phone
                };

                await _userService.EditCustomerInfo(id, customer);
                return Ok(new { message = "Data updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteCustomerAsync()
        {
            try
            {
                string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                Guid id = Guid.Parse(currentUserId);

                await _userService.DeleteCustomerAsync(id);

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
