using AutoMapper;
using CinehubBack.Data.Favorite;
using CinehubBack.Migrations;

namespace CinehubBack.Profiles;

public class FavoriteProfile: Profile
{
    public FavoriteProfile()
    {
        CreateMap<CreateFavoriteDto, Model.Favorites>();
        CreateMap<Model.Favorites, ReadFavoriteDto>();
    }
}