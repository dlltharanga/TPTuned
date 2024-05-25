using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private readonly ChinookContext _chinookContext;
        private readonly IMapper _mapper;

        public TrackService(ChinookContext chinookContext, IMapper mapper)
        {
            _chinookContext = chinookContext;
            _mapper = mapper;
        }     

    }
}

