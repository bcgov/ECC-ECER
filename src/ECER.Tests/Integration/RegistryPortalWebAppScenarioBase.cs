﻿using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Client;
using System.Globalization;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("RegistryPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<RegistryPortalWebAppFixture>;

[Collection("RegistryPortalWebAppScenario")]
public abstract class RegistryPortalWebAppScenarioBase : WebAppScenarioBase
{
  protected new RegistryPortalWebAppFixture Fixture => (RegistryPortalWebAppFixture)base.Fixture;

  protected RegistryPortalWebAppScenarioBase(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class RegistryPortalWebAppFixture : WebAppFixtureBase
{
  private Contact authenticatedBcscUser = null!;
  private ecer_Application testApplication = null!;
  private ecer_Communication testCommunication = null!;

  private Contact authenticatedBcscUser2 = null!;
  private ecer_Application testApplication2 = null!;

  public UserIdentity AuthenticatedBcscUserIdentity => authenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId => authenticatedBcscUser.Id.ToString();
  public string communicationId => testCommunication.Id.ToString();
  public string applicationId => testApplication.Id.ToString();

  public UserIdentity AuthenticatedBcscUserIdentity2 => authenticatedBcscUser2.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
  public string AuthenticatedBcscUserId2 => authenticatedBcscUser2.Id.ToString();
  public string applicationId2 => testApplication2.Id.ToString();
  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("registry_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("registry_new_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_new_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("registry_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.RegistryPortal.Server.Program>();
    using var scope = Host.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EcerContext>();
    await InitializeDataverseTestData(context);
  }

  private async Task InitializeDataverseTestData(EcerContext context)
  {
    await Task.CompletedTask;

    authenticatedBcscUser = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user1");
    testApplication = GetOrAddApplication(context, authenticatedBcscUser);
    testCommunication = GetOrAddCommunication(context, testApplication);

    context.SaveChanges();

    //load dependent properties
    context.Attach(authenticatedBcscUser);
    context.LoadProperty(authenticatedBcscUser, Contact.Fields.ecer_contact_ecer_authentication_455);

    //load user 2
    authenticatedBcscUser2 = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user2");
    testApplication2 = GetOrAddApplication(context, authenticatedBcscUser2);

    context.SaveChanges();

    //load dependent properties
    context.Attach(authenticatedBcscUser2);
    context.LoadProperty(authenticatedBcscUser2, Contact.Fields.ecer_contact_ecer_authentication_455);
  }

  private Contact GetOrAddApplicant(EcerContext context, string identityProvider, string userId)
  {
    var contact = (from a in context.ecer_AuthenticationSet
                   join c in context.ContactSet on a.ecer_authentication_Contactid.ContactId equals c.ContactId into contacts
                   from c in contacts.DefaultIfEmpty()
                   where a.ecer_IdentityProvider == identityProvider && a.ecer_ExternalID == userId
                   select c).SingleOrDefault();

    if (contact == null)
    {
      contact = new Contact
      {
        FirstName = "test1",
        LastName = "test1",
        BirthDate = DateTime.Parse("2000-03-15", CultureInfo.InvariantCulture),
      };

      var authentication = new ecer_Authentication
      {
        ecer_IdentityProvider = identityProvider,
        ecer_ExternalID = userId
      };
      context.AddObject(authentication);
      context.AddObject(contact);
      context.AddLink(authentication, ecer_Authentication.Fields.ecer_contact_ecer_authentication_455, contact);
    }

    return contact;
  }

  private ecer_Application GetOrAddApplication(EcerContext context, Contact applicant)
  {
    var application = (from a in context.ecer_ApplicationSet
                       where a.ecer_Applicantid.Id == applicant.Id
                       select a).FirstOrDefault();

    if (application == null)
    {
      application = new ecer_Application
      {
        Id = Guid.NewGuid(),
      };
      context.AddObject(application);
      context.AddLink(application, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    return application;
  }

  private ecer_Communication GetOrAddCommunication(EcerContext context, ecer_Application application)
  {
    var communication = (from a in context.ecer_CommunicationSet
                         where a.ecer_Applicationid.Id == application.Id
                         select a).FirstOrDefault();

    if (communication == null)
    {
      communication = new ecer_Communication
      {
        Id = Guid.NewGuid(),
        ecer_Message = "Test message",
        ecer_Acknowledged = false,
        StatusCode = ecer_Communication_StatusCode.NotifiedRecipient,
      };

      context.AddObject(communication);
      context.AddLink(communication, ecer_Communication.Fields.ecer_communication_Applicationid, testApplication);
    }

    return communication;
  }
}
