using AutoMapper;
using Post.Command.Api.Commands;
using Post.Command.Api.DTO;

namespace Post.Command.Api.Mappings;

public class CommandToDtoProfile : Profile
{
    public CommandToDtoProfile()
    {
        // New Post mappings
        CreateMap<NewPostCommand, NewPostDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ReverseMap();

        // Edit message mappings
        CreateMap<EditMessageCommand, EditMessageDto>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
            .ReverseMap();

        // Comment mappings
        CreateMap<AddCommentCommand, AddCommentDto>()
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ReverseMap();

        CreateMap<EditCommentCommand, EditCommentDto>()
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ReverseMap();

        // Like post mapping
        CreateMap<LikePostCommand, LikePostDto>().ReverseMap();

        // Delete mappings
        CreateMap<DeletePostCommand, DeletePostDto>().ReverseMap();
        CreateMap<RemoveCommentCommand, RemoveCommentDto>().ReverseMap();
    }
}