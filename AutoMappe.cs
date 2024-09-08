
using AutoMapper;
using testProd.task;

namespace testProd.task
{


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
        CreateMap<TaskModelDto, TaskModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<PaginatedList<TaskModel>, PaginatedList<TaskResponseDto>>();
        CreateMap<TaskModel, TaskResponseDto>();
        }
    }

}