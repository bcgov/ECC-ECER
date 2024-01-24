import type { AxiosRequestConfig, OpenAPIClient, OperationResponse, Parameters, UnknownParamsObject } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    /**
     * Address
     */
    export interface Address {
      /**
       *
       */
      line1?: string | null;
      /**
       *
       */
      line2?: string | null;
      /**
       *
       */
      city?: string | null;
      /**
       *
       */
      postalCode?: string | null;
      /**
       *
       */
      province?: string | null;
      /**
       *
       */
      country?: string | null;
    }
    export interface Application {
      id?: string | null;
      submittedOn?: string; // date-time
      registrantId?: string | null;
    }
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
    }
    /**
     * Application query response
     */
    export interface ApplicationQueryResponse {
      /**
       * The items in the response
       */
      items?: Application[] | null;
    }
    export type CertificationType = 0 | 1; // int32
    /**
     * New application request
     */
    export interface DraftApplication {
      id?: string | null;
      certificationTypes?: CertificationType /* int32 */[] | null;
    }
    /**
     * New application response
     */
    export interface DraftApplicationResponse {
      /**
       * The new application id
       */
      applicationId?: string | null;
    }
    /**
     * New user request
     */
    export interface NewUserRequest {
      profile: /* User profile information */ UserProfile;
    }
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
    }
    /**
     * User profile information response
     */
    export interface UserInfoResponse {
      userInfo?: /* User profile information */ UserProfile;
    }
    /**
     * User profile information
     */
    export interface UserProfile {
      /**
       * First name
       */
      firstName?: string | null;
      /**
       * Last name
       */
      lastName?: string | null;
      /**
       * Date of birth in the form of yyyy-MM-dd
       */
      dateOfBirth?: string | null; // date
      /**
       * Email address
       */
      email?: string | null;
      /**
       * Phone number
       */
      phone?: string | null;
      homeAddress?: /* Address */ Address;
      mailingAddress?: /* Address */ Address;
    }
  }
}
declare namespace Paths {
  namespace ApplicationGet {
    namespace Responses {
      export type $200 = /* Application query response */ Components.Schemas.ApplicationQueryResponse;
    }
  }
  namespace ApplicationPost {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id: Parameters.Id;
    }
    namespace Responses {
      export type $200 = /* New application response */ Components.Schemas.DraftApplicationResponse;
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
    export type RequestBody = /* New application request */ Components.Schemas.DraftApplication;
    namespace Responses {
      export type $200 = /* New application response */ Components.Schemas.DraftApplicationResponse;
    }
  }
  namespace ProfilePost {
    export type RequestBody = /* New user request */ Components.Schemas.NewUserRequest;
    namespace Responses {
      export interface $200 {}
    }
  }
  namespace UserinfoGet {
    namespace Responses {
      export type $200 = /* User profile information response */ Components.Schemas.UserInfoResponse;
      export interface $404 {}
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
   * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
   */
  "userinfo_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.UserinfoGet.Responses.$200>;
  /**
   * profile_post - Creates or updates the currently logged on user's profile
   */
  "profile_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProfilePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ProfilePost.Responses.$200>;
  /**
   * draftapplication_put - Handles  a new application submission to ECER
   */
  "draftapplication_put"(
    parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
    data?: Paths.DraftapplicationPut.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>;
  /**
   * application_post - Handles a new application submission to ECER
   */
  "application_post"(
    parameters?: Parameters<Paths.ApplicationPost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationPost.Responses.$200>;
  /**
   * application_get - Handles application queries
   */
  "application_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
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
  ["/api/userinfo"]: {
    /**
     * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.UserinfoGet.Responses.$200>;
  };
  ["/api/userinfo/profile"]: {
    /**
     * profile_post - Creates or updates the currently logged on user's profile
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProfilePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ProfilePost.Responses.$200>;
  };
  ["/api/draftapplications/{id}"]: {
    /**
     * draftapplication_put - Handles  a new application submission to ECER
     */
    "put"(
      parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
      data?: Paths.DraftapplicationPut.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>;
  };
  ["/api/applications/{id}"]: {
    /**
     * application_post - Handles a new application submission to ECER
     */
    "post"(
      parameters?: Parameters<Paths.ApplicationPost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationPost.Responses.$200>;
  };
  ["/api/applications"]: {
    /**
     * application_get - Handles application queries
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.ApplicationGet.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;
