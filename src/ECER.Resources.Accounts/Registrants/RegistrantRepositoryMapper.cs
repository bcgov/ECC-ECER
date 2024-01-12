using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepositoryMapper : Profile
{
    public RegistrantRepositoryMapper()
    {
        CreateMap<Contact, Registrant>()
            .ForMember(d => d.Profile, opts => opts.MapFrom(s => s))
            .ForMember(d => d.Identities, opts => opts.MapFrom(s => s.ECER_Contact_ECER_Authentication_455))
            .ReverseMap()
            .ValidateMemberList(MemberList.Source)
            ;

        CreateMap<ECER_Authentication, UserIdentity>()
            .ForCtorParam(nameof(UserIdentity.Id), opts => opts.MapFrom(s => s.ECER_ExternalId))
            .ForCtorParam(nameof(UserIdentity.IdentityProvider), opts => opts.MapFrom(s => s.ECER_IdentityProvider))
            .ForMember(d => d.LastLogin, opts => opts.MapFrom(s => s.ModifiedOn))
            ;

        CreateMap<Contact, UserProfile>()
            .ForMember(d => d.DateOfBirth, opts => opts.MapFrom(s => DateOnly.FromDateTime(s.Birthdate.GetValueOrDefault())))
            .ForMember(d => d.Email, opts => opts.MapFrom(s => s.EmailAddress1))
            .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Address1_Telephone1))
            .ForMember(d => d.HomeAddress, opts => opts.MapFrom(s => s.Address1_Line1 == null ? null : new Address(s.Address1_Line1, s.Address1_Line2, s.Address1_City, s.Address1_PostalCode, s.Address1_StateOrProvince, s.Address1_Country)))
            .ForMember(d => d.MailingAddress, opts => opts.MapFrom(s => s.Address2_Line1 == null ? null : new Address(s.Address2_Line1, s.Address2_Line2, s.Address2_City, s.Address2_PostalCode, s.Address2_StateOrProvince, s.Address2_Country)))
            .ReverseMap()
            //.ValidateMemberList(MemberList.Source)
            .ForMember(d => d.Birthdate, opts => opts.MapFrom(s => s.DateOfBirth == null ? (DateTime?)null : s.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)))
            .ForMember(d => d.Telephone1, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.Phone))
            .ForMember(d => d.EmailAddress1, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.Email))
            .ForMember(d => d.Address1_Line1, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.Line1))
            .ForMember(d => d.Address1_Line2, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.Line2))
            .ForMember(d => d.Address1_City, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.City))
            .ForMember(d => d.Address1_PostalCode, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.PostalCode))
            .ForMember(d => d.Address1_StateOrProvince, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.Province))
            .ForMember(d => d.Address1_Country, opts => opts.MapFrom(s => s.HomeAddress == null ? null : s.HomeAddress.Country))
            .ForMember(d => d.Address2_Line1, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.Line1))
            .ForMember(d => d.Address2_Line2, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.Line2))
            .ForMember(d => d.Address2_City, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.City))
            .ForMember(d => d.Address2_PostalCode, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.PostalCode))
            .ForMember(d => d.Address2_StateOrProvince, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.Province))
            .ForMember(d => d.Address2_Country, opts => opts.MapFrom(s => s.MailingAddress == null ? null : s.MailingAddress.Country))
            ;
    }
}