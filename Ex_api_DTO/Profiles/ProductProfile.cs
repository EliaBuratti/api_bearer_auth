using AutoMapper;

namespace Ex_api_DTO.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile() 
        {
            CreateMap<Entities.Product, Models.ProductDto>();
        }
    }
}
