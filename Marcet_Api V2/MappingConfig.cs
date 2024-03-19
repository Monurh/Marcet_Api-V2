using AutoMapper;
using Marcet_Api_V2.Model.DTO.Product;
using Marcet_Api_V2.Models;
using Models.Dto.Auth;
using Models.Dto.UserAddInformation;
using TestRest.Models.Dto;

namespace TestRest
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<AuthRequestWithGoogleDTO, Customer>(); 
            CreateMap<Customer, AuthUserResponseWithGoogleDTO>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
