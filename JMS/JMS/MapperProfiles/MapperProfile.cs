using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.Users.CreateUserViewModel, ViewModels.Users.CreateUserViewModel>();
        }
    }
}
