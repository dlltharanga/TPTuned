using AutoMapper;
using Chinook.Models;

namespace Chinook.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Playlist, ClientModels.Playlist>();
        }
    }
}
