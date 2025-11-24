using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.MetadataResources;

internal class MetadataResourceRepositoryMapper : Profile
{
  public MetadataResourceRepositoryMapper()
  {
    CreateMap<ecer_Province, Province>(MemberList.Source)
        .ForCtorParam(nameof(Province.ProvinceId), opt => opt.MapFrom(src => src.ecer_ProvinceId))
        .ForCtorParam(nameof(Province.ProvinceName), opt => opt.MapFrom(src => src.ecer_Name))
        .ForCtorParam(nameof(Province.ProvinceCode), opt => opt.MapFrom(src => src.ecer_Abbreviation))
        .ValidateMemberList(MemberList.Destination)
        .ReverseMap()
        .ForMember(dest => dest.ecer_ProvinceId, opt => opt.MapFrom(src => src.ProvinceId))
        .ForMember(dest => dest.ecer_Name, opt => opt.MapFrom(src => src.ProvinceName))
        .ForMember(dest => dest.ecer_Abbreviation, opt => opt.MapFrom(src => src.ProvinceCode));

    CreateMap<ecer_SystemMessage, SystemMessage>(MemberList.Source)
        .ForCtorParam(nameof(SystemMessage.Message), opt => opt.MapFrom(src => src.ecer_message))
        .ForCtorParam(nameof(SystemMessage.Subject), opt => opt.MapFrom(src => src.ecer_subject))
        .ForCtorParam(nameof(SystemMessage.Name), opt => opt.MapFrom(src => src.ecer_name))
        .ForMember(d => d.StartDate, opts => opts.MapFrom(s => s.ecer_startdate))
        .ForMember(d => d.EndDate, opts => opts.MapFrom(s => s.ecer_enddate))
        .ForMember(d => d.PortalTags, opts => opts.MapFrom(s => s.ecer_PortalTags))
        .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_Country, Country>(MemberList.Source)
        .ForCtorParam(nameof(Country.CountryId), opt => opt.MapFrom(src => src.ecer_CountryId))
        .ForCtorParam(nameof(Country.CountryName), opt => opt.MapFrom(src => src.ecer_Name))
        .ForCtorParam(nameof(Country.CountryCode), opt => opt.MapFrom(src => src.ecer_ShortName))
        .ForCtorParam(nameof(Country.IsICRA), opt => opt.MapFrom(src => src.ecer_EligibleforICRA))
        .ValidateMemberList(MemberList.Destination)
        .ReverseMap()
        .ForMember(dest => dest.ecer_CountryId, opt => opt.MapFrom(src => src.CountryId))
        .ForMember(dest => dest.ecer_Name, opt => opt.MapFrom(src => src.CountryName))
        .ForMember(dest => dest.ecer_EligibleforICRA, opt => opt.MapFrom(src => src.IsICRA))
        .ForMember(dest => dest.ecer_ShortName, opt => opt.MapFrom(src => src.CountryCode));

    CreateMap<ecer_PostSecondaryInstitute, PostSecondaryInstitution>(MemberList.Source)
       .ForCtorParam(nameof(PostSecondaryInstitution.Id), opt => opt.MapFrom(src => src.ecer_PostSecondaryInstituteId))
       .ForCtorParam(nameof(PostSecondaryInstitution.Name), opt => opt.MapFrom(src => src.ecer_Name))
       .ForCtorParam(nameof(PostSecondaryInstitution.ProvinceId), opt => opt.MapFrom(src => src.ecer_ProvinceId.Id))
       .ValidateMemberList(MemberList.Destination)
       .ReverseMap()
       .ForMember(dest => dest.ecer_PostSecondaryInstituteId, opt => opt.MapFrom(src => src.Id))
       .ForMember(dest => dest.ecer_Name, opt => opt.MapFrom(src => src.Name));

    CreateMap<ecer_identificationtype, IdentificationType>(MemberList.Source)
        .ForCtorParam(nameof(IdentificationType.Id), opt => opt.MapFrom(src => src.ecer_identificationtypeId))
        .ForCtorParam(nameof(IdentificationType.Name), opt => opt.MapFrom(src => src.ecer_Name))
        .ForCtorParam(nameof(IdentificationType.ForPrimary), opt => opt.MapFrom(src => src.ecer_ForPrimary))
        .ForCtorParam(nameof(IdentificationType.ForSecondary), opt => opt.MapFrom(src => src.ecer_ForSecondary))
        .ValidateMemberList(MemberList.Destination)
        .ReverseMap()
        .ForMember(dest => dest.ecer_identificationtypeId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.ecer_Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.ecer_ForPrimary, opt => opt.MapFrom(src => src.ForPrimary))
        .ForMember(dest => dest.ecer_ForSecondary, opt => opt.MapFrom(src => src.ForSecondary));

    CreateMap<PortalTags, ecer_PortalTags>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();

    CreateMap<ecer_outofprovincecertificationtype, OutOfProvinceCertificationType>(MemberList.Source)
        .ForCtorParam(nameof(ecer_outofprovincecertificationtype.Id), opt => opt.MapFrom(src => src.Id))
        .ForMember(d => d.CertificationType, opts => opts.MapFrom(s => s.ecer_certificationtype))
        .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_certificationcomparison, CertificationComparison>(MemberList.Source)
        .ForCtorParam(nameof(ecer_certificationcomparison.Id), opt => opt.MapFrom(src => src.Id))
        .ForMember(d => d.BcCertificate, opts => opts.MapFrom(s => s.ecer_bccertificate))
        .ForMember(d => d.TransferringCertificate, opts => opts.MapFrom(s => s.ecer_certificationcomparisontransferringcertificate))
        .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_DefaultContents, DefaultContent>(MemberList.Source)
        .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
        .ForMember(d => d.SingleText, opts => opts.MapFrom(s => s.ecer_SingleLineofText))
        .ForMember(d => d.MultiText, opts => opts.MapFrom(s => s.ecer_MultipleLineofText))
        .ValidateMemberList(MemberList.Destination);

    CreateMap<bcgov_config, DynamicsConfig>(MemberList.Source)
        .ForCtorParam(nameof(DynamicsConfig.Key), opts => opts.MapFrom(s => s.bcgov_Key))
        .ForCtorParam(nameof(DynamicsConfig.Value), opts => opts.MapFrom(s => s.bcgov_Value))
        .ValidateMemberList(MemberList.Destination);
  }
}
