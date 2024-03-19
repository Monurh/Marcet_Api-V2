using Marcet_Api_V2.Model.DTO.Product;
using Marcet_Api_V2.Model.Enums;
using Marcet_Api_V2.Models;

namespace Marcet_Api_V2.Services.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductDTO productDTO);
        Task<IEnumerable<Product>> SearchProductByNameAsync(string productName);
        Task<IEnumerable<Product>> GetSortedProductsAsync(SortProduct sortOption);
        Task UpdateProductAsync(Guid id, ProductDTO productDTO);
        Task DeleteProductAsync(Guid id);
        Task<ProductDTO> GetProductByIdAsync(Guid id);
    }
}
