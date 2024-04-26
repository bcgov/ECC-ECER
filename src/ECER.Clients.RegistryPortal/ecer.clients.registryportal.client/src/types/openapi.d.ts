import type { AxiosRequestConfig, OpenAPIClient, OperationResponse, Parameters, UnknownParamsObject } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    /**
     * Address
     */
    export interface Address {
      line1?: string | null;
      line2?: string | null;
      city?: string | null;
      postalCode?: string | null;
      province?: string | null;
      country?: string | null;
    }
    export interface Application {
      id?: string | null;
      createdOn?: string; // date-time
      submittedOn?: string | null; // date-time
      signedDate?: string | null; // date-time
      certificationTypes?: CertificationType[] | null;
      transcripts?: Transcript[] | null;
      workExperienceReferences?: WorkExperienceReference[] | null;
      status?: ApplicationStatus;
      stage?: PortalStage;
      characterReferences?: CharacterReference[] | null;
    }
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
    }
    export type ApplicationStatus =
      | "Draft"
      | "Submitted"
      | "Complete"
      | "Reconsideration"
      | "Cancelled"
      | "Escalated"
      | "Decision"
      | "Withdrawn"
      | "Ready"
      | "InProgress"
      | "PendingQueue"
      | "ReconsiderationDecision";
    /**
     * Submit application request
     */
    export interface ApplicationSubmissionRequest {
      /**
       * The application id
       */
      id?: string | null;
    }
    /**
     * delete draft application response
     */
    export interface CancelDraftApplicationResponse {
      /**
       * The application id
       */
      applicationId?: string | null;
    }
    export type CertificationType = "EceAssistant" | "OneYear" | "FiveYears" | "Ite" | "Sne";
    export interface CharacterReference {
      firstName?: string | null;
      lastName?: string | null;
      phoneNumber?: string | null;
      emailAddress?: string | null;
      id?: string | null;
    }
    export interface CharacterReferenceContactInformation {
      lastName?: string | null;
      firstName?: string | null;
      email?: string | null;
      phoneNumber?: string | null;
      certificateNumber?: string | null;
      certificateProvinceId?: string | null;
      certificateProvinceOther?: string | null;
    }
    export interface CharacterReferenceEvaluation {
      relationship?: string | null;
      lengthOfAcquaintance?: string | null;
      workedWithChildren?: boolean;
      childInteractionObservations?: string | null;
      applicantTemperamentAssessment?: string | null;
      applicantShouldNotBeECE?: boolean;
      applicantNotQualifiedReason?: string | null;
    }
    export interface CharacterReferenceSubmissionRequest {
      token?: string | null;
      referenceContactInformation?: CharacterReferenceContactInformation;
      referenceEvaluation?: CharacterReferenceEvaluation;
      responseAccuracyConfirmation?: boolean;
    }
    export interface Communication {
      id?: string | null;
      subject?: string | null;
      text?: string | null;
      acknowledged?: boolean;
      notifiedOn?: string; // date-time
      status?: CommunicationStatus;
    }
    export type CommunicationStatus = "Draft" | "NotifiedRecipient" | "Acknowledged" | "Inactive";
    export interface CommunicationsStatus {
      count?: number; // int32
      hasUnread?: boolean;
    }
    export interface CommunicationsStatusResults {
      status?: CommunicationsStatus;
    }
    export interface DraftApplication {
      id?: string | null;
      signedDate?: string | null; // date-time
      certificationTypes?: CertificationType[] | null;
      transcripts?: Transcript[] | null;
      workExperienceReferences?: WorkExperienceReference[] | null;
      stage?: PortalStage;
      characterReferences?: CharacterReference[] | null;
    }
    /**
     * Save draft application response
     */
    export interface DraftApplicationResponse {
      /**
       * The application id
       */
      applicationId?: string | null;
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
    export type InviteType = "CharacterReference" | "WorkExperienceReference";
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
      idp?: string | null;
    }
    export interface OptOutReferenceRequest {
      token?: string | null;
      unabletoProvideReferenceReasons?: UnabletoProvideReferenceReasons;
    }
    export interface PortalInvitation {
      id?: string | null;
      name?: string | null;
      referenceFirstName?: string | null;
      referenceLastName?: string | null;
      referenceEmailAddress?: string | null;
      applicantFirstName?: string | null;
      applicantLastName?: string | null;
      applicationId?: string | null;
      certificationTypes?: CertificationType[] | null;
      workexperienceReferenceId?: string | null;
      characterReferenceId?: string | null;
      inviteType?: InviteType;
    }
    export interface PortalInvitationQueryResult {
      portalInvitation?: PortalInvitation;
    }
    export type PortalStage = "CertificationType" | "Declaration" | "ContactInformation" | "Education" | "CharacterReferences" | "WorkReferences" | "Review";
    export interface ProblemDetails {
      [name: string]: any;
      type?: string | null;
      title?: string | null;
      status?: number | null; // int32
      detail?: string | null;
      instance?: string | null;
    }
    export interface Province {
      provinceId?: string | null;
      provinceName?: string | null;
    }
    /**
     * Save draft application request
     */
    export interface SaveDraftApplicationRequest {
      draftApplication?: DraftApplication;
    }
    export interface SubmitApplicationResponse {
      applicationId?: string | null;
    }
    export interface Transcript {
      id?: string | null;
      educationalInstitutionName: string;
      programName: string;
      campusLocation?: string | null;
      studentName: string;
      studentNumber: string;
      languageofInstruction?: string | null;
      startDate: string; // date-time
      endDate: string; // date-time
      isECEAssistant?: boolean;
      doesECERegistryHaveTranscript?: boolean;
      isOfficialTranscriptRequested?: boolean;
    }
    export type UnabletoProvideReferenceReasons =
      | "Iamunabletoatthistime"
      | "Idonothavetheinformationrequired"
      | "Idonotknowthisperson"
      | "Idonotmeettherequirementstoprovideareference"
      | "Other";
    export interface UserInfo {
      firstName?: string | null;
      lastName?: string | null;
      dateOfBirth?: string; // date
      email?: string | null;
      phone?: string | null;
      unreadMessagesCount?: number; // int32
    }
    /**
     * User profile information
     */
    export interface UserProfile {
      firstName?: string | null;
      lastName?: string | null;
      middleName?: string | null;
      preferredName?: string | null;
      alternateContactPhone?: string | null;
      dateOfBirth?: string | null; // date
      email?: string | null;
      phone?: string | null;
      residentialAddress?: /* Address */ Address;
      mailingAddress?: /* Address */ Address;
    }
    export interface WorkExperienceReference {
      firstName?: string | null;
      lastName?: string | null;
      emailAddress?: string | null;
      hours?: number | null; // int32
      id?: string | null;
      phoneNumber?: string | null;
    }
  }
}
declare namespace Paths {
  namespace ApplicationGet {
    namespace Parameters {
      export type ByStatus = Components.Schemas.ApplicationStatus[];
      export type Id = string;
    }
    export interface PathParameters {
      id?: Parameters.Id;
    }
    export interface QueryParameters {
      byStatus?: Parameters.ByStatus;
    }
    namespace Responses {
      export type $200 = Components.Schemas.Application[];
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace ApplicationPost {
    export type RequestBody = /* Submit application request */ Components.Schemas.ApplicationSubmissionRequest;
    namespace Responses {
      export type $200 = Components.Schemas.SubmitApplicationResponse;
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
  namespace CharacterReferencePost {
    export type RequestBody = Components.Schemas.CharacterReferenceSubmissionRequest;
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace ConfigurationGet {
    namespace Responses {
      export type $200 = Components.Schemas.ApplicationConfiguration;
    }
  }
  namespace DraftapplicationDelete {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id: Parameters.Id;
    }
    namespace Responses {
      export type $200 = /* delete draft application response */ Components.Schemas.CancelDraftApplicationResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace DraftapplicationPut {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id?: Parameters.Id;
    }
    export type RequestBody = /* Save draft application request */ Components.Schemas.SaveDraftApplicationRequest;
    namespace Responses {
      export type $200 = /* Save draft application response */ Components.Schemas.DraftApplicationResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace MessageGet {
    namespace Responses {
      export type $200 = Components.Schemas.Communication[];
    }
  }
  namespace MessageStatusGet {
    namespace Responses {
      export type $200 = Components.Schemas.CommunicationsStatusResults;
    }
  }
  namespace ProfileGet {
    namespace Responses {
      export type $200 = /* User profile information */ Components.Schemas.UserProfile;
      export interface $404 {}
    }
  }
  namespace ProfilePut {
    export type RequestBody = /* User profile information */ Components.Schemas.UserProfile;
    namespace Responses {
      export interface $200 {}
    }
  }
  namespace ProvinceGet {
    namespace Responses {
      export type $200 = Components.Schemas.Province[];
    }
  }
  namespace ReferenceOptout {
    export type RequestBody = Components.Schemas.OptOutReferenceRequest;
    namespace Responses {
      export interface $200 {}
      export type $400 = string;
    }
  }
  namespace ReferencesGet {
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
  namespace UserinfoGet {
    namespace Responses {
      export type $200 = Components.Schemas.UserInfo;
      export interface $404 {}
    }
  }
  namespace UserinfoPost {
    export type RequestBody = Components.Schemas.UserInfo;
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
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
   * province_get - Handles province queries
   */
  "province_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ProvinceGet.Responses.$200>;
  /**
   * profile_get - Gets the current user profile
   */
  "profile_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ProfileGet.Responses.$200>;
  /**
   * profile_put - Gets the current user profile
   */
  "profile_put"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProfilePut.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ProfilePut.Responses.$200>;
  /**
   * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
   */
  "userinfo_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.UserinfoGet.Responses.$200>;
  /**
   * userinfo_post - Creates or updates the currently logged on user's profile
   */
  "userinfo_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.UserinfoPost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.UserinfoPost.Responses.$200>;
  /**
   * references_get - Handles references queries
   */
  "references_get"(
    parameters?: Parameters<Paths.ReferencesGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ReferencesGet.Responses.$200>;
  /**
   * character_reference_post - Handles character reference submission
   */
  "character_reference_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.CharacterReferencePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.CharacterReferencePost.Responses.$200>;
  /**
   * reference_optout - Handles reference optout
   */
  "reference_optout"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ReferenceOptout.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ReferenceOptout.Responses.$200>;
  /**
   * message_get - Handles messages queries
   */
  "message_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessageGet.Responses.$200>;
  /**
   * message_status_get - Handles messages status
   */
  "message_status_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
  /**
   * draftapplication_put - Save a draft application for the current user
   */
  "draftapplication_put"(
    parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
    data?: Paths.DraftapplicationPut.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>;
  /**
   * application_post - Submit an application
   */
  "application_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ApplicationPost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationPost.Responses.$200>;
  /**
   * application_get - Handles application queries
   */
  "application_get"(
    parameters?: Parameters<Paths.ApplicationGet.PathParameters & Paths.ApplicationGet.QueryParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationGet.Responses.$200>;
  /**
   * draftapplication_delete - Cancel a draft application for the current user
   *
   * Changes status to cancelled
   */
  "draftapplication_delete"(
    parameters?: Parameters<Paths.DraftapplicationDelete.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.DraftapplicationDelete.Responses.$200>;
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
  ["/api/provincelist"]: {
    /**
     * province_get - Handles province queries
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.ProvinceGet.Responses.$200>;
  };
  ["/api/profile"]: {
    /**
     * profile_get - Gets the current user profile
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.ProfileGet.Responses.$200>;
    /**
     * profile_put - Gets the current user profile
     */
    "put"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProfilePut.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ProfilePut.Responses.$200>;
  };
  ["/api/userinfo"]: {
    /**
     * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.UserinfoGet.Responses.$200>;
    /**
     * userinfo_post - Creates or updates the currently logged on user's profile
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.UserinfoPost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.UserinfoPost.Responses.$200>;
  };
  ["/api/PortalInvitations/{token}"]: {
    /**
     * references_get - Handles references queries
     */
    "get"(
      parameters?: Parameters<Paths.ReferencesGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ReferencesGet.Responses.$200>;
  };
  ["/api/References/Character"]: {
    /**
     * character_reference_post - Handles character reference submission
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.CharacterReferencePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.CharacterReferencePost.Responses.$200>;
  };
  ["/api/References/OptOut"]: {
    /**
     * reference_optout - Handles reference optout
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ReferenceOptout.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ReferenceOptout.Responses.$200>;
  };
  ["/api/messages"]: {
    /**
     * message_get - Handles messages queries
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.MessageGet.Responses.$200>;
  };
  ["/api/messages/status"]: {
    /**
     * message_status_get - Handles messages status
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.MessageStatusGet.Responses.$200>;
  };
  ["/api/draftapplications/{id}"]: {
    /**
     * draftapplication_put - Save a draft application for the current user
     */
    "put"(
      parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
      data?: Paths.DraftapplicationPut.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>;
  };
  ["/api/applications"]: {
    /**
     * application_post - Submit an application
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ApplicationPost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationPost.Responses.$200>;
  };
  ["/api/applications/{id}"]: {
    /**
     * application_get - Handles application queries
     */
    "get"(
      parameters?: Parameters<Paths.ApplicationGet.PathParameters & Paths.ApplicationGet.QueryParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationGet.Responses.$200>;
  };
  ["/api/draftApplications/{id}"]: {
    /**
     * draftapplication_delete - Cancel a draft application for the current user
     *
     * Changes status to cancelled
     */
    "delete"(
      parameters?: Parameters<Paths.DraftapplicationDelete.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.DraftapplicationDelete.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;
