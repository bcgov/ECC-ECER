using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Resources.Accounts.Documents;
using ECER.Utilities.DataverseSdk.Model;
using Ganss.Xss;

namespace ECER.Resources.Accounts.Communications;

internal class DocumentRepositoryMapper : Profile
{
  public DocumentRepositoryMapper()
  {
    CreateMap<bcgov_DocumentUrl, Document>(MemberList.Destination)
     .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
     .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName))
     .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize));
  }
}
