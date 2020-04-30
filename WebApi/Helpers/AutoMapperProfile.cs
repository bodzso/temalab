using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Users;
using WebApi.Models.Transactions;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<Models.Users.UpdateModel, User>();
            CreateMap<CreateModel, Transaction>();
        }
    }
}