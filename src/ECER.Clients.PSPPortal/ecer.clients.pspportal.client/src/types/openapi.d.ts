import type {
  OpenAPIClient,
  Parameters,
  UnknownParamsObject,
  OperationResponse,
  AxiosRequestConfig,
} from 'openapi-client-axios';

declare namespace Components {
    namespace Schemas {
        export interface VersionMetadata {
            version?: string | null;
            timestamp?: string | null;
            commit?: string | null;
        }
    }
}
declare namespace Paths {
    namespace VersionGet {
        namespace Responses {
            export type $200 = Components.Schemas.VersionMetadata;
        }
    }
}


export interface OperationMethods {
  /**
   * version_get - Returns the version information
   */
  'version_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.VersionGet.Responses.$200>
}

export interface PathsDictionary {
  ['/api/version']: {
    /**
     * version_get - Returns the version information
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.VersionGet.Responses.$200>
  }
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>


export type VersionMetadata = Components.Schemas.VersionMetadata;
