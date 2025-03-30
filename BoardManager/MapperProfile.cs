using System.Text.Json;
using AutoMapper;
using BoardManager.DTOs;

namespace BoardManager
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<NewBoardRequest, BoardAccess.Models.Board>()
                .ForMember(dest => dest.InitialState, opt => opt.MapFrom(src => MapBoardState(src.InitialState)));
            CreateMap<BoardAccess.Models.Board, Board>()
                .ForMember(dest => dest.InitialState, opt => opt.MapFrom(src => MapBoardState(src.InitialState)));
            CreateMap<BoardAccess.Models.Generation, Generation>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => MapBoardState(src.State)));
            CreateMap<NewGenerationRequest, BoardAccess.Models.Generation>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => MapBoardState(src.State)));
        }

        private string MapBoardState(bool[][] state) {
            var result = JsonSerializer.Serialize(state);
            return result;
        }
        private bool[][] MapBoardState(string state)
        {
            var result = JsonSerializer.Deserialize<bool[][]>(state);
            return result;
        }
    }
}
