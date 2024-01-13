import type {
  OpenAPIClient,
  Parameters,
  UnknownParamsObject,
  OperationResponse,
  AxiosRequestConfig,
} from 'openapi-client-axios';

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
        /**
         * New application request
         */
        export interface NewApplicationRequest {
        }
        /**
         * New application response
         */
        export interface NewApplicationResponse {
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
    namespace ApiUserinfoProfile {
        namespace Post {
            export type RequestBody = /* New user request */ Components.Schemas.NewUserRequest;
            namespace Responses {
                export interface $200 {
                }
            }
        }
    }
    namespace Configuration {
        namespace Responses {
            export type $200 = Components.Schemas.ApplicationConfiguration;
        }
    }
    namespace GetApplications {
        namespace Responses {
            export type $200 = /* Application query response */ Components.Schemas.ApplicationQueryResponse;
        }
    }
    namespace GetUserInfo {
        namespace Responses {
            export type $200 = /* User profile information response */ Components.Schemas.UserInfoResponse;
            export interface $404 {
            }
        }
    }
    namespace PostNewApplication {
        export type RequestBody = /* New application request */ Components.Schemas.NewApplicationRequest;
        namespace Responses {
            export type $200 = /* New application response */ Components.Schemas.NewApplicationResponse;
        }
    }
}

export interface OperationMethods {
  /**
   * configuration - Frontend Configuration
   * 
   * Frontend Configuration endpoint
   */
  'configuration'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.Configuration.Responses.$200>
  /**
   * GetUserInfo - Get user profile information
   * 
   * Gets the current user profile information
   */
  'GetUserInfo'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.GetUserInfo.Responses.$200>
  /**
   * GetApplications - Query applications
   * 
   * Handles application queries
   */
  'GetApplications'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.GetApplications.Responses.$200>
  /**
   * PostNewApplication - New Application Submission
   * 
   * Handles  a new application submission to ECER
   */
  'PostNewApplication'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PostNewApplication.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PostNewApplication.Responses.$200>
}

export interface PathsDictionary {
  ['/api/configuration']: {
    /**
     * configuration - Frontend Configuration
     * 
     * Frontend Configuration endpoint
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.Configuration.Responses.$200>
  }
  ['/api/userinfo']: {
    /**
     * GetUserInfo - Get user profile information
     * 
     * Gets the current user profile information
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.GetUserInfo.Responses.$200>
  }
  ['/api/userinfo/profile']: {
  }
  ['/api/applications']: {
    /**
     * PostNewApplication - New Application Submission
     * 
     * Handles  a new application submission to ECER
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PostNewApplication.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PostNewApplication.Responses.$200>
    /**
     * GetApplications - Query applications
     * 
     * Handles application queries
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.GetApplications.Responses.$200>
  }
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>
