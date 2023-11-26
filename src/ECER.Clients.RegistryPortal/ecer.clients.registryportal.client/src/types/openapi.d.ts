import type {
  AxiosRequestConfig,
  OpenAPIClient,
  OperationResponse,
  Parameters,
  UnknownParamsObject,
} from "openapi-client-axios";

declare namespace Components {
  namespace Schemas {
    export interface Application {
      id?: string | null;
      submittedOn?: string; // date-time
      registrantId?: string | null;
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
    export interface NewApplicationRequest {}
    /**
     * New application response
     */
    export interface NewApplicationResponse {
      /**
       * The new application id
       */
      applicationId?: string | null;
    }
  }
}
declare namespace Paths {
  namespace GetApplications {
    namespace Responses {
      export type $200 =
        /* Application query response */ Components.Schemas.ApplicationQueryResponse;
    }
  }
  namespace PostNewApplication {
    export type RequestBody =
      /* New application request */ Components.Schemas.NewApplicationRequest;
    namespace Responses {
      export type $200 =
        /* New application response */ Components.Schemas.NewApplicationResponse;
    }
  }
}

export interface OperationMethods {
  /**
   * GetApplications - Query applications
   *
   * Handles application queries
   */
  "GetApplications"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.GetApplications.Responses.$200>;
  /**
   * PostNewApplication - New Application Submission
   *
   * Handles  a new application submission to ECER
   */
  "PostNewApplication"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PostNewApplication.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.PostNewApplication.Responses.$200>;
}

export interface PathsDictionary {
  ["/api/applications"]: {
    /**
     * PostNewApplication - New Application Submission
     *
     * Handles  a new application submission to ECER
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PostNewApplication.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.PostNewApplication.Responses.$200>;
    /**
     * GetApplications - Query applications
     *
     * Handles application queries
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.GetApplications.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;
