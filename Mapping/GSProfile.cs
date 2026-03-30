using AutoMapper;
using HrBackend.DTO_S.GeneralSettings;
using HrBackend.Models;

public class GSProfile : Profile
{
    public GSProfile()
    {
        CreateMap<UpdateGeneralSettingsDTO, GeneralSettings>();
    }
}
