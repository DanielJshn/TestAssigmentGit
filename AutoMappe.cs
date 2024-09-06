
using AutoMapper;
using testProd.task;

namespace testProd.task
{


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           CreateMap<TaskModelDto, TaskModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Игнорируем Id при маппинге
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Игнорируем CreatedAt
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Игнорируем UpdatedAt

        CreateMap<TaskModel, TaskResponseDto>();
        }
    }

}