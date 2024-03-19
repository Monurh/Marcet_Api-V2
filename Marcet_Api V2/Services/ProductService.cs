using AutoMapper;
using Marcet_Api_V2.Model.DTO.Product;
using Marcet_Api_V2.Model.Enums;
using Marcet_Api_V2.Models;
using Marcet_Api_V2.Repository;
using Marcet_Api_V2.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Linq.Expressions;

namespace Marcet_Api_V2.Services
{
    public class ProductService : IProductService
    {
        private readonly MarcetDbContext _dbContext;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public ProductService(MarcetDbContext dbContext, IRepository<Product> productRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _productRepository = productRepository;
            _mapper = mapper;
            _mapper = mapper;
        }

        public async Task<ProductDTO> AddProductAsync(ProductDTO productDTO)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = productDTO.ProductName,
                Description = productDTO.Description,
                Price = productDTO.Price,
                StockQuantity = productDTO.StockQuantity,
                Category = productDTO.Category,
                Manufacturer = productDTO.Manufacturer
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return productDTO;
        }
        public async Task<IEnumerable<Product>> SearchProductByNameAsync(string productName)
        {
            var allProducts = await _productRepository.GetAllAsync();
            return allProducts.Where(p => p.ProductName.Contains(productName));
        }

        public async Task<IEnumerable<Product>> GetSortedProductsAsync(SortProduct sortOption)
        {
            IQueryable<Product> query = _productRepository.Query();

            switch (sortOption)
            {
                case SortProduct.NameAsc:
                    query = query.OrderBy(p => p.ProductName);
                    break;
                case SortProduct.NameDesc:
                    query = query.OrderByDescending(p => p.ProductName);
                    break;
                case SortProduct.PriceAsc:
                    query = query.OrderBy(p => p.Price);
                    break;
                case SortProduct.PriceDesc:
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case SortProduct.Category:
                    query = query.OrderBy(p => p.Category);
                    break;
                default:
                    throw new ArgumentException("Invalid sort option");
            }

            return await query.ToListAsync();
        }

        public async Task UpdateProductAsync(Guid id, ProductDTO productDTO)
        {
            var existingProduct = await _productRepository.GetAsync(p => p.ProductId == id);
            if (existingProduct == null)
            {
                throw new ArgumentException("Product not found");
            }

            _mapper.Map(productDTO, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);
        }
        public async Task DeleteProductAsync(Guid id)
        {
            try
            {
                Expression<Func<Product, bool>> filter = p => p.ProductId == id;
                await _productRepository.DeleteAsync(filter);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete product", ex);
            }
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            return _mapper.Map<ProductDTO>(product);
        }
    }
}
