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
      status?: ApplicationStatus;
      stage?: PortalStage;
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
      | "ReviewforCompletness"
      | "ReadyforAssessment"
      | "BeingAssessed"
      | "Reconsideration"
      | "Appeal"
      | "Cancelled";
    /**
     * Submit application request
     */
    export interface ApplicationSubmissionRequest {
      /**
       * The application id
       */
      id?: string | null;
    }
    export type CertificationType = "EceAssistant" | "OneYear" | "FiveYears" | "Ite" | "Sne";
    export interface Communication {
      id?: string | null;
      subject?: string | null;
      text?: string | null;
    }
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
      stage?: PortalStage;
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
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
    }
    export type PortalStage = "WorkReferences" | "CertificationType" | "ContactInformation" | "Education" | "CharacterReferences" | "Review" | "Declaration";
    /**
     * Save draft application request
     */
    export interface SaveDraftApplicationRequest {
      draftApplication?: DraftApplication;
    }
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
  }
}
declare namespace Paths {
  namespace ApplicationGet {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id?: Parameters.Id;
    }
    namespace Responses {
      export type $200 = Components.Schemas.Application[];
    }
  }
  namespace ApplicationPost {
    export type RequestBody = /* Submit application request */ Components.Schemas.ApplicationSubmissionRequest;
    namespace Responses {
      export interface $200 {}
    }
  }
  namespace ConfigurationGet {
    namespace Responses {
      export type $200 = Components.Schemas.ApplicationConfiguration;
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
      export type $400 = string;
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
    parameters?: Parameters<Paths.ApplicationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationGet.Responses.$200>;
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
      parameters?: Parameters<Paths.ApplicationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationGet.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;
