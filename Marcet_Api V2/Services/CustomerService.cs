using Microsoft.EntityFrameworkCore;
using Marcet_Api_V2.Models;
using Marcet_Api_V2.Repository;
using Repository.IRepository;
using Models.Dto.UserAddInformation;
using Services.IServices;

namespace Marcet_Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly MarcetDbContext _dbContext;

        public CustomerService(IRepository<Customer> dbCustomer, MarcetDbContext dbContext)
        {
            _customerRepository = dbCustomer;
            _dbContext = dbContext;
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                return await _customerRepository.GetAsync(u => u.CustomerId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CustomerDTO>?> GetCustomerById(Guid id)
        {
            var query = _dbContext.Customers
                   .AsNoTracking()
                   .Where(u => u.CustomerId == id)
                   .Select(u => new CustomerDTO()
                   {
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       Address = u.Address,
                       Email = u.Email,
                       Phone = u.Phone
                   })
                   .AsQueryable();
            return query.ToList();
        }


        public async Task EditCustomerInfo(Guid id, CustomerDTO customerDTO)
        {
            try
            {
                var existingCustomer = await _dbContext.Customers.FindAsync(id);
                if (existingCustomer != null)
                {
                    if (customerDTO.FirstName != null)
                    {
                        existingCustomer.FirstName = customerDTO.FirstName;
                    }
                    if (customerDTO.LastName != null)
                    {
                        existingCustomer.LastName = customerDTO.LastName;
                    }
                    if (customerDTO.Address != null)
                    {
                        existingCustomer.Address = customerDTO.Address;
                    }
                    if (customerDTO.Email != null)
                    {
                        existingCustomer.Email = customerDTO.Email;
                    }
                    if (customerDTO.Phone != null)
                    {
                        existingCustomer.Phone = customerDTO.Phone;
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (ArgumentException ex)
            {
                throw;
            }
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            try
            {
                await _customerRepository.DeleteAsync(u => u.CustomerId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            try
            {
                return await _customerRepository.GetAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task CreateAsync(Customer customer)
        {
            try
            {
                await _customerRepository.CreateAsync(customer);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

