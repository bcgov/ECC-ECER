import type { OpenAPIClient, Parameters, UnknownParamsObject, OperationResponse, AxiosRequestConfig } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
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
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
      idp?: string | null;
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
     * User profile information
     */
    export interface PspUserProfile {
      id?: string | null;
      firstName?: string | null;
      lastName?: string | null;
      email?: string | null;
      bceidBusinessId?: string | null;
      programRepresentativeId?: string | null;
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
  namespace ProgramRepresentativePost {
    export type RequestBody = /* User profile information */ Components.Schemas.PspUserProfile;
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
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
   * programRepresentative_post - Create Program Representative
   */
  "programRepresentative_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProgramRepresentativePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ProgramRepresentativePost.Responses.$200>;
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
  ["/api/users/register"]: {
    /**
     * programRepresentative_post - Create Program Representative
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProgramRepresentativePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ProgramRepresentativePost.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;

export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type HttpValidationProblemDetails = Components.Schemas.HttpValidationProblemDetails;
export type OidcAuthenticationSettings = Components.Schemas.OidcAuthenticationSettings;
export type ProblemDetails = Components.Schemas.ProblemDetails;
export type PspUserProfile = Components.Schemas.PspUserProfile;
export type VersionMetadata = Components.Schemas.VersionMetadata;
