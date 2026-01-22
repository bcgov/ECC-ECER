import type {
  OpenAPIClient,
  Parameters,
  UnknownParamsObject,
  OperationResponse,
  AxiosRequestConfig,
} from 'openapi-client-axios';

declare namespace Components {
    namespace Schemas {
        export interface ApplicationConfiguration {
            clientAuthenticationMethods?: {
                [name: string]: OidcAuthenticationSettings;
            } | null;
        }
        export interface AreaOfInstruction {
            id?: string | null;
            name?: string | null;
            programTypes?: ProgramTypes[] | null;
            minimumHours?: number | null; // int32
        }
        export interface AreaOfInstructionListResponse {
            areaOfInstruction?: AreaOfInstruction[] | null;
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
            educationInstituteName?: string | null;
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
        export interface CommunicationsStatus {
            count?: number; // int32
            hasUnread?: boolean;
        }
        export interface CommunicationsStatusResults {
            status?: CommunicationsStatus;
        }
        export interface Country {
            countryId?: string | null;
            countryName?: string | null;
            countryCode?: string | null;
            isICRA?: boolean;
        }
        export interface Course {
            courseId: string;
            courseNumber: string;
            courseTitle: string;
            newCourseNumber?: string | null;
            newCourseTitle?: string | null;
            courseAreaOfInstruction?: CourseAreaOfInstruction[] | null;
            programType: string;
        }
        export interface CourseAreaOfInstruction {
            courseAreaOfInstructionId?: string | null;
            newHours?: string | null;
            areaOfInstructionId?: string | null;
        }
        export interface DraftProgramResponse {
            program?: Program;
        }
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
        /**
         * file Response
         */
        export interface FileResponse {
            /**
             *
             */
            fileId?: string | null;
            url?: string | null;
        }
        export interface GetMessagesResponse {
            communications?: Communication[] | null;
            totalMessagesCount?: number; // int32
        }
        export interface GetProgramsResponse {
            programs?: Program[] | null;
            totalProgramsCount?: number; // int32
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
            bceidBusinessName?: string | null;
            isLinked?: boolean;
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
        export interface Program {
            id?: string | null;
            portalStage: string;
            status?: ProgramStatus;
            createdOn?: string | null; // date-time
            name?: string | null;
            postSecondaryInstituteName?: string | null;
            startDate?: string | null; // date-time
            endDate?: string | null; // date-time
            newBasicTotalHours?: string | null;
            newSneTotalHours?: string | null;
            newIteTotalHours?: string | null;
            programProfileType?: ProgramProfileType;
            programTypes?: ProgramTypes[] | null;
            courses?: Course[] | null;
        }
        export type ProgramProfileType = "ChangeRequest" | "AnnualReview";
        export type ProgramStatus = "Draft" | "UnderReview" | "Approved" | "Denied" | "Inactive" | "ChangeRequestInProgress" | "Withdrawn";
        export type ProgramTypes = "Basic" | "SNE" | "ITE";
        export interface Province {
            provinceId?: string | null;
            provinceName?: string | null;
            provinceCode?: string | null;
        }
        /**
         * Error codes for PSP user registration failures
         */
        export type PspRegistrationError = "PostSecondaryInstitutionNotFound" | "PortalInvitationTokenInvalid" | "PortalInvitationWrongStatus" | "BceidBusinessIdDoesNotMatch" | "GenericError";
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
            bceidBusinessName?: string | null;
            profile: /* User profile information */ PspUserProfile;
        }
        export interface SaveDraftProgramRequest {
            program?: Program;
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
        export interface SubmitProgramRequest {
            programId?: string | null;
        }
        export interface UpdateCourseRequest {
            courses?: Course[] | null;
        }
        export interface VersionMetadata {
            version?: string | null;
            timestamp?: string | null;
            commit?: string | null;
        }
    }
}
declare namespace Paths {
    namespace AreaOfInstructionGet {
        namespace Responses {
            export type $200 = Components.Schemas.AreaOfInstructionListResponse;
        }
    }
    namespace CommunicationPut {
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
            export interface $404 {
            }
        }
    }
    namespace ConfigurationGet {
        namespace Responses {
            export type $200 = Components.Schemas.ApplicationConfiguration;
        }
    }
    namespace CountryGet {
        namespace Responses {
            export type $200 = Components.Schemas.Country[];
        }
    }
    namespace CoursePut {
        namespace Parameters {
            export type Id = string;
        }
        export interface PathParameters {
            id: Parameters.Id;
        }
        export type RequestBody = Components.Schemas.UpdateCourseRequest;
        namespace Responses {
            export type $200 = string;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace DeleteFile {
        namespace Parameters {
            export type FileId = string;
        }
        export interface PathParameters {
            fileId: Parameters.FileId;
        }
        namespace Responses {
            export type $200 = /* file Response */ Components.Schemas.FileResponse;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace DraftprogramPut {
        namespace Parameters {
            export type Id = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        export type RequestBody = Components.Schemas.SaveDraftProgramRequest;
        namespace Responses {
            export type $200 = Components.Schemas.DraftProgramResponse;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace EducationInstitutionGet {
        namespace Responses {
            export type $200 = Components.Schemas.EducationInstitution;
            export interface $404 {
            }
        }
    }
    namespace EducationInstitutionPut {
        export type RequestBody = Components.Schemas.EducationInstitution;
        namespace Responses {
            export interface $200 {
            }
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
        }
    }
    namespace FilesCommunicationGet {
        namespace Parameters {
            export type CommunicationId = string;
            export type FileId = string;
        }
        export interface PathParameters {
            communicationId: Parameters.CommunicationId;
            fileId: Parameters.FileId;
        }
        namespace Responses {
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace MessageGet {
        namespace Parameters {
            export type ParentId = string;
        }
        export interface PathParameters {
            parentId?: Parameters.ParentId;
        }
        namespace Responses {
            export type $200 = Components.Schemas.GetMessagesResponse;
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace MessagePost {
        export type RequestBody = /* Send Message Request */ Components.Schemas.SendMessageRequest;
        namespace Responses {
            export type $200 = /* Send Message Response */ Components.Schemas.SendMessageResponse;
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace MessageStatusGet {
        namespace Responses {
            export type $200 = Components.Schemas.CommunicationsStatusResults;
            export interface $404 {
            }
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
    namespace ProgramGet {
        namespace Parameters {
            export type ByStatus = Components.Schemas.ProgramStatus[];
            export type Id = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        export interface QueryParameters {
            byStatus?: Parameters.ByStatus;
        }
        namespace Responses {
            export type $200 = Components.Schemas.GetProgramsResponse;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace ProgramPost {
        export type RequestBody = Components.Schemas.SubmitProgramRequest;
        namespace Responses {
            export type $200 = string;
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace ProgramPut {
        namespace Parameters {
            export type Id = string;
        }
        export interface PathParameters {
            id: Parameters.Id;
        }
        export type RequestBody = Components.Schemas.Program;
        namespace Responses {
            export type $200 = string;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace ProvinceGet {
        namespace Responses {
            export type $200 = Components.Schemas.Province[];
        }
    }
    namespace PspUserAdd {
        export type RequestBody = /* User profile information */ Components.Schemas.PspUserProfile;
        namespace Responses {
            export type $200 = Components.Schemas.NewPspUserResponse;
            export type $400 = string;
            export interface $404 {
            }
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
            export interface $200 {
            }
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace PspUserManageGet {
        namespace Responses {
            export type $200 = Components.Schemas.PspUserListItem[];
            export interface $404 {
            }
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
            export interface $200 {
            }
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
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
            export interface $200 {
            }
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace PspUserProfileGet {
        namespace Responses {
            export type $200 = /* User profile information */ Components.Schemas.PspUserProfile;
            export interface $404 {
            }
        }
    }
    namespace PspUserProfilePut {
        export type RequestBody = /* User profile information */ Components.Schemas.PspUserProfile;
        namespace Responses {
            export interface $200 {
            }
        }
    }
    namespace PspUserRegisterPost {
        export type RequestBody = /* Request to register a new psp user */ Components.Schemas.RegisterPspUserRequest;
        namespace Responses {
            export interface $200 {
            }
            export type $400 = /* Error response for PSP user registration failures. Returns only the error code for frontend handling. */ Components.Schemas.PspRegistrationErrorResponse;
        }
    }
    namespace UploadFile {
        namespace Parameters {
            export type FileId = string;
        }
        export interface PathParameters {
            fileId: Parameters.FileId;
        }
        export interface RequestBody {
            file: string; // binary
        }
        namespace Responses {
            export type $200 = /* file Response */ Components.Schemas.FileResponse;
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
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
  'configuration_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ConfigurationGet.Responses.$200>
  /**
   * province_get - Handles province queries
   */
  'province_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProvinceGet.Responses.$200>
  /**
   * country_get - Handles country queries
   */
  'country_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CountryGet.Responses.$200>
  /**
   * area_of_instruction_get - Handles area of instruction queries
   */
  'area_of_instruction_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.AreaOfInstructionGet.Responses.$200>
  /**
   * version_get - Returns the version information
   */
  'version_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.VersionGet.Responses.$200>
  /**
   * psp_user_manage_get - Gets PSP representatives for the current user's institution
   */
  'psp_user_manage_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserManageGet.Responses.$200>
  /**
   * psp_user_manage_deactivate_post - Deactivates a PSP representative for the current user's institution
   */
  'psp_user_manage_deactivate_post'(
    parameters?: Parameters<Paths.PspUserManageDeactivatePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserManageDeactivatePost.Responses.$200>
  /**
   * psp_user_manage_reactivate_post - Reactivates a PSP representative for the current user's institution
   */
  'psp_user_manage_reactivate_post'(
    parameters?: Parameters<Paths.PspUserManageReactivatePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserManageReactivatePost.Responses.$200>
  /**
   * psp_user_manage_set_primary_post - Sets the specified PSP representative as Primary for the current user's institution
   */
  'psp_user_manage_set_primary_post'(
    parameters?: Parameters<Paths.PspUserManageSetPrimaryPost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserManageSetPrimaryPost.Responses.$200>
  /**
   * psp_user_add - Adds a new psp user to an institution
   */
  'psp_user_add'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserAdd.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserAdd.Responses.$200>
  /**
   * psp_user_profile_get - Gets the currently logged in user profile or NotFound if no profile found
   */
  'psp_user_profile_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserProfileGet.Responses.$200>
  /**
   * psp_user_profile_put - Updates the currently logged in users profile
   */
  'psp_user_profile_put'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserProfilePut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserProfilePut.Responses.$200>
  /**
   * psp_user_register_post - Update a psp user profile
   */
  'psp_user_register_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.PspUserRegisterPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PspUserRegisterPost.Responses.$200>
  /**
   * draftprogram_put - Save a draft program for the current user
   */
  'draftprogram_put'(
    parameters?: Parameters<Paths.DraftprogramPut.PathParameters> | null,
    data?: Paths.DraftprogramPut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.DraftprogramPut.Responses.$200>
  /**
   * program_get - Handles program queries
   */
  'program_get'(
    parameters?: Parameters<Paths.ProgramGet.QueryParameters & Paths.ProgramGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProgramGet.Responses.$200>
  /**
   * course_put - Update a course for a program profile
   */
  'course_put'(
    parameters?: Parameters<Paths.CoursePut.PathParameters> | null,
    data?: Paths.CoursePut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CoursePut.Responses.$200>
  /**
   * program_put - Update program profile
   */
  'program_put'(
    parameters?: Parameters<Paths.ProgramPut.PathParameters> | null,
    data?: Paths.ProgramPut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProgramPut.Responses.$200>
  /**
   * program_post - Submit a draft program profile
   */
  'program_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProgramPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProgramPost.Responses.$200>
  /**
   * portal_invitation_get - Handles portal invitation queries
   */
  'portal_invitation_get'(
    parameters?: Parameters<Paths.PortalInvitationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PortalInvitationGet.Responses.$200>
  /**
   * files_communication_get - Handles fetching files
   */
  'files_communication_get'(
    parameters?: Parameters<Paths.FilesCommunicationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<any>
  /**
   * upload_file - Handles upload file request
   */
  'upload_file'(
    parameters?: Parameters<Paths.UploadFile.PathParameters> | null,
    data?: Paths.UploadFile.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.UploadFile.Responses.$200>
  /**
   * delete_file - Handles delete uploaded file request
   */
  'delete_file'(
    parameters?: Parameters<Paths.DeleteFile.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.DeleteFile.Responses.$200>
  /**
   * education_institution_get - Get users education institution
   */
  'education_institution_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.EducationInstitutionGet.Responses.$200>
  /**
   * education_institution_put - Updates the education institution
   */
  'education_institution_put'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.EducationInstitutionPut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.EducationInstitutionPut.Responses.$200>
  /**
   * message_get - Paginated endpoint to get all user messages
   */
  'message_get'(
    parameters?: Parameters<Paths.MessageGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.MessageGet.Responses.$200>
  /**
   * message_post - Endpoint to reply to an existing message
   */
  'message_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.MessagePost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.MessagePost.Responses.$200>
  /**
   * communication_put - Marks a communication as seen
   */
  'communication_put'(
    parameters?: Parameters<Paths.CommunicationPut.PathParameters> | null,
    data?: Paths.CommunicationPut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CommunicationPut.Responses.$200>
  /**
   * message_status_get - Handles messages status
   */
  'message_status_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.MessageStatusGet.Responses.$200>
}

export interface PathsDictionary {
  ['/api/configuration']: {
    /**
     * configuration_get - Returns the UI initial configuration
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ConfigurationGet.Responses.$200>
  }
  ['/api/provincelist']: {
    /**
     * province_get - Handles province queries
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProvinceGet.Responses.$200>
  }
  ['/api/countrylist']: {
    /**
     * country_get - Handles country queries
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CountryGet.Responses.$200>
  }
  ['/api/areaofinstructionlist']: {
    /**
     * area_of_instruction_get - Handles area of instruction queries
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.AreaOfInstructionGet.Responses.$200>
  }
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
  ['/api/users/manage']: {
    /**
     * psp_user_manage_get - Gets PSP representatives for the current user's institution
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserManageGet.Responses.$200>
  }
  ['/api/users/manage/{programRepId}/deactivate']: {
    /**
     * psp_user_manage_deactivate_post - Deactivates a PSP representative for the current user's institution
     */
    'post'(
      parameters?: Parameters<Paths.PspUserManageDeactivatePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserManageDeactivatePost.Responses.$200>
  }
  ['/api/users/manage/{programRepId}/reactivate']: {
    /**
     * psp_user_manage_reactivate_post - Reactivates a PSP representative for the current user's institution
     */
    'post'(
      parameters?: Parameters<Paths.PspUserManageReactivatePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserManageReactivatePost.Responses.$200>
  }
  ['/api/users/manage/{programRepId}/set-primary']: {
    /**
     * psp_user_manage_set_primary_post - Sets the specified PSP representative as Primary for the current user's institution
     */
    'post'(
      parameters?: Parameters<Paths.PspUserManageSetPrimaryPost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserManageSetPrimaryPost.Responses.$200>
  }
  ['/api/users/manage/add']: {
    /**
     * psp_user_add - Adds a new psp user to an institution
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserAdd.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserAdd.Responses.$200>
  }
  ['/api/users/profile']: {
    /**
     * psp_user_profile_get - Gets the currently logged in user profile or NotFound if no profile found
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserProfileGet.Responses.$200>
    /**
     * psp_user_profile_put - Updates the currently logged in users profile
     */
    'put'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserProfilePut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserProfilePut.Responses.$200>
  }
  ['/api/users/register']: {
    /**
     * psp_user_register_post - Update a psp user profile
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.PspUserRegisterPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PspUserRegisterPost.Responses.$200>
  }
  ['/api/draftprograms/{id}']: {
    /**
     * draftprogram_put - Save a draft program for the current user
     */
    'put'(
      parameters?: Parameters<Paths.DraftprogramPut.PathParameters> | null,
      data?: Paths.DraftprogramPut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.DraftprogramPut.Responses.$200>
  }
  ['/api/programs/{id}']: {
    /**
     * program_get - Handles program queries
     */
    'get'(
      parameters?: Parameters<Paths.ProgramGet.QueryParameters & Paths.ProgramGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProgramGet.Responses.$200>
  }
  ['/api/program/{id}/courses']: {
    /**
     * course_put - Update a course for a program profile
     */
    'put'(
      parameters?: Parameters<Paths.CoursePut.PathParameters> | null,
      data?: Paths.CoursePut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CoursePut.Responses.$200>
  }
  ['/api/program/{id}']: {
    /**
     * program_put - Update program profile
     */
    'put'(
      parameters?: Parameters<Paths.ProgramPut.PathParameters> | null,
      data?: Paths.ProgramPut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProgramPut.Responses.$200>
  }
  ['/api/programs']: {
    /**
     * program_post - Submit a draft program profile
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProgramPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProgramPost.Responses.$200>
  }
  ['/api/PortalInvitations/{token}']: {
    /**
     * portal_invitation_get - Handles portal invitation queries
     */
    'get'(
      parameters?: Parameters<Paths.PortalInvitationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PortalInvitationGet.Responses.$200>
  }
  ['/api/files/communication/{communicationId}/file/{fileId}']: {
    /**
     * files_communication_get - Handles fetching files
     */
    'get'(
      parameters?: Parameters<Paths.FilesCommunicationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<any>
  }
  ['/api/files/{fileId}']: {
    /**
     * delete_file - Handles delete uploaded file request
     */
    'delete'(
      parameters?: Parameters<Paths.DeleteFile.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.DeleteFile.Responses.$200>
    /**
     * upload_file - Handles upload file request
     */
    'post'(
      parameters?: Parameters<Paths.UploadFile.PathParameters> | null,
      data?: Paths.UploadFile.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.UploadFile.Responses.$200>
  }
  ['/api/education-institution']: {
    /**
     * education_institution_get - Get users education institution
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.EducationInstitutionGet.Responses.$200>
    /**
     * education_institution_put - Updates the education institution
     */
    'put'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.EducationInstitutionPut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.EducationInstitutionPut.Responses.$200>
  }
  ['/api/messages/{parentId}']: {
    /**
     * message_get - Paginated endpoint to get all user messages
     */
    'get'(
      parameters?: Parameters<Paths.MessageGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.MessageGet.Responses.$200>
  }
  ['/api/messages']: {
    /**
     * message_post - Endpoint to reply to an existing message
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.MessagePost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.MessagePost.Responses.$200>
  }
  ['/api/messages/{id}/seen']: {
    /**
     * communication_put - Marks a communication as seen
     */
    'put'(
      parameters?: Parameters<Paths.CommunicationPut.PathParameters> | null,
      data?: Paths.CommunicationPut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CommunicationPut.Responses.$200>
  }
  ['/api/messages/status']: {
    /**
     * message_status_get - Handles messages status
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.MessageStatusGet.Responses.$200>
  }
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>


export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type AreaOfInstruction = Components.Schemas.AreaOfInstruction;
export type AreaOfInstructionListResponse = Components.Schemas.AreaOfInstructionListResponse;
export type Auspice = Components.Schemas.Auspice;
export type Communication = Components.Schemas.Communication;
export type CommunicationDocument = Components.Schemas.CommunicationDocument;
export type CommunicationResponse = Components.Schemas.CommunicationResponse;
export type CommunicationSeenRequest = Components.Schemas.CommunicationSeenRequest;
export type CommunicationStatus = Components.Schemas.CommunicationStatus;
export type CommunicationsStatus = Components.Schemas.CommunicationsStatus;
export type CommunicationsStatusResults = Components.Schemas.CommunicationsStatusResults;
export type Country = Components.Schemas.Country;
export type Course = Components.Schemas.Course;
export type CourseAreaOfInstruction = Components.Schemas.CourseAreaOfInstruction;
export type DraftProgramResponse = Components.Schemas.DraftProgramResponse;
export type EducationInstitution = Components.Schemas.EducationInstitution;
export type FileResponse = Components.Schemas.FileResponse;
export type GetMessagesResponse = Components.Schemas.GetMessagesResponse;
export type GetProgramsResponse = Components.Schemas.GetProgramsResponse;
export type HttpValidationProblemDetails = Components.Schemas.HttpValidationProblemDetails;
export type InitiatedFrom = Components.Schemas.InitiatedFrom;
export type InviteType = Components.Schemas.InviteType;
export type NewPspUserResponse = Components.Schemas.NewPspUserResponse;
export type OidcAuthenticationSettings = Components.Schemas.OidcAuthenticationSettings;
export type PortalAccessStatus = Components.Schemas.PortalAccessStatus;
export type PortalInvitation = Components.Schemas.PortalInvitation;
export type PortalInvitationQueryResult = Components.Schemas.PortalInvitationQueryResult;
export type ProblemDetails = Components.Schemas.ProblemDetails;
export type Program = Components.Schemas.Program;
export type ProgramProfileType = Components.Schemas.ProgramProfileType;
export type ProgramStatus = Components.Schemas.ProgramStatus;
export type ProgramTypes = Components.Schemas.ProgramTypes;
export type Province = Components.Schemas.Province;
export type PspRegistrationError = Components.Schemas.PspRegistrationError;
export type PspRegistrationErrorResponse = Components.Schemas.PspRegistrationErrorResponse;
export type PspUserListItem = Components.Schemas.PspUserListItem;
export type PspUserProfile = Components.Schemas.PspUserProfile;
export type PspUserRole = Components.Schemas.PspUserRole;
export type RegisterPspUserRequest = Components.Schemas.RegisterPspUserRequest;
export type SaveDraftProgramRequest = Components.Schemas.SaveDraftProgramRequest;
export type SendMessageRequest = Components.Schemas.SendMessageRequest;
export type SendMessageResponse = Components.Schemas.SendMessageResponse;
export type SubmitProgramRequest = Components.Schemas.SubmitProgramRequest;
export type UpdateCourseRequest = Components.Schemas.UpdateCourseRequest;
export type VersionMetadata = Components.Schemas.VersionMetadata;
