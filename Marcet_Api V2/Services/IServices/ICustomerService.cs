using Marcet_Api_V2.Models;
using Models.Dto.UserAddInformation;

namespace Services.IServices
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(Guid id);
        Task<IEnumerable<Models.Dto.UserAddInformation.CustomerDTO>?> GetCustomerById(Guid id);
        Task EditCustomerInfo(Guid id, CustomerDTO customerDTO);
        Task DeleteCustomerAsync(Guid id);
        Task CreateAsync(Customer customer);
        Task<Customer> GetCustomerByEmailAsync(string email);
    }
}
