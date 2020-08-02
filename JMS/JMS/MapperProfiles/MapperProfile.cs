using AutoMapper;
using JMS.Models.Register;
using JMS.Models.Submissions;
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
            CreateMap<RegisterAuthorModel, ViewModels.Register.RegisterAuthorModel>();
            CreateMap<AddSubmissionFileModel,ViewModels.Submissions.AddSubmissionFileModel>().ForMember(x=>x.FileName,y=>y.MapFrom(z=>z.File.FileName)).ForMember(x=>x.FileStream,y=>y.MapFrom(z=>z.File.OpenReadStream()));
            CreateMap<AddContributerModel, ViewModels.Submissions.AddContributerViewModel>();
            CreateMap<EditContributerModel, ViewModels.Submissions.EditContributerModel>();
        }
    }
}
