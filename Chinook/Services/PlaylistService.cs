using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ChinookContext _chinookContext;
        private readonly IMapper _mapper;
        public const string favoriteConst = "My favorite tracks";

        public PlaylistService(ChinookContext chinookContext, IMapper mapper)
        {
            _chinookContext = chinookContext;
            _mapper = mapper;
        }

        public async Task AddFavoriteTrack(string userId, long trackId)
        {
            try
            {               
                var track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);

                var playList = await _chinookContext.Playlists.Include(t => t.Tracks).FirstOrDefaultAsync(p =>
                         (p.Name == favoriteConst && 
                          (p.UserPlaylists!=null && p.UserPlaylists.Any(up => up.UserId == userId))                         
                         )
                       );

                if( playList == null )
                {
                    Models.Playlist favoritePlayList = new Models.Playlist();
                    favoritePlayList.Name = favoriteConst;
                    var tracks = new List<Models.Track>();
                    tracks.Add(track);
                    favoritePlayList.Tracks = tracks;
                    await _chinookContext.Playlists.AddAsync(favoritePlayList);
                    await _chinookContext.SaveChangesAsync();

                    Models.UserPlaylist userPlaylist = new Models.UserPlaylist();
                    userPlaylist.UserId = userId;
                    userPlaylist.PlaylistId = favoritePlayList.PlaylistId;
                    await _chinookContext.UserPlaylists.AddAsync(userPlaylist);                 
                }
                else
                {
                    if (playList.Tracks == null)
                    {
                        var tracks = new List<Models.Track>();
                        tracks.Add(track);
                        playList.Tracks = tracks;
                    }
                    else
                    {
                        playList.Tracks.Add(track);
                    }
                }
                await _chinookContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UnFavoriteTrack(string userId, long trackId)
        {
            try
            {             
                var track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);

                var playList = await _chinookContext.Playlists.Include(t => t.Tracks).FirstOrDefaultAsync
                    (p =>
                     (p.Name == favoriteConst &&
                     (p.UserPlaylists != null && p.UserPlaylists.Any(up => up.UserId == userId))
                    )
                  );

                if ( playList != null && playList.Tracks != null)
                {
                    playList.Tracks.Remove(track);
                    _chinookContext.Playlists.Update(playList);
                    await _chinookContext.SaveChangesAsync();
                }                       

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<List<ClientModels.Playlist>> GetUserPlaylistAsync(string userId)
        {
            var playList = await _chinookContext.Playlists               
                .Where(p => (p.UserPlaylists != null && p.UserPlaylists.Any(up => up.UserId == userId)))
                .Select(p => new ClientModels.Playlist()
                {
                    Name = p.Name,
                    PlaylistId = p.PlaylistId                  
                })
                .ToListAsync();           

            return playList;
        }

        public async Task<List<ClientModels.PlaylistTrack>> GetPlaylistTrackByArtistIdAsync(string userId, long artistId)
        {
            var playListTracks = await _chinookContext.Tracks.Where(a => a.Album.ArtistId == artistId)
            .Include(a => a.Album)
            .Select(t => new PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == favoriteConst)).Any()
            })
            .ToListAsync();        

            return playListTracks;
        }

        public async Task<ClientModels.Playlist> GetPlaylistByIdAsync(long playListId,string userId)
        {
            var playList=  await _chinookContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playListId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == favoriteConst)).Any()
                }).ToList()
            })
            .FirstOrDefaultAsync();
            return playList;
        }

        public async Task AddUserPlaylist(string playListName, string userId)
        {
            try
            {
                Models.Playlist playList = new Models.Playlist();
                playList.Name = playListName;
               
                await _chinookContext.Playlists.AddAsync(playList);
                await _chinookContext.SaveChangesAsync();

                Models.UserPlaylist userPlaylist = new Models.UserPlaylist();
                userPlaylist.UserId = userId;
                userPlaylist.PlaylistId = playList.PlaylistId;
                await _chinookContext.UserPlaylists.AddAsync(userPlaylist);
                await _chinookContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task AddTrackToPlaylist(long playListId, long trackId)
        {
            try
            {
                var playList = await _chinookContext.Playlists.Include(t => t.Tracks).FirstOrDefaultAsync(p =>
                (p.PlaylistId == playListId)                
                 );

                if(playList != null)
                {
                    var track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);
                    playList.Tracks.Add(track);
                    _chinookContext.Playlists.Update(playList);
                   await _chinookContext.SaveChangesAsync();
                }              

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task RemoveTrackFromPlaylist(long playListId, long trackId)
        {
            try
            {
                var playList = await _chinookContext.Playlists.Include(t => t.Tracks).FirstOrDefaultAsync(p =>
                (p.PlaylistId == playListId)
                 );

                if (playList != null)
                {
                    var track = await _chinookContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);
                    playList.Tracks.Remove(track);
                    _chinookContext.Playlists.Update(playList);
                    await _chinookContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
