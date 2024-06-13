using AutoMapper;
using PokemonReviewApi.Dto;
using PokemonReviewApi.Models;

namespace PokemonReviewApi.Helper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Pokemon, PokemonDto>();
        CreateMap<PokemonDto, Pokemon>();

        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();

        CreateMap<Country, CountryDto>();
        CreateMap<CountryDto, Country>();

        CreateMap<Owner, OwnerDto>();
        CreateMap<OwnerDto, Owner>();

        CreateMap<Review, ReviewDto>();
        CreateMap<ReviewDto, Review>();

        CreateMap<Reviewer, ReviewerDto>();
        CreateMap<ReviewerDto, Reviewer>();
    }
}
