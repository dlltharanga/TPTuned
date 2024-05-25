using Chinook.ClientModels;

namespace Chinook.Services
{
    public interface IPlaylistService
    {
        Task<List<Playlist>> GetUserPlaylistAsync(string userId);
        Task AddFavoriteTrack(string userId,long trackId);
        Task UnFavoriteTrack(string userId, long trackId);
        Task<List<PlaylistTrack>> GetPlaylistTrackByArtistIdAsync(string userId, long artistId);
        Task<ClientModels.Playlist> GetPlaylistByIdAsync(long playListId, string userId);
        Task AddUserPlaylist(string playListName, string userId);

        Task AddTrackToPlaylist(long playListId, long trackId);

        Task RemoveTrackFromPlaylist(long playListId, long trackId);
        
    }
}
