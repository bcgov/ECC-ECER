import type { OpenAPIClient, Parameters, UnknownParamsObject, OperationResponse, AxiosRequestConfig } from "openapi-client-axios";

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
      | "Pending"
      | "Ready"
      | "InProgress"
      | "PendingQueue"
      | "ReconsiderationDecision"
      | "AppealDecision";
    export type ApplicationStatusReasonDetail =
      | "Actioned"
      | "BeingAssessed"
      | "Certified"
      | "Denied"
      | "ForReview"
      | "InvestigationsConsultationNeeded"
      | "MoreInformationRequired"
      | "OperationSupervisorManagerofCertificationsConsultationNeeded"
      | "PendingDocuments"
      | "ProgramAnalystReview"
      | "ReadyforAssessment"
      | "ReceivedPending"
      | "ReceivePhysicalTranscripts"
      | "SupervisorConsultationNeeded"
      | "ValidatingIDs";
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
    export interface CharacterReferenceEvaluation {
      referenceRelationship?: ReferenceRelationship;
      referenceRelationshipOther?: string | null;
      lengthOfAcquaintance?: ReferenceKnownTime;
      workedWithChildren?: boolean;
      childInteractionObservations?: string | null;
      applicantTemperamentAssessment?: string | null;
    }
    export type CharacterReferenceStage =
      | "ApplicationSubmitted"
      | "Approved"
      | "Draft"
      | "InProgress"
      | "Rejected"
      | "Submitted"
      | "UnderReview"
      | "WaitingResponse";
    export interface CharacterReferenceStatus {
      id?: string | null;
      status?: CharacterReferenceStage;
      firstName?: string | null;
      lastName?: string | null;
      emailAddress?: string | null;
      phoneNumber?: string | null;
      willProvideReference?: boolean | null;
    }
    export interface CharacterReferenceSubmissionRequest {
      token?: string | null;
      willProvideReference?: boolean;
      referenceContactInformation?: ReferenceContactInformation;
      referenceEvaluation?: CharacterReferenceEvaluation;
      confirmProvidedInformationIsRight?: boolean;
      recaptchaToken?: string | null;
    }
    export type ChildcareAgeRanges = "From0to12Months" | "From12to24Months" | "From25to30Months" | "From31to36Months" | "Grade1" | "Preschool";
    export type ChildrenProgramType =
      | "Childminding"
      | "Familychildcare"
      | "Groupchildcare"
      | "InHomeMultiAgechildcare"
      | "MultiAgechildcare"
      | "Occasionalchildcare"
      | "Other"
      | "Preschool";
    export interface Communication {
      id?: string | null;
      subject?: string | null;
      text?: string | null;
      acknowledged?: boolean;
      notifiedOn?: string; // date-time
      status?: CommunicationStatus;
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
    /**
     * file Response
     */
    export interface FileResponse {
      /**
       *
       */
      fileId?: string | null;
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
    export type LikertScale = "Yes" | "No";
    export interface OidcAuthenticationSettings {
      authority?: string | null;
      clientId?: string | null;
      scope?: string | null;
      idp?: string | null;
    }
    export interface OptOutReferenceRequest {
      token?: string | null;
      unabletoProvideReferenceReasons?: UnabletoProvideReferenceReasons;
      recaptchaToken?: string | null;
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
      workExperienceReferenceHours?: number | null; // int32
    }
    export interface PortalInvitationQueryResult {
      portalInvitation?: PortalInvitation;
    }
    export type PortalStage = "CertificationType" | "Declaration" | "ContactInformation" | "Education" | "CharacterReferences" | "WorkReferences" | "Review";
    /**
     * Previous Name
     */
    export interface PreviousName {
      firstName?: string | null;
      lastName?: string | null;
      id?: string | null;
      middleName?: string | null;
      preferredName?: string | null;
      status?: PreviousNameStage;
    }
    export type PreviousNameStage = "Unverified" | "ReadyforVerification" | "Verified" | "Archived";
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
    export interface ReferenceContactInformation {
      lastName?: string | null;
      firstName?: string | null;
      email?: string | null;
      phoneNumber?: string | null;
      certificateProvinceOther?: string | null;
      certificateProvinceId?: string | null;
      certificateNumber?: string | null;
      dateOfBirth?: string | null; // date-time
    }
    export type ReferenceKnownTime = "From1to2years" | "From2to5years" | "From6monthsto1year" | "Lessthan6months" | "Morethan5years";
    export type ReferenceRelationship = "CoWorker" | "Other" | "ParentGuardianofChildinCare" | "Supervisor" | "Teacher";
    /**
     * Resend reference invite response
     */
    export interface ResendReferenceInviteResponse {
      /**
       * The reference id
       */
      referenceId?: string | null;
    }
    /**
     * Save draft application request
     */
    export interface SaveDraftApplicationRequest {
      draftApplication?: DraftApplication;
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
    export interface SubmitApplicationResponse {
      applicationId?: string | null;
    }
    export interface SubmittedApplicationStatus {
      id?: string | null;
      submittedOn?: string; // date-time
      status?: ApplicationStatus;
      subStatus?: ApplicationStatusReasonDetail;
      certificationTypes?: CertificationType[] | null;
      readyForAssessmentDate?: string | null; // date-time
      transcriptsStatus?: TranscriptStatus[] | null;
      workExperienceReferencesStatus?: WorkExperienceReferenceStatus[] | null;
      characterReferencesStatus?: CharacterReferenceStatus[] | null;
      addMoreCharacterReference?: boolean | null;
      addMoreWorkExperienceReference?: boolean | null;
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
    export type TranscriptStage = "Accepted" | "ApplicationSubmitted" | "Draft" | "InProgress" | "Rejected" | "Submitted" | "WaitingforDetails";
    export interface TranscriptStatus {
      id?: string | null;
      status?: TranscriptStage;
      educationalInstitutionName?: string | null;
    }
    export type UnabletoProvideReferenceReasons =
      | "Iamunabletoatthistime"
      | "Idonothavetheinformationrequired"
      | "Idonotknowthisperson"
      | "Idonotmeettherequirementstoprovideareference"
      | "Other";
    export interface UpdateReferenceResponse {
      referenceId?: string | null;
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
      previousNames?: /* Previous Name */ PreviousName[] | null;
    }
    export type WorkExperienceRefStage =
      | "ApplicationSubmitted"
      | "Approved"
      | "Draft"
      | "InProgress"
      | "Rejected"
      | "Submitted"
      | "UnderReview"
      | "WaitingforResponse";
    export interface WorkExperienceReference {
      firstName?: string | null;
      lastName?: string | null;
      emailAddress?: string | null;
      hours?: number | null; // int32
      id?: string | null;
      phoneNumber?: string | null;
    }
    export interface WorkExperienceReferenceCompetenciesAssessment {
      childDevelopment?: LikertScale;
      childDevelopmentReason?: string | null;
      childGuidance?: LikertScale;
      childGuidanceReason?: string | null;
      healthSafetyAndNutrition?: LikertScale;
      healthSafetyAndNutritionReason?: string | null;
      developAnEceCurriculum?: LikertScale;
      developAnEceCurriculumReason?: string | null;
      implementAnEceCurriculum?: LikertScale;
      implementAnEceCurriculumReason?: string | null;
      fosteringPositiveRelationChild?: LikertScale;
      fosteringPositiveRelationChildReason?: string | null;
      fosteringPositiveRelationFamily?: LikertScale;
      fosteringPositiveRelationFamilyReason?: string | null;
      fosteringPositiveRelationCoworker?: LikertScale;
      fosteringPositiveRelationCoworkerReason?: string | null;
    }
    export interface WorkExperienceReferenceDetails {
      hours?: number; // int32
      workHoursType?: WorkHoursType;
      childrenProgramName?: string | null;
      childrenProgramType?: ChildrenProgramType;
      childrenProgramTypeOther?: string | null;
      childcareAgeRanges?: ChildcareAgeRanges[] | null;
      startDate?: string; // date-time
      endDate?: string; // date-time
      referenceRelationship?: ReferenceRelationship;
      referenceRelationshipOther?: string | null;
    }
    export interface WorkExperienceReferenceStatus {
      id?: string | null;
      status?: WorkExperienceRefStage;
      firstName?: string | null;
      lastName?: string | null;
      emailAddress?: string | null;
      phoneNumber?: string | null;
      totalNumberofHoursAnticipated?: number | null; // int32
      totalNumberofHoursApproved?: number | null; // int32
      totalNumberofHoursObserved?: number | null; // int32
      willProvideReference?: boolean | null;
    }
    export interface WorkExperienceReferenceSubmissionRequest {
      token?: string | null;
      willProvideReference?: boolean;
      referenceContactInformation?: ReferenceContactInformation;
      workExperienceReferenceDetails?: WorkExperienceReferenceDetails;
      workExperienceReferenceCompetenciesAssessment?: WorkExperienceReferenceCompetenciesAssessment;
      confirmProvidedInformationIsRight?: boolean;
      recaptchaToken?: string | null;
    }
    export type WorkHoursType = "FullTime" | "PartTime";
  }
}
declare namespace Paths {
  namespace ApplicationCharacterReferenceResendInvitePost {
    namespace Parameters {
      export type ApplicationId = string;
      export type ReferenceId = string;
    }
    export interface PathParameters {
      applicationId: Parameters.ApplicationId;
      referenceId: Parameters.ReferenceId;
    }
    namespace Responses {
      export type $200 = /* Resend reference invite response */ Components.Schemas.ResendReferenceInviteResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace ApplicationCharacterreferenceUpdatePost {
    namespace Parameters {
      export type ApplicationId = string;
      export type ReferenceId = string;
    }
    export interface PathParameters {
      application_id: Parameters.ApplicationId;
      reference_id?: Parameters.ReferenceId;
    }
    export type RequestBody = Components.Schemas.CharacterReference;
    namespace Responses {
      export type $200 = Components.Schemas.UpdateReferenceResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
    }
  }
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
  namespace ApplicationStatusGet {
    namespace Parameters {
      export type Id = string;
    }
    export interface PathParameters {
      id: Parameters.Id;
    }
    namespace Responses {
      export type $200 = Components.Schemas.SubmittedApplicationStatus;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
      export type $404 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace ApplicationWorkExperienceReferenceResendInvitePost {
    namespace Parameters {
      export type ApplicationId = string;
      export type ReferenceId = string;
    }
    export interface PathParameters {
      applicationId: Parameters.ApplicationId;
      referenceId: Parameters.ReferenceId;
    }
    namespace Responses {
      export type $200 = /* Resend reference invite response */ Components.Schemas.ResendReferenceInviteResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
    }
  }
  namespace ApplicationWorkexperiencereferenceUpdatePost {
    namespace Parameters {
      export type ApplicationId = string;
      export type ReferenceId = string;
    }
    export interface PathParameters {
      application_id: Parameters.ApplicationId;
      reference_id?: Parameters.ReferenceId;
    }
    export type RequestBody = Components.Schemas.WorkExperienceReference;
    namespace Responses {
      export type $200 = Components.Schemas.UpdateReferenceResponse;
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
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
    }
  }
  namespace ConfigurationGet {
    namespace Responses {
      export type $200 = Components.Schemas.ApplicationConfiguration;
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
      export interface $404 {}
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
  namespace MessagePost {
    export type RequestBody = /* Send Message Request */ Components.Schemas.SendMessageRequest;
    namespace Responses {
      export type $200 = /* Send Message Response */ Components.Schemas.SendMessageResponse;
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
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
  namespace RecaptchaSiteKeyGet {
    namespace Responses {
      export type $200 = string;
    }
  }
  namespace ReferenceOptout {
    export type RequestBody = Components.Schemas.OptOutReferenceRequest;
    namespace Responses {
      export interface $200 {}
      export type $400 = Components.Schemas.HttpValidationProblemDetails;
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
  namespace UploadFile {
    export interface HeaderParameters {
      "file-classification": Parameters.FileClassification;
      "file-tag"?: Parameters.FileTag;
    }
    namespace Parameters {
      export type FileClassification = string;
      export type FileId = string;
      export type FileTag = string;
    }
    export interface PathParameters {
      fileId: Parameters.FileId;
    }
    export type RequestBody = string; // binary
    namespace Responses {
      export type $200 = /* file Response */ Components.Schemas.FileResponse;
      export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
      export interface $404 {}
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
  namespace WorkExperienceReferencePost {
    export type RequestBody = Components.Schemas.WorkExperienceReferenceSubmissionRequest;
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
   * recaptcha_site_key_get - Obtains site key for recaptcha
   */
  "recaptcha_site_key_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.RecaptchaSiteKeyGet.Responses.$200>;
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
   * workExperience_reference_post - Handles work experience reference submission
   */
  "workExperience_reference_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.WorkExperienceReferencePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.WorkExperienceReferencePost.Responses.$200>;
  /**
   * reference_optout - Handles reference optout
   */
  "reference_optout"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ReferenceOptout.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ReferenceOptout.Responses.$200>;
  /**
   * upload_file - Handles upload file request
   */
  "upload_file"(
    parameters?: Parameters<Paths.UploadFile.PathParameters & Paths.UploadFile.HeaderParameters> | null,
    data?: Paths.UploadFile.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.UploadFile.Responses.$200>;
  /**
   * delete_file - Handles delete uploaded file request
   */
  "delete_file"(
    parameters?: Parameters<Paths.DeleteFile.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.DeleteFile.Responses.$200>;
  /**
   * message_get - Handles messages queries
   */
  "message_get"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessageGet.Responses.$200>;
  /**
   * message_post - Handles message send request
   */
  "message_post"(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.MessagePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.MessagePost.Responses.$200>;
  /**
   * communication_put - Marks a communication as seen
   */
  "communication_put"(
    parameters?: Parameters<Paths.CommunicationPut.PathParameters> | null,
    data?: Paths.CommunicationPut.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.CommunicationPut.Responses.$200>;
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
   * application_status_get - Handles application status queries
   */
  "application_status_get"(
    parameters?: Parameters<Paths.ApplicationStatusGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationStatusGet.Responses.$200>;
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
  /**
   * application_workexperiencereference_update_post - Update work experience reference
   */
  "application_workexperiencereference_update_post"(
    parameters?: Parameters<Paths.ApplicationWorkexperiencereferenceUpdatePost.PathParameters> | null,
    data?: Paths.ApplicationWorkexperiencereferenceUpdatePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationWorkexperiencereferenceUpdatePost.Responses.$200>;
  /**
   * application_characterreference_update_post - Update character reference
   */
  "application_characterreference_update_post"(
    parameters?: Parameters<Paths.ApplicationCharacterreferenceUpdatePost.PathParameters> | null,
    data?: Paths.ApplicationCharacterreferenceUpdatePost.RequestBody,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationCharacterreferenceUpdatePost.Responses.$200>;
  /**
   * application_character_reference_resend_invite_post - Resend a character reference invite
   *
   * Changes character reference invite again status to true
   */
  "application_character_reference_resend_invite_post"(
    parameters?: Parameters<Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationCharacterReferenceResendInvitePost.Responses.$200>;
  /**
   * application_work_experience_reference_resend_invite_post - Resend a work experience reference invite
   *
   * Changes work experience reference invite again status to true
   */
  "application_work_experience_reference_resend_invite_post"(
    parameters?: Parameters<Paths.ApplicationWorkExperienceReferenceResendInvitePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig,
  ): OperationResponse<Paths.ApplicationWorkExperienceReferenceResendInvitePost.Responses.$200>;
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
  ["/api/recaptchaSiteKey"]: {
    /**
     * recaptcha_site_key_get - Obtains site key for recaptcha
     */
    "get"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.RecaptchaSiteKeyGet.Responses.$200>;
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
  ["/api/References/WorkExperience"]: {
    /**
     * workExperience_reference_post - Handles work experience reference submission
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.WorkExperienceReferencePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.WorkExperienceReferencePost.Responses.$200>;
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
  ["/api/files/{fileId}"]: {
    /**
     * delete_file - Handles delete uploaded file request
     */
    "delete"(
      parameters?: Parameters<Paths.DeleteFile.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.DeleteFile.Responses.$200>;
    /**
     * upload_file - Handles upload file request
     */
    "post"(
      parameters?: Parameters<Paths.UploadFile.PathParameters & Paths.UploadFile.HeaderParameters> | null,
      data?: Paths.UploadFile.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.UploadFile.Responses.$200>;
  };
  ["/api/messages"]: {
    /**
     * message_get - Handles messages queries
     */
    "get"(parameters?: Parameters<UnknownParamsObject> | null, data?: any, config?: AxiosRequestConfig): OperationResponse<Paths.MessageGet.Responses.$200>;
    /**
     * message_post - Handles message send request
     */
    "post"(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.MessagePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.MessagePost.Responses.$200>;
  };
  ["/api/messages/{id}/seen"]: {
    /**
     * communication_put - Marks a communication as seen
     */
    "put"(
      parameters?: Parameters<Paths.CommunicationPut.PathParameters> | null,
      data?: Paths.CommunicationPut.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.CommunicationPut.Responses.$200>;
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
  ["/api/applications/{id}/status"]: {
    /**
     * application_status_get - Handles application status queries
     */
    "get"(
      parameters?: Parameters<Paths.ApplicationStatusGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationStatusGet.Responses.$200>;
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
  ["/api/applications/{application_id}/workexperiencereference/{reference_id}"]: {
    /**
     * application_workexperiencereference_update_post - Update work experience reference
     */
    "post"(
      parameters?: Parameters<Paths.ApplicationWorkexperiencereferenceUpdatePost.PathParameters> | null,
      data?: Paths.ApplicationWorkexperiencereferenceUpdatePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationWorkexperiencereferenceUpdatePost.Responses.$200>;
  };
  ["/api/applications/{application_id}/characterreference/{reference_id}"]: {
    /**
     * application_characterreference_update_post - Update character reference
     */
    "post"(
      parameters?: Parameters<Paths.ApplicationCharacterreferenceUpdatePost.PathParameters> | null,
      data?: Paths.ApplicationCharacterreferenceUpdatePost.RequestBody,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationCharacterreferenceUpdatePost.Responses.$200>;
  };
  ["/api/applications/{applicationId}/characterReference/{referenceId}/resendInvite"]: {
    /**
     * application_character_reference_resend_invite_post - Resend a character reference invite
     *
     * Changes character reference invite again status to true
     */
    "post"(
      parameters?: Parameters<Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationCharacterReferenceResendInvitePost.Responses.$200>;
  };
  ["/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite"]: {
    /**
     * application_work_experience_reference_resend_invite_post - Resend a work experience reference invite
     *
     * Changes work experience reference invite again status to true
     */
    "post"(
      parameters?: Parameters<Paths.ApplicationWorkExperienceReferenceResendInvitePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig,
    ): OperationResponse<Paths.ApplicationWorkExperienceReferenceResendInvitePost.Responses.$200>;
  };
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>;
