using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<List<Artist>> GetArtistListAsync();
        Task<List<Artist>> GetArtistListByNameAsync(string name);
        Task<Artist> GetArtisByIdAsync(long ArtistId);
    }
}
