import type { OpenAPIClient, Parameters, UnknownParamsObject, OperationResponse, AxiosRequestConfig } from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    export interface ApplicationConfiguration {
      clientAuthenticationMethods?: {
        [name: string]: OidcAuthenticationSettings;
      } | null;
    }
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
      idp?: string | null;
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
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;

export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type OidcAuthenticationSettings = Components.Schemas.OidcAuthenticationSettings;
export type VersionMetadata = Components.Schemas.VersionMetadata;
