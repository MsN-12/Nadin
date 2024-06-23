using AutoMapper;
using Nadin.Core.Entities;
using Nadin.Application.DTOs;

namespace Nadin.Application.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}