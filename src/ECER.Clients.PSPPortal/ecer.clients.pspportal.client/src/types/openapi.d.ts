import type { OpenAPIClient, Parameters, UnknownParamsObject, OperationResponse, AxiosRequestConfig } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
    }
    export type Auspice = "ContinuingEducation" | "PublicOOP" | "Private" | "Public";
    export interface EducationInstitution {
      id?: string | null;
      name?: string | null;
      auspice?: Auspice;
      websiteUrl?: string | null;
      street1?: string | null;
      street2?: string | null;
      street3?: string | null;
      city?: string | null;
      province?: string | null;
      country?: string | null;
      postalCode?: string | null;
    }
    export interface HttpValidationProblemDetails {
      [name: string]: any;
      type?: string | null;
      title?: string | null;
      status?: number | null; // int32
      detail?: string | null;
      instance?: string | null;
      errors?: {
        [name: string]: string[];
      } | null;
    }
    export type InviteType = "PSIProgramRepresentative";
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
      idp?: string | null;
    }
    export type PortalAccessStatus = "Invited" | "Active" | "Disabled";
    export interface PortalInvitation {
      id?: string | null;
      pspProgramRepresentativeId?: string | null;
      inviteType?: InviteType;
    }
    export interface PortalInvitationQueryResult {
      portalInvitation?: PortalInvitation;
    }
    export interface ProblemDetails {
      [name: string]: any;
      type?: string | null;
      title?: string | null;
      status?: number | null; // int32
      detail?: string | null;
      instance?: string | null;
    }
    /**
     * Error codes for PSP user registration failures
     */
    export type PspRegistrationError =
      | "PostSecondaryInstitutionNotFound"
      | "PortalInvitationTokenInvalid"
      | "PortalInvitationWrongStatus"
      | "BceidBusinessIdDoesNotMatch"
      | "GenericError";
    /**
     * Error response for PSP user registration failures. Returns only the error code for frontend handling.
     */
    export interface PspRegistrationErrorResponse {
      errorCode?: /* Error codes for PSP user registration failures */ PspRegistrationError;
    }
    export interface PspUserListItem {
      id?: string | null;
      profile?: /* User profile information */ PspUserProfile;
      accessToPortal?: PortalAccessStatus;
      postSecondaryInstituteId?: string | null;
    }
    /**
     * User profile information
     */
    export interface PspUserProfile {
      id?: string | null;
      firstName?: string | null;
      lastName?: string | null;
      preferredName?: string | null;
      phone?: string | null;
      phoneExtension?: string | null;
      jobTitle?: string | null;
      role?: /* Role of the PSP user */ PspUserRole;
      email?: string | null;
      hasAcceptedTermsOfUse?: boolean | null;
    }
    /**
     * Role of the PSP user
     */
    export type PspUserRole = "Primary" | "Secondary";
    /**
     * Request to register a new psp user
     */
    export interface RegisterPspUserRequest {
      token?: string | null;
      programRepresentativeId?: string | null;
      bceidBusinessId?: string | null;
      profile: /* User profile information */ PspUserProfile;
    }
    export interface VersionMetadata {
      version?: string | null;
      timestamp?: string | null;
      commit?: string | null;
    }
  }
}
declare namespace Paths {
  namespace ConfigurationGet {
    namespace Responses {
      export type $200 = Components.Schemas.ApplicationConfiguration;
    }
  }
  namespace EducationInstitutionGet {
    namespace Responses {
      export type $200 = Components.Schemas.EducationInstitution;
      export interface $404 {}
    }
  }
  namespace PortalInvitationGet {
    namespace Parameters {
      export type Token = string;
    }
    export interface PathParameters {
      token?: Parameters.Token;
    }
    namespace Responses {
      export type $200 = Components.Schemas.PortalInvitationQueryResult;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace PspUserManageDeactivatePost {
    namespace Parameters {
      export type ProgramRepId = string;
    }
    export interface PathParameters {
      programRepId: Parameters.ProgramRepId;
    }
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace PspUserManageReactivatePost {
    namespace Parameters {
      export type ProgramRepId = string;
    }
    export interface PathParameters {
      programRepId: Parameters.ProgramRepId;
    }
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace PspUserManageGet {
    namespace Responses {
      export type $200 = Components.Schemas.PspUserListItem[];
      export interface $404 {}
    }
  }
  namespace PspUserManageSetPrimaryPost {
    namespace Parameters {
      export type ProgramRepId = string;
    }
    export interface PathParameters {
      programRepId: Parameters.ProgramRepId;
    }
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace PspUserProfileGet {
    namespace Responses {
      export type $200 = /* User profile information */ Components.Schemas.PspUserProfile;
      export interface $404 {}
    }
  }
  namespace PspUserProfilePut {
    export type RequestBody = /* User profile information */ Components.Schemas.PspUserProfile;
    namespace Responses {
      export interface $200 {}
    }
  }
  namespace PspUserRegisterPost {
    export type RequestBody = /* Request to register a new psp user */ Components.Schemas.RegisterPspUserRequest;
    namespace Responses {
      export interface $200 {}
      export type $400 =
        /* Error response for PSP user registration failures. Returns only the error code for frontend handling. */ Components.Schemas.PspRegistrationErrorResponse;
    }
  }
  namespace VersionGet {
    namespace Responses {
      export type $200 = Components.Schemas.VersionMetadata;
    }
  }
}

export interface OperationMethods {
  /**
   * configuration_get - Returns the UI initial configuration
   */
  "configuration_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ConfigurationGet.Responses.$200>;
  /**
   * version_get - Returns the version information
   */
  "version_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.VersionGet.Responses.$200>;
  /**
   * psp_user_manage_get - Gets PSP representatives for the current user's institution
   */
  "psp_user_manage_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserManageGet.Responses.$200>;
  /**
   * psp_user_manage_deactivate_post - Deactivates a PSP representative for the current user's institution
   */
  "psp_user_manage_deactivate_post"(
    parameters?: Parameters<Paths.PspUserManageDeactivatePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserManageDeactivatePost.Responses.$200>;
  /**
   * psp_user_manage_reactivate_post - Reactivates a PSP representative for the current user's institution
   */
  "psp_user_manage_reactivate_post"(
    parameters?: Parameters<Paths.PspUserManageReactivatePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserManageReactivatePost.Responses.$200>;
  /**
   * psp_user_manage_set_primary_post - Sets the specified PSP representative as Primary for the current user's institution
   */
  "psp_user_manage_set_primary_post"(
    parameters?: Parameters<Paths.PspUserManageSetPrimaryPost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserManageSetPrimaryPost.Responses.$200>;
  /**
   * psp_user_profile_get - Gets the currently logged in user profile or NotFound if no profile found
   */
  "psp_user_profile_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserProfileGet.Responses.$200>;
  /**
   * psp_user_profile_put - Updates the currently logged in users profile
   */
  "psp_user_profile_put"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserProfilePut.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserProfilePut.Responses.$200>;
  /**
   * psp_user_register_post - Update a psp user profile
   */
  "psp_user_register_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserRegisterPost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserRegisterPost.Responses.$200>;
  /**
   * portal_invitation_get - Handles portal invitation queries
   */
  "portal_invitation_get"(
    parameters?: Parameters<Paths.PortalInvitationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PortalInvitationGet.Responses.$200>;
  /**
   * education_institution_get - Get users education institution
   */
  "education_institution_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.EducationInstitutionGet.Responses.$200>;
}

export interface PathsDictionary {
  ["/api/configuration"]: {
    /**
     * configuration_get - Returns the UI initial configuration
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ConfigurationGet.Responses.$200>;
  };
  ["/api/version"]: {
    /**
     * version_get - Returns the version information
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.VersionGet.Responses.$200>;
  };
  ["/api/users/manage"]: {
    /**
     * psp_user_manage_get - Gets PSP representatives for the current user's institution
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserManageGet.Responses.$200>;
  };
  ["/api/users/manage/{programRepId}/deactivate"]: {
    /**
     * psp_user_manage_deactivate_post - Deactivates a PSP representative for the current user's institution
     */
    "post"(
      parameters?: Parameters<Paths.PspUserManageDeactivatePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserManageDeactivatePost.Responses.$200>;
  };
  ["/api/users/manage/{programRepId}/reactivate"]: {
    /**
     * psp_user_manage_reactivate_post - Reactivates a PSP representative for the current user's institution
     */
    "post"(
      parameters?: Parameters<Paths.PspUserManageReactivatePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserManageReactivatePost.Responses.$200>;
  };
  ["/api/users/manage/{programRepId}/set-primary"]: {
    /**
     * psp_user_manage_set_primary_post - Sets the specified PSP representative as Primary for the current user's institution
     */
    "post"(
      parameters?: Parameters<Paths.PspUserManageSetPrimaryPost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserManageSetPrimaryPost.Responses.$200>;
  };
  ["/api/users/profile"]: {
    /**
     * psp_user_profile_get - Gets the currently logged in user profile or NotFound if no profile found
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserProfileGet.Responses.$200>;
    /**
     * psp_user_profile_put - Updates the currently logged in users profile
     */
    "put"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserProfilePut.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserProfilePut.Responses.$200>;
  };
  ["/api/users/register"]: {
    /**
     * psp_user_register_post - Update a psp user profile
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserRegisterPost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserRegisterPost.Responses.$200>;
  };
  ["/api/PortalInvitations/{token}"]: {
    /**
     * portal_invitation_get - Handles portal invitation queries
     */
    "get"(
      parameters?: Parameters<Paths.PortalInvitationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PortalInvitationGet.Responses.$200>;
  };
  ["/api/education-institution"]: {
    /**
     * education_institution_get - Get users education institution
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.EducationInstitutionGet.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;

export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type Auspice = Components.Schemas.Auspice;
export type EducationInstitution = Components.Schemas.EducationInstitution;
export type HttpValidationProblemDetails = Components.Schemas.HttpValidationProblemDetails;
export type InviteType = Components.Schemas.InviteType;
export type OidcAuthenticationSettings = Components.Schemas.OidcAuthenticationSettings;
export type PortalAccessStatus = Components.Schemas.PortalAccessStatus;
export type PortalInvitation = Components.Schemas.PortalInvitation;
export type PortalInvitationQueryResult = Components.Schemas.PortalInvitationQueryResult;
export type ProblemDetails = Components.Schemas.ProblemDetails;
export type PspRegistrationError = Components.Schemas.PspRegistrationError;
export type PspRegistrationErrorResponse = Components.Schemas.PspRegistrationErrorResponse;
export type PspUserListItem = Components.Schemas.PspUserListItem;
export type PspUserProfile = Components.Schemas.PspUserProfile;
export type PspUserRole = Components.Schemas.PspUserRole;
export type RegisterPspUserRequest = Components.Schemas.RegisterPspUserRequest;
export type VersionMetadata = Components.Schemas.VersionMetadata;
