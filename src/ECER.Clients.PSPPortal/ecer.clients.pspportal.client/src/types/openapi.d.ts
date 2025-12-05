import type { OpenAPIClient, Parameters, UnknownParamsObject, OperationResponse, AxiosRequestConfig } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
    }
    export type Auspice = "ContinuingEducation" | "PublicOOP" | "Private" | "Public";
    export interface Communication {
      id?: string | null;
      subject?: string | null;
      text?: string | null;
      from?: InitiatedFrom;
      acknowledged?: boolean;
      notifiedOn?: string; // date-time
      status?: CommunicationStatus;
      doNotReply?: boolean;
      latestMessageNotifiedOn?: string | null; // date-time
      isRead?: boolean | null;
      applicationId?: string | null;
      icraEligibilityId?: string | null;
      programRepresentativeId?: string | null;
      documents?: CommunicationDocument[] | null;
    }
    export interface CommunicationDocument {
      id?: string | null;
      url?: string | null;
      extention?: string | null;
      name?: string | null;
      size?: string | null;
    }
    /**
     * Save communication response
     */
    export interface CommunicationResponse {
      /**
       * The communication id
       */
      communicationId?: string | null;
    }
    /**
     * Communication seen request
     */
    export interface CommunicationSeenRequest {
      /**
       * The communication ID
       */
      communicationId?: string | null;
    }
    export type CommunicationStatus = "Draft" | "NotifiedRecipient" | "Acknowledged" | "Inactive";
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
    export interface GetMessagesResponse {
      communications?: Communication[] | null;
      totalMessagesCount?: number; // int32
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
    export type InitiatedFrom = "Investigation" | "PortalUser" | "Registry" | "ProgramRepresentative";
    export type InviteType = "PSIProgramRepresentative";
    export interface NewPspUserResponse {
      id?: string | null;
    }
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
      unreadMessagesCount?: number; // int32
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
    /**
     * Send Message Request
     */
    export interface SendMessageRequest {
      communication?: Communication;
    }
    /**
     * Send Message Response
     */
    export interface SendMessageResponse {
      /**
       *
       */
      communicationId?: string | null;
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
  namespace GetAllMessages {
    namespace Parameters {
      export type ParentId = string;
    }
    export interface PathParameters {
      parentId?: Parameters.ParentId;
    }
    namespace Responses {
      export type $200 = Components.Schemas.GetMessagesResponse;
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace MessageStatusGet {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id?: Parameters.Id;
    }
    export type RequestBody = /* Communication seen request */ Components.Schemas.CommunicationSeenRequest;
    namespace Responses {
      export type $200 = /* Save communication response */ Components.Schemas.CommunicationResponse;
      export type $400 = string;
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
  namespace PostMessageReply {
    export type RequestBody = /* Send Message Request */ Components.Schemas.SendMessageRequest;
    namespace Responses {
      export type $200 = /* Send Message Response */ Components.Schemas.SendMessageResponse;
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace PspUserAdd {
    export type RequestBody = /* User profile information */ Components.Schemas.PspUserProfile;
    namespace Responses {
      export type $200 = Components.Schemas.NewPspUserResponse;
      export type $400 = string;
      export interface $404 {}
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
   * psp_user_manage_set_primary_post - Sets the specified PSP representative as Primary for the current user's institution
   */
  "psp_user_manage_set_primary_post"(
    parameters?: Parameters<Paths.PspUserManageSetPrimaryPost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserManageSetPrimaryPost.Responses.$200>;
  /**
   * psp_user_add - Adds a new psp user to an institution
   */
  "psp_user_add"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserAdd.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PspUserAdd.Responses.$200>;
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
  /**
   * get_all_messages - Paginated endpoint to get all user messages
   */
  "get_all_messages"(
    parameters?: Parameters<Paths.GetAllMessages.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.GetAllMessages.Responses.$200>;
  /**
   * post_message_reply - Endpoint to reply to an existing message
   */
  "post_message_reply"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PostMessageReply.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PostMessageReply.Responses.$200>;
  /**
   * message_status_get - Handles messages status
   */
  "message_status_get"(
    parameters?: Parameters<Paths.MessageStatusGet.PathParameters> | null,
    data?: Paths.MessageStatusGet.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
  /**
   * message_status_get - Handles messages status
   */
  "message_status_get"(
    parameters?: Parameters<Paths.MessageStatusGet.PathParameters> | null,
    data?: Paths.MessageStatusGet.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
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
  ["/api/users/manage/add"]: {
    /**
     * psp_user_add - Adds a new psp user to an institution
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserAdd.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PspUserAdd.Responses.$200>;
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
  ["/api/messages/{parentId}"]: {
    /**
     * get_all_messages - Paginated endpoint to get all user messages
     */
    "get"(
      parameters?: Parameters<Paths.GetAllMessages.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.GetAllMessages.Responses.$200>;
  };
  ["/api/messages"]: {
    /**
     * post_message_reply - Endpoint to reply to an existing message
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PostMessageReply.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PostMessageReply.Responses.$200>;
  };
  ["/api/messages/{id}/seen"]: {
    /**
     * message_status_get - Handles messages status
     */
    "put"(
      parameters?: Parameters<Paths.MessageStatusGet.PathParameters> | null,
      data?: Paths.MessageStatusGet.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
  };
  ["/api/messages/status"]: {
    /**
     * message_status_get - Handles messages status
     */
    "get"(
      parameters?: Parameters<Paths.MessageStatusGet.PathParameters> | null,
      data?: Paths.MessageStatusGet.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;

export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type Auspice = Components.Schemas.Auspice;
export type Communication = Components.Schemas.Communication;
export type CommunicationDocument = Components.Schemas.CommunicationDocument;
export type CommunicationResponse = Components.Schemas.CommunicationResponse;
export type CommunicationSeenRequest = Components.Schemas.CommunicationSeenRequest;
export type CommunicationStatus = Components.Schemas.CommunicationStatus;
export type EducationInstitution = Components.Schemas.EducationInstitution;
export type GetMessagesResponse = Components.Schemas.GetMessagesResponse;
export type HttpValidationProblemDetails = Components.Schemas.HttpValidationProblemDetails;
export type InitiatedFrom = Components.Schemas.InitiatedFrom;
export type InviteType = Components.Schemas.InviteType;
export type NewPspUserResponse = Components.Schemas.NewPspUserResponse;
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
export type SendMessageRequest = Components.Schemas.SendMessageRequest;
export type SendMessageResponse = Components.Schemas.SendMessageResponse;
export type VersionMetadata = Components.Schemas.VersionMetadata;
