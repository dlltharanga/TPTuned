using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ChinookContext _chinookContext;
        private readonly IMapper _mapper;

        public ArtistService(ChinookContext chinookContext, IMapper mapper)
        {
            _chinookContext = chinookContext;
            _mapper = mapper;
        }

        public async Task<List<Artist>> GetArtistListAsync()
        {
            var artists = await _chinookContext.Artists.Include(a => a.Albums)
                .ToListAsync();

            return artists;
        }

        public async Task<List<Artist>> GetArtistListByNameAsync(string name)
        {
            var artists = await _chinookContext.Artists.Include(a => a.Albums).Where(
                x => (
                      !string.IsNullOrEmpty(x.Name) && x.Name.Contains(name)
                     )
                ).
                ToListAsync();

            return artists;
        }

        public async Task<Artist> GetArtisByIdAsync(long ArtistId)
        {
            var artist = await _chinookContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == ArtistId);

            return artist;
        }

    }
}

