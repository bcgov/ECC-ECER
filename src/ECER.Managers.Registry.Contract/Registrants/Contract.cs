﻿using ECER.Utilities.Security;
using MediatR;

namespace ECER.Managers.Registry.Contract.Registrants;

/// <summary>
/// Invokes a new registrant registration use case
/// </summary>
public record RegisterNewUserCommand(UserProfile Profile, UserIdentity Identity) : IRequest<string>;

public interface IRegistrationIdentityService
{
  Task<string> Resolve(RegisterNewUserCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Invokes updating a registrant's profile use case
/// </summary>
/// <param name="Registrant"></param>
public record UpdateRegistrantProfileCommand(Registrant Registrant) : IRequest<string>;

public record UpdateRegistrantProfileIdentificationCommand(ProfileIdentification Identification) : IRequest<string>;

/// <summary>
/// Invokes a registrant query use case
/// </summary>
public record SearchRegistrantQuery : IRequest<RegistrantQueryResults>
{
  public UserIdentity? ByUserIdentity { get; set; }
}

/// <summary>
/// Container for <see cref="SearchRegistrantQuery"/> results
/// </summary>
public record RegistrantQueryResults(IEnumerable<Registrant> Items);

public record Registrant(string UserId, UserProfile Profile);

public record UserProfile
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? MiddleName { get; set; }
  public string? GivenName { get; set; }
  public string? PreferredName { get; set; }
  public string? AlternateContactPhone { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? RegistrationNumber { get; set; }
  public string Email { get; set; } = null!;
  public string Phone { get; set; } = null!;
  public Address? ResidentialAddress { get; set; }
  public Address? MailingAddress { get; set; }
  public bool IsVerified { get; set; }
  public IEnumerable<PreviousName> PreviousNames { get; set; } = Array.Empty<PreviousName>();
  public bool IsRegistrant { get; set; }
  public StatusCode Status { get; set; }
};

public record PreviousName(string FirstName, string LastName)
{
  public string? Id { get; set; }
  public string? MiddleName { get; set; }
  public string? PreferredName { get; set; }
  public PreviousNameStage? Status { get; set; }
  public PreviousNameSources? Source { get; set; }
  public IEnumerable<IdentityDocument> Documents { get; set; } = Array.Empty<IdentityDocument>();
}

public enum PreviousNameStage
{
  Archived,
  PendingforDocuments,
  ReadyforVerification,
  Unverified,
  Verified,
}

public enum PreviousNameSources
{
  NameLog,
  Profile,
  Transcript,
  OutofProvinceCertificate
}

public record IdentityDocument(string Id)
{
  public string Url { get; set; } = null!;
  public string Extention { get; set; } = null!;
  public string Name { get; set; } = null!;
  public string Size { get; set; } = null!;
}

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );

public enum StatusCode
{
  Inactive,
  PendingforDocuments,
  ReadyforIDVerification,
  ReadyforRegistrantMatch,
  Unverified,
  Verified,
}

public record ProfileIdentification()
{
  public string RegistrantId { get; set; } = null!;
  public string PrimaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> PrimaryIds { get; set; } = Array.Empty<IdentityDocument>();
  public string SecondaryIdTypeObjectId { get; set; } = null!;
  public IEnumerable<IdentityDocument> SecondaryIds { get; set; } = Array.Empty<IdentityDocument>();
}
