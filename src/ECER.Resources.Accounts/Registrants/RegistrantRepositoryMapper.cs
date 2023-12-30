using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepositoryMapper : Profile
{
    public RegistrantRepositoryMapper()
    {
        CreateMap<Contact, Registrant>()
            .ForMember(d => d.Profile, opts => opts.MapFrom(s => s))
            .ForMember(d => d.Identities, opts => opts.MapFrom(s => s.ECER_Authentication_ContactId))
            ;

        CreateMap<Contact, UserProfile>()
            .ForMember(d => d.DateOfBirth, opts => opts.MapFrom(s => DateOnly.FromDateTime(s.Birthdate.GetValueOrDefault())))
            .ReverseMap()
            ;

        CreateMap<ECER_Authentication, UserIdentity>()
            .ForCtorParam(nameof(UserIdentity.Id), opts => opts.MapFrom(s => s.ECER_ExternalId))
            .ForCtorParam(nameof(UserIdentity.IdentityProvider), opts => opts.MapFrom(s => s.ECER_IdentityProvider.GetValueOrDefault()))
            .ForMember(d => d.LastLogin, opts => opts.MapFrom(s => s.ModifiedOn.GetValueOrDefault()))
            .ReverseMap()
            ;
    }
}