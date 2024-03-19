using FluentValidation;
using Marcet_Api_V2.Model.DTO.Product;

namespace Marcet_Api_V2.Validators.Product
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty();
            RuleFor(x => x.Price).NotNull().GreaterThan(0);
            RuleFor(x => x.StockQuantity).NotNull().GreaterThanOrEqualTo(0);
        }
    }
}
