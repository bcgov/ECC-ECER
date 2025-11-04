using AutoMapper;
using System.Diagnostics.Contracts;

namespace ECER.Clients.RegistryPortal.Server.ICRA;

public class ICRAEligibilityMapper : Profile
{
  public ICRAEligibilityMapper()
  {
    CreateMap<ICRAEligibility, Managers.Registry.Contract.ICRA.ICRAEligibility>()
      .ReverseMap();

    CreateMap<InternationalCertification, Managers.Registry.Contract.ICRA.InternationalCertification>()
      .ReverseMap();
    CreateMap<EmploymentReference, Managers.Registry.Contract.ICRA.EmploymentReference>()
      .ForMember(d => d.Status, o => o.Ignore())
      .ForMember(d => d.WillProvideReference, o => o.Ignore())
      .ReverseMap();
    CreateMap<Managers.Registry.Contract.ICRA.ICRAEligibility, ICRAEligibilityStatus>()
      .ForMember(d => d.EmploymentReferencesStatus, o => o.MapFrom(s => s.EmploymentReferences));
    CreateMap<Managers.Registry.Contract.ICRA.EmploymentReference, EmploymentReferenceStatus>()
      .ForCtorParam(nameof(EmploymentReference.Id), o => o.MapFrom(s => s.Id))
      .ForCtorParam(nameof(EmploymentReference.Status), o => o.MapFrom(s => s.Status))
      .ForCtorParam(nameof(EmploymentReference.FirstName), o => o.MapFrom(s => s.FirstName))
      .ForCtorParam(nameof(EmploymentReference.LastName), o => o.MapFrom(s => s.LastName))
      .ForCtorParam(nameof(EmploymentReference.EmailAddress), o => o.MapFrom(s => s.EmailAddress))
      .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.WillProvideReference, o => o.MapFrom(s => s.WillProvideReference));
  }
}
