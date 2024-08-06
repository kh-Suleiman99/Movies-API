﻿using AutoMapper;
using MoviesApi.Dtos;
using MoviesApi.Entities;

namespace MoviesApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MovieDto, Movie>()
                .ForMember(src => src.Poster, opt => opt.Ignore());
        }
    }
}
