using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Certifications
{
  public class CertificationMapper : Profile
  {
    public CertificationMapper()
    {
      CreateMap<Managers.Registry.Contract.Certifications.Certification, Certification>().ReverseMap();
      CreateMap<Managers.Registry.Contract.Certifications.CertificationLevel, CertificationLevel>().ReverseMap();
    }
  }
}
