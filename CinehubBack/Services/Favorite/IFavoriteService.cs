using CinehubBack.Data.Favorite;

namespace CinehubBack.Services.Favorite;

public interface IFavoriteService
{
    ReadFavoriteDto CreateFavorite(CreateFavoriteDto createFavoriteDto);
    void DeleteFavorite(DeleteFavoriteDto deleteFavoriteDto);
}