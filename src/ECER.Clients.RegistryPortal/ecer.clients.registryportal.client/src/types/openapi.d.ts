import type {
  OpenAPIClient,
  Parameters,
  UnknownParamsObject,
  OperationResponse,
  AxiosRequestConfig,
} from 'openapi-client-axios';

declare namespace Components {
    namespace Schemas {
        export interface AddProfessionalDevelopmentResponse {
            applicationId?: string | null;
        }
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
            characterReferences?: CharacterReference[] | null;
            professionalDevelopments?: ProfessionalDevelopment[] | null;
            status?: ApplicationStatus;
            stage?: string | null;
            applicationType?: ApplicationTypes;
            educationOrigin?: EducationOrigin;
            educationRecognition?: EducationRecognition;
            oneYearRenewalExplanationChoice?: OneYearRenewalexplanations;
            fiveYearRenewalExplanationChoice?: FiveYearRenewalExplanations;
            renewalExplanationOther?: string | null;
            origin?: ApplicationOrigin;
            labourMobilityCertificateInformation?: CertificateInformation;
        }
        export interface ApplicationConfiguration {
            clientAuthenticationMethods?: {
                [name: string]: OidcAuthenticationSettings;
            } | null;
        }
        export type ApplicationOrigin = "Manual" | "Oracle" | "Portal";
        export type ApplicationStatus = "Draft" | "Submitted" | "Complete" | "Reconsideration" | "Cancelled" | "Closed" | "Escalated" | "Decision" | "Withdrawn" | "Pending" | "Ready" | "InProgress" | "PendingQueue" | "ReconsiderationDecision" | "AppealDecision";
        export type ApplicationStatusReasonDetail = "Actioned" | "BeingAssessed" | "Certified" | "Denied" | "ForReview" | "InvestigationsConsultationNeeded" | "MoreInformationRequired" | "OperationSupervisorManagerofCertificationsConsultationNeeded" | "PendingDocuments" | "ProgramAnalystReview" | "ReadyforAssessment" | "ReceivedPending" | "ReceivePhysicalTranscripts" | "SupervisorConsultationNeeded" | "ValidatingIDs";
        /**
         * Submit application request
         */
        export interface ApplicationSubmissionRequest {
            /**
             * The application id
             */
            id?: string | null;
        }
        export type ApplicationTypes = "New" | "Renewal" | "LabourMobility";
        /**
         * delete draft application response
         */
        export interface CancelDraftApplicationResponse {
            /**
             * The application id
             */
            applicationId?: string | null;
        }
        export interface CertificateCondition {
            id?: string | null;
            name?: string | null;
            details?: string | null;
            startDate?: string; // date-time
            endDate?: string; // date-time
            displayOrder?: number; // int32
        }
        export interface CertificateInformation {
            certificateComparisonId?: string | null;
            labourMobilityProvince?: Province;
            currentCertificationNumber?: string | null;
            existingCertificationType?: string | null;
            legalFirstName?: string | null;
            legalMiddleName?: string | null;
            legalLastName?: string | null;
            hasOtherName?: boolean | null;
        }
        export type CertificatePDFGeneration = "No" | "Requested" | "Yes";
        export type CertificateStatusCode = "Active" | "Cancelled" | "Expired" | "Inactive" | "Renewed" | "Reprinted" | "Suspended";
        export interface Certification {
            id?: string | null;
            name?: string | null;
            number?: string | null;
            expiryDate?: string | null; // date-time
            effectiveDate?: string | null; // date-time
            date?: string | null; // date-time
            printDate?: string | null; // date-time
            hasConditions?: boolean | null;
            levelName?: string | null;
            statusCode?: CertificateStatusCode;
            certificatePDFGeneration?: CertificatePDFGeneration;
            ineligibleReference?: YesNoNull;
            levels?: CertificationLevel[] | null;
            files?: CertificationFile[] | null;
            certificateConditions?: CertificateCondition[] | null;
        }
        export interface CertificationComparison {
            id?: string | null;
            bcCertificate?: string | null;
        }
        export interface CertificationFile {
            id?: string | null;
            url?: string | null;
            extention?: string | null;
            size?: string | null;
            name?: string | null;
        }
        export interface CertificationLevel {
            id?: string | null;
            type?: string | null;
        }
        export interface CertificationLookupRequest {
            recaptchaToken?: string | null;
            firstName?: string | null;
            lastName?: string | null;
            registrationNumber?: string | null;
            pageSize?: number; // int32
            pageNumber?: number; // int32
            sortField?: string | null;
            sortDirection?: string | null;
        }
        export interface CertificationLookupResponse {
            id?: string | null;
            name?: string | null;
            registrationNumber?: string | null;
            statusCode?: CertificateStatusCode;
            levelName?: string | null;
            expiryDate?: string | null; // date-time
            hasConditions?: boolean | null;
            levels?: CertificationLevel[] | null;
            certificateConditions?: CertificateCondition[] | null;
        }
        export type CertificationType = "EceAssistant" | "OneYear" | "FiveYears" | "Ite" | "Sne";
        export interface CharacterReference {
            lastName?: string | null;
            phoneNumber?: string | null;
            emailAddress?: string | null;
            firstName?: string | null;
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
        export type CharacterReferenceStage = "ApplicationSubmitted" | "Approved" | "Draft" | "InProgress" | "Rejected" | "Submitted" | "UnderReview" | "WaitingResponse";
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
        export type ChildcareAgeRanges = "_036months" | "_35years" | "_68years";
        export type ChildrenProgramType = "Childminding" | "Familychildcare" | "Groupchildcare" | "InHomeMultiAgechildcare" | "MultiAgechildcare" | "Occasionalchildcare" | "Other" | "Preschool";
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
        export interface ComparisonRecord {
            transferringCertificate?: OutOfProvinceCertificationType;
            options?: CertificationComparison[] | null;
        }
        export type ComprehensiveReportOptions = "FeeWaiver" | "InternationalCredentialEvaluationService" | "RegistryAlreadyHas";
        export interface Country {
            countryId?: string | null;
            countryName?: string | null;
            countryCode?: string | null;
        }
        export type CourseOutlineOptions = "UploadNow" | "RegistryAlreadyHas";
        export interface DraftApplication {
            id?: string | null;
            signedDate?: string | null; // date-time
            certificationTypes?: CertificationType[] | null;
            transcripts?: Transcript[] | null;
            workExperienceReferences?: WorkExperienceReference[] | null;
            characterReferences?: CharacterReference[] | null;
            professionalDevelopments?: ProfessionalDevelopment[] | null;
            stage?: string | null;
            fromCertificate?: string | null;
            applicationType?: ApplicationTypes;
            educationOrigin?: EducationOrigin;
            educationRecognition?: EducationRecognition;
            oneYearRenewalExplanationChoice?: OneYearRenewalexplanations;
            fiveYearRenewalExplanationChoice?: FiveYearRenewalExplanations;
            renewalExplanationOther?: string | null;
            createdOn?: string | null; // date-time
            origin?: ApplicationOrigin;
            labourMobilityCertificateInformation?: CertificateInformation;
        }
        /**
         * Save draft application response
         */
        export interface DraftApplicationResponse {
            application?: Application;
        }
        export type EducationOrigin = "InsideBC" | "OutsideBC" | "OutsideofCanada";
        export type EducationRecognition = "Recognized" | "NotRecognized";
        export interface FileInfo {
            id?: string | null;
            url?: string | null;
            extention?: string | null;
            name?: string | null;
            size?: string | null;
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
        export type FiveYearRenewalExplanations = "Ileftthechildcarefieldforpersonalreasons" | "Iwasunabletocompletetherequiredhoursofprofessionaldevelopment" | "Iwasunabletofindemploymentinthechildcarefieldinmycommunity" | "MyemploymentdiddoesnotrequirecertificationasanECEforexamplenannyteachercollegeinstructoradministratoretc" | "Other";
        export interface GetMessagesResponse {
            communications?: Communication[] | null;
            totalMessagesCount?: number; // int32
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
        export interface IdentificationType {
            id?: string | null;
            name?: string | null;
            forPrimary?: boolean;
            forSecondary?: boolean;
        }
        export interface IdentityDocument {
            id?: string | null;
            url?: string | null;
            extention?: string | null;
            name?: string | null;
            size?: string | null;
        }
        export type InitiatedFrom = "Investigation" | "PortalUser" | "Registry";
        export type InviteType = "CharacterReference" | "WorkExperienceReference";
        export type LikertScale = "Yes" | "No";
        export interface OidcAuthenticationSettings {
            authority?: string | null;
            clientId?: string | null;
            scope?: string | null;
            idp?: string | null;
        }
        export type OneYearRenewalexplanations = "IliveandworkinacommunitywithoutothercertifiedECEs" | "Iwasunabletofindemploymentinthechildcarefieldtocompletetherequirednumberofhours" | "Iwasunabletoworkduetothestatusofmyvisaorwasunabletoenterthecountryasexpected" | "Iwasunabletoworkinthechildcarefieldforpersonalreasons" | "Other";
        export interface OptOutReferenceRequest {
            token?: string | null;
            unabletoProvideReferenceReasons?: UnabletoProvideReferenceReasons;
            recaptchaToken?: string | null;
        }
        export interface OutOfProvinceCertificationType {
            id?: string | null;
            certificationType?: string | null;
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
            workExperienceType?: WorkExperienceTypes;
            latestCertification?: Certification;
        }
        export interface PortalInvitationQueryResult {
            portalInvitation?: PortalInvitation;
        }
        export type PortalTags = "LOGIN" | "LOOKUP" | "REFERENCES";
        export interface PostSecondaryInstitution {
            id?: string | null;
            name?: string | null;
            provinceId?: string | null;
        }
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
            source?: PreviousNameSources;
            documents?: IdentityDocument[] | null;
        }
        export type PreviousNameSources = "NameLog" | "Profile" | "Transcript" | "OutofProvinceCertificate";
        export type PreviousNameStage = "Archived" | "PendingforDocuments" | "ReadyforVerification" | "Unverified" | "Verified";
        export interface ProblemDetails {
            [name: string]: any;
            type?: string | null;
            title?: string | null;
            status?: number | null; // int32
            detail?: string | null;
            instance?: string | null;
        }
        export interface ProfessionalDevelopment {
            courseName?: string | null;
            organizationName?: string | null;
            startDate?: string; // date-time
            endDate?: string; // date-time
            numberOfHours?: number; // double
            id?: string | null;
            organizationContactInformation?: string | null;
            organizationEmailAddress?: string | null;
            instructorName?: string | null;
            courseorWorkshopLink?: string | null;
            status?: ProfessionalDevelopmentStatusCode;
            deletedFiles?: string[] | null;
            newFiles?: string[] | null;
            files?: FileInfo[] | null;
        }
        export interface ProfessionalDevelopmentStatus {
            id?: string | null;
            courseName?: string | null;
            numberOfHours?: number; // double
            status?: ProfessionalDevelopmentStatusCode;
        }
        export type ProfessionalDevelopmentStatusCode = "ApplicationSubmitted" | "Approved" | "Draft" | "InProgress" | "Rejected" | "Submitted" | "UnderReview" | "WaitingResponse";
        export interface ProfileIdentification {
            registrantId?: string | null;
            primaryIdTypeObjectId?: string | null;
            primaryIds?: IdentityDocument[] | null;
            secondaryIdTypeObjectId?: string | null;
            secondaryIds?: IdentityDocument[] | null;
        }
        export type ProgramConfirmationOptions = "UploadNow" | "RegistryAlreadyHas";
        export interface Province {
            provinceId?: string | null;
            provinceName?: string | null;
            provinceCode?: string | null;
        }
        export interface ReferenceContactInformation {
            lastName?: string | null;
            email?: string | null;
            phoneNumber?: string | null;
            certificateProvinceOther?: string | null;
            firstName?: string | null;
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
        export type StatusCode = "Inactive" | "PendingforDocuments" | "ReadyforIDVerification" | "ReadyforRegistrantMatch" | "Unverified" | "Verified";
        export interface SubmitApplicationResponse {
            application?: Application;
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
            professionalDevelopmentsStatus?: ProfessionalDevelopmentStatus[] | null;
            addMoreCharacterReference?: boolean | null;
            addMoreWorkExperienceReference?: boolean | null;
            addMoreProfessionalDevelopment?: boolean | null;
            fromCertificate?: string | null;
            applicationType?: ApplicationTypes;
        }
        export interface SystemMessage {
            name?: string | null;
            subject?: string | null;
            message?: string | null;
            startDate?: string; // date-time
            endDate?: string; // date-time
            portalTags?: PortalTags[] | null;
        }
        export interface Transcript {
            educationalInstitutionName?: string | null;
            programName?: string | null;
            studentLastName?: string | null;
            startDate?: string; // date-time
            endDate?: string; // date-time
            isNameUnverified?: boolean;
            educationRecognition?: EducationRecognition;
            educationOrigin?: EducationOrigin;
            id?: string | null;
            campusLocation?: string | null;
            studentFirstName?: string | null;
            studentMiddleName?: string | null;
            studentNumber?: string | null;
            isECEAssistant?: boolean;
            transcriptStatusOption?: TranscriptStatusOptions;
            country?: Country;
            province?: Province;
            postSecondaryInstitution?: PostSecondaryInstitution;
        }
        export interface TranscriptDocuments {
            applicationId?: string | null;
            transcriptId?: string | null;
            newCourseOutlineFiles?: string[] | null;
            newProgramConfirmationFiles?: string[] | null;
            courseOutlineOptions?: CourseOutlineOptions;
            comprehensiveReportOptions?: ComprehensiveReportOptions;
            programConfirmationOptions?: ProgramConfirmationOptions;
        }
        export type TranscriptStage = "Accepted" | "ApplicationSubmitted" | "Draft" | "InProgress" | "Rejected" | "Submitted" | "WaitingforDetails";
        export interface TranscriptStatus {
            id?: string | null;
            status?: TranscriptStage;
            educationalInstitutionName?: string | null;
            programName?: string | null;
            courseOutlineFiles?: FileInfo[] | null;
            programConfirmationFiles?: FileInfo[] | null;
            courseOutlineOptions?: CourseOutlineOptions;
            comprehensiveReportOptions?: ComprehensiveReportOptions;
            programConfirmationOptions?: ProgramConfirmationOptions;
            courseOutlineReceivedByRegistry?: boolean | null;
            programConfirmationReceivedByRegistry?: boolean | null;
            transcriptReceivedByRegistry?: boolean | null;
            comprehensiveReportReceivedByRegistry?: boolean | null;
            country?: Country;
            educationRecognition?: EducationRecognition;
        }
        export type TranscriptStatusOptions = "RegistryHasTranscript" | "OfficialTranscriptRequested" | "TranscriptWillRequireEnglishTranslation";
        export type UnabletoProvideReferenceReasons = "Iamunabletoatthistime" | "Idonothavetheinformationrequired" | "Idonotknowthisperson" | "Idonotmeettherequirementstoprovideareference" | "Other";
        export interface UpdateReferenceResponse {
            referenceId?: string | null;
        }
        export interface UserInfo {
            lastName?: string | null;
            dateOfBirth?: string; // date
            email?: string | null;
            phone?: string | null;
            firstName?: string | null;
            givenName?: string | null;
            middleName?: string | null;
            preferredName?: string | null;
            registrationNumber?: string | null;
            isVerified?: boolean;
            status?: StatusCode;
            unreadMessagesCount?: number; // int32
            residentialAddress?: /* Address */ Address;
            mailingAddress?: /* Address */ Address;
            isRegistrant?: boolean;
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
        export interface VersionMetadata {
            version?: string | null;
            timestamp?: string | null;
            commit?: string | null;
        }
        export type WorkExperienceRefStage = "ApplicationSubmitted" | "Approved" | "Draft" | "InProgress" | "Rejected" | "Submitted" | "UnderReview" | "WaitingforResponse";
        export interface WorkExperienceReference {
            lastName?: string | null;
            emailAddress?: string | null;
            hours?: number; // int32
            firstName?: string | null;
            id?: string | null;
            phoneNumber?: string | null;
            type?: WorkExperienceTypes;
        }
        export interface WorkExperienceReferenceCompetenciesAssessment {
            childDevelopment: LikertScale;
            childDevelopmentReason?: string | null;
            childGuidance: LikertScale;
            childGuidanceReason?: string | null;
            healthSafetyAndNutrition: LikertScale;
            healthSafetyAndNutritionReason?: string | null;
            developAnEceCurriculum: LikertScale;
            developAnEceCurriculumReason?: string | null;
            implementAnEceCurriculum: LikertScale;
            implementAnEceCurriculumReason?: string | null;
            fosteringPositiveRelationChild: LikertScale;
            fosteringPositiveRelationChildReason?: string | null;
            fosteringPositiveRelationFamily: LikertScale;
            fosteringPositiveRelationFamilyReason?: string | null;
            fosteringPositiveRelationCoworker: LikertScale;
            fosteringPositiveRelationCoworkerReason?: string | null;
        }
        export interface WorkExperienceReferenceDetails {
            hours: number; // int32
            workHoursType: WorkHoursType;
            childrenProgramName: string;
            childrenProgramType?: ChildrenProgramType;
            childrenProgramTypeOther?: string | null;
            childcareAgeRanges?: ChildcareAgeRanges[] | null;
            role?: string | null;
            ageofChildrenCaredFor?: string | null;
            startDate: string; // date-time
            endDate: string; // date-time
            referenceRelationship: ReferenceRelationship;
            referenceRelationshipOther?: string | null;
            additionalComments?: string | null;
            workExperienceType: WorkExperienceTypes;
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
            type?: WorkExperienceTypes;
        }
        export interface WorkExperienceReferenceSubmissionRequest {
            token?: string | null;
            willProvideReference?: boolean;
            referenceContactInformation?: ReferenceContactInformation;
            workExperienceReferenceDetails?: WorkExperienceReferenceDetails;
            workExperienceReferenceCompetenciesAssessment?: WorkExperienceReferenceCompetenciesAssessment;
            confirmProvidedInformationIsRight?: boolean;
            recaptchaToken?: string | null;
            workExperienceType: WorkExperienceTypes;
        }
        export type WorkExperienceTypes = "Is400Hours" | "Is500Hours";
        export type WorkHoursType = "FullTime" | "PartTime";
        export type YesNoNull = "No" | "Yes";
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
            export interface $404 {
            }
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
            export interface $404 {
            }
        }
    }
    namespace ApplicationProfessionaldevelopmentAddPost {
        namespace Parameters {
            export type ApplicationId = string;
        }
        export interface PathParameters {
            application_id: Parameters.ApplicationId;
        }
        export type RequestBody = Components.Schemas.ProfessionalDevelopment;
        namespace Responses {
            export type $200 = Components.Schemas.AddProfessionalDevelopmentResponse;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
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
    namespace ApplicationUpdateTranscriptPost {
        export type RequestBody = Components.Schemas.TranscriptDocuments;
        namespace Responses {
            export type $200 = /* Save draft application response */ Components.Schemas.DraftApplicationResponse;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
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
            export interface $404 {
            }
        }
    }
    namespace CertificationComparisonGet {
        namespace Parameters {
            export type Id = string;
            export type ProvinceId = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        export interface QueryParameters {
            provinceId?: Parameters.ProvinceId;
        }
        namespace Responses {
            export type $200 = Components.Schemas.ComparisonRecord[];
        }
    }
    namespace CertificationGet {
        namespace Parameters {
            export type Id = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        namespace Responses {
            export type $200 = Components.Schemas.Certification[];
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
        }
    }
    namespace CertificationRequestpdfPut {
        namespace Parameters {
            export type Id = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        namespace Responses {
            export type $200 = string;
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
        }
    }
    namespace CertificationsLookupPost {
        export type RequestBody = Components.Schemas.CertificationLookupRequest;
        namespace Responses {
            export type $200 = Components.Schemas.CertificationLookupResponse[];
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
        }
    }
    namespace CharacterReferencePost {
        export type RequestBody = Components.Schemas.CharacterReferenceSubmissionRequest;
        namespace Responses {
            export interface $200 {
            }
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
    namespace CountryGet {
        namespace Responses {
            export type $200 = Components.Schemas.Country[];
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
            export interface $404 {
            }
        }
    }
    namespace FilesCertificateGet {
        namespace Parameters {
            export type CertificateId = string;
        }
        export interface PathParameters {
            certificateId: Parameters.CertificateId;
        }
        namespace Responses {
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
            export interface $404 {
            }
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
    namespace IdentificationTypesGet {
        namespace Parameters {
            export type ById = string;
            export type ForPrimary = boolean;
            export type ForSecondary = boolean;
        }
        export interface QueryParameters {
            ById?: Parameters.ById;
            ForPrimary?: Parameters.ForPrimary;
            ForSecondary?: Parameters.ForSecondary;
        }
        namespace Responses {
            export type $200 = Components.Schemas.IdentificationType[];
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
        }
    }
    namespace ProfileGet {
        namespace Responses {
            export type $200 = /* User profile information */ Components.Schemas.UserProfile;
            export interface $404 {
            }
        }
    }
    namespace ProfilePut {
        export type RequestBody = /* User profile information */ Components.Schemas.UserProfile;
        namespace Responses {
            export interface $200 {
            }
        }
    }
    namespace ProfileVerificationPost {
        export type RequestBody = Components.Schemas.ProfileIdentification;
        namespace Responses {
            export interface $200 {
            }
            export type $400 = Components.Schemas.ProblemDetails | Components.Schemas.HttpValidationProblemDetails;
        }
    }
    namespace ProvinceGet {
        namespace Responses {
            export type $200 = Components.Schemas.Province[];
        }
    }
    namespace PsiGet {
        namespace Parameters {
            export type Id = string;
            export type Name = string;
            export type ProvinceId = string;
        }
        export interface PathParameters {
            id?: Parameters.Id;
        }
        export interface QueryParameters {
            name?: Parameters.Name;
            provinceId?: Parameters.ProvinceId;
        }
        namespace Responses {
            export type $200 = Components.Schemas.PostSecondaryInstitution[];
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
            export interface $200 {
            }
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
    namespace SystemMessageGet {
        namespace Responses {
            export type $200 = Components.Schemas.SystemMessage[];
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
    namespace UserinfoGet {
        namespace Responses {
            export type $200 = Components.Schemas.UserInfo;
            export interface $404 {
            }
        }
    }
    namespace UserinfoPost {
        export type RequestBody = Components.Schemas.UserInfo;
        namespace Responses {
            export interface $200 {
            }
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
        }
    }
    namespace VersionGet {
        namespace Responses {
            export type $200 = Components.Schemas.VersionMetadata;
        }
    }
    namespace WorkExperienceReferencePost {
        export type RequestBody = Components.Schemas.WorkExperienceReferenceSubmissionRequest;
        namespace Responses {
            export interface $200 {
            }
            export type $400 = Components.Schemas.HttpValidationProblemDetails;
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
   * certificationComparison_get - Handles certification comparison queries
   */
  'certificationComparison_get'(
    parameters?: Parameters<Paths.CertificationComparisonGet.QueryParameters & Paths.CertificationComparisonGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CertificationComparisonGet.Responses.$200>
  /**
   * psi_get - Handles psi queries
   */
  'psi_get'(
    parameters?: Parameters<Paths.PsiGet.QueryParameters & Paths.PsiGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.PsiGet.Responses.$200>
  /**
   * systemMessage_get - Handles system messages queries
   */
  'systemMessage_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.SystemMessageGet.Responses.$200>
  /**
   * identificationTypes_get - Handles identification types queries
   */
  'identificationTypes_get'(
    parameters?: Parameters<Paths.IdentificationTypesGet.QueryParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.IdentificationTypesGet.Responses.$200>
  /**
   * recaptcha_site_key_get - Obtains site key for recaptcha
   */
  'recaptcha_site_key_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.RecaptchaSiteKeyGet.Responses.$200>
  /**
   * version_get - Returns the version information
   */
  'version_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.VersionGet.Responses.$200>
  /**
   * profile_get - Gets the current user profile
   */
  'profile_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProfileGet.Responses.$200>
  /**
   * profile_put - Gets the current user profile
   */
  'profile_put'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProfilePut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProfilePut.Responses.$200>
  /**
   * profileVerification_post - Sets user verification Ids
   */
  'profileVerification_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ProfileVerificationPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ProfileVerificationPost.Responses.$200>
  /**
   * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
   */
  'userinfo_get'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.UserinfoGet.Responses.$200>
  /**
   * userinfo_post - Creates or updates the currently logged on user's profile
   */
  'userinfo_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.UserinfoPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.UserinfoPost.Responses.$200>
  /**
   * references_get - Handles references queries
   */
  'references_get'(
    parameters?: Parameters<Paths.ReferencesGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ReferencesGet.Responses.$200>
  /**
   * character_reference_post - Handles character reference submission
   */
  'character_reference_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.CharacterReferencePost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CharacterReferencePost.Responses.$200>
  /**
   * workExperience_reference_post - Handles work experience reference submission
   */
  'workExperience_reference_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.WorkExperienceReferencePost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.WorkExperienceReferencePost.Responses.$200>
  /**
   * reference_optout - Handles reference optout
   */
  'reference_optout'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ReferenceOptout.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ReferenceOptout.Responses.$200>
  /**
   * files_certificate_get - Handles fetching certificate PDF's
   */
  'files_certificate_get'(
    parameters?: Parameters<Paths.FilesCertificateGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<any>
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
   * message_get - Handles messages queries
   */
  'message_get'(
    parameters?: Parameters<Paths.MessageGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.MessageGet.Responses.$200>
  /**
   * message_post - Handles message send request
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
  /**
   * certification_get - Handles certification queries
   */
  'certification_get'(
    parameters?: Parameters<Paths.CertificationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CertificationGet.Responses.$200>
  /**
   * certification_requestpdf_put - Handles certification queries
   */
  'certification_requestpdf_put'(
    parameters?: Parameters<Paths.CertificationRequestpdfPut.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CertificationRequestpdfPut.Responses.$200>
  /**
   * certifications_lookup_post - Handles certifications lookup queries
   */
  'certifications_lookup_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.CertificationsLookupPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.CertificationsLookupPost.Responses.$200>
  /**
   * draftapplication_put - Save a draft application for the current user
   */
  'draftapplication_put'(
    parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
    data?: Paths.DraftapplicationPut.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>
  /**
   * application_post - Submit an application
   */
  'application_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ApplicationPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationPost.Responses.$200>
  /**
   * application_get - Handles application queries
   */
  'application_get'(
    parameters?: Parameters<Paths.ApplicationGet.QueryParameters & Paths.ApplicationGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationGet.Responses.$200>
  /**
   * application_status_get - Handles application status queries
   */
  'application_status_get'(
    parameters?: Parameters<Paths.ApplicationStatusGet.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationStatusGet.Responses.$200>
  /**
   * draftapplication_delete - Cancel a draft application for the current user
   * 
   * Changes status to cancelled
   */
  'draftapplication_delete'(
    parameters?: Parameters<Paths.DraftapplicationDelete.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.DraftapplicationDelete.Responses.$200>
  /**
   * application_workexperiencereference_update_post - Update work experience reference
   */
  'application_workexperiencereference_update_post'(
    parameters?: Parameters<Paths.ApplicationWorkexperiencereferenceUpdatePost.PathParameters> | null,
    data?: Paths.ApplicationWorkexperiencereferenceUpdatePost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationWorkexperiencereferenceUpdatePost.Responses.$200>
  /**
   * application_characterreference_update_post - Update character reference
   */
  'application_characterreference_update_post'(
    parameters?: Parameters<Paths.ApplicationCharacterreferenceUpdatePost.PathParameters> | null,
    data?: Paths.ApplicationCharacterreferenceUpdatePost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationCharacterreferenceUpdatePost.Responses.$200>
  /**
   * application_character_reference_resend_invite_post - Resend a character reference invite
   * 
   * Changes character reference invite again status to true
   */
  'application_character_reference_resend_invite_post'(
    parameters?: Parameters<Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationCharacterReferenceResendInvitePost.Responses.$200>
  /**
   * application_work_experience_reference_resend_invite_post - Resend a work experience reference invite
   * 
   * Changes work experience reference invite again status to true
   */
  'application_work_experience_reference_resend_invite_post'(
    parameters?: Parameters<Paths.ApplicationWorkExperienceReferenceResendInvitePost.PathParameters> | null,
    data?: any,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationWorkExperienceReferenceResendInvitePost.Responses.$200>
  /**
   * application_professionaldevelopment_add_post - Add Professional Development
   */
  'application_professionaldevelopment_add_post'(
    parameters?: Parameters<Paths.ApplicationProfessionaldevelopmentAddPost.PathParameters> | null,
    data?: Paths.ApplicationProfessionaldevelopmentAddPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationProfessionaldevelopmentAddPost.Responses.$200>
  /**
   * application_update_transcript_post - Save application transcript documents and options
   */
  'application_update_transcript_post'(
    parameters?: Parameters<UnknownParamsObject> | null,
    data?: Paths.ApplicationUpdateTranscriptPost.RequestBody,
    config?: AxiosRequestConfig  
  ): OperationResponse<Paths.ApplicationUpdateTranscriptPost.Responses.$200>
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
  ['/api/certificationComparison/{id}']: {
    /**
     * certificationComparison_get - Handles certification comparison queries
     */
    'get'(
      parameters?: Parameters<Paths.CertificationComparisonGet.QueryParameters & Paths.CertificationComparisonGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CertificationComparisonGet.Responses.$200>
  }
  ['/api/postSecondaryInstitutionList/{id}']: {
    /**
     * psi_get - Handles psi queries
     */
    'get'(
      parameters?: Parameters<Paths.PsiGet.QueryParameters & Paths.PsiGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.PsiGet.Responses.$200>
  }
  ['/api/systemMessages']: {
    /**
     * systemMessage_get - Handles system messages queries
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.SystemMessageGet.Responses.$200>
  }
  ['/api/identificationTypes']: {
    /**
     * identificationTypes_get - Handles identification types queries
     */
    'get'(
      parameters?: Parameters<Paths.IdentificationTypesGet.QueryParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.IdentificationTypesGet.Responses.$200>
  }
  ['/api/recaptchaSiteKey']: {
    /**
     * recaptcha_site_key_get - Obtains site key for recaptcha
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.RecaptchaSiteKeyGet.Responses.$200>
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
  ['/api/profile']: {
    /**
     * profile_get - Gets the current user profile
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProfileGet.Responses.$200>
    /**
     * profile_put - Gets the current user profile
     */
    'put'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProfilePut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProfilePut.Responses.$200>
  }
  ['/api/profile/verificationIds']: {
    /**
     * profileVerification_post - Sets user verification Ids
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ProfileVerificationPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ProfileVerificationPost.Responses.$200>
  }
  ['/api/userinfo']: {
    /**
     * userinfo_get - Gets the currently logged in user profile or NotFound if no profile found
     */
    'get'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.UserinfoGet.Responses.$200>
    /**
     * userinfo_post - Creates or updates the currently logged on user's profile
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.UserinfoPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.UserinfoPost.Responses.$200>
  }
  ['/api/PortalInvitations/{token}']: {
    /**
     * references_get - Handles references queries
     */
    'get'(
      parameters?: Parameters<Paths.ReferencesGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ReferencesGet.Responses.$200>
  }
  ['/api/References/Character']: {
    /**
     * character_reference_post - Handles character reference submission
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.CharacterReferencePost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CharacterReferencePost.Responses.$200>
  }
  ['/api/References/WorkExperience']: {
    /**
     * workExperience_reference_post - Handles work experience reference submission
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.WorkExperienceReferencePost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.WorkExperienceReferencePost.Responses.$200>
  }
  ['/api/References/OptOut']: {
    /**
     * reference_optout - Handles reference optout
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ReferenceOptout.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ReferenceOptout.Responses.$200>
  }
  ['/api/files/certificate/{certificateId}']: {
    /**
     * files_certificate_get - Handles fetching certificate PDF's
     */
    'get'(
      parameters?: Parameters<Paths.FilesCertificateGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<any>
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
  ['/api/messages/{parentId}']: {
    /**
     * message_get - Handles messages queries
     */
    'get'(
      parameters?: Parameters<Paths.MessageGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.MessageGet.Responses.$200>
  }
  ['/api/messages']: {
    /**
     * message_post - Handles message send request
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
  ['/api/certifications/{id}']: {
    /**
     * certification_get - Handles certification queries
     */
    'get'(
      parameters?: Parameters<Paths.CertificationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CertificationGet.Responses.$200>
  }
  ['/api/certifications/RequestPdf/{id}']: {
    /**
     * certification_requestpdf_put - Handles certification queries
     */
    'put'(
      parameters?: Parameters<Paths.CertificationRequestpdfPut.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CertificationRequestpdfPut.Responses.$200>
  }
  ['/api/certifications/lookup']: {
    /**
     * certifications_lookup_post - Handles certifications lookup queries
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.CertificationsLookupPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.CertificationsLookupPost.Responses.$200>
  }
  ['/api/draftapplications/{id}']: {
    /**
     * draftapplication_put - Save a draft application for the current user
     */
    'put'(
      parameters?: Parameters<Paths.DraftapplicationPut.PathParameters> | null,
      data?: Paths.DraftapplicationPut.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.DraftapplicationPut.Responses.$200>
  }
  ['/api/applications']: {
    /**
     * application_post - Submit an application
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ApplicationPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationPost.Responses.$200>
  }
  ['/api/applications/{id}']: {
    /**
     * application_get - Handles application queries
     */
    'get'(
      parameters?: Parameters<Paths.ApplicationGet.QueryParameters & Paths.ApplicationGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationGet.Responses.$200>
  }
  ['/api/applications/{id}/status']: {
    /**
     * application_status_get - Handles application status queries
     */
    'get'(
      parameters?: Parameters<Paths.ApplicationStatusGet.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationStatusGet.Responses.$200>
  }
  ['/api/draftApplications/{id}']: {
    /**
     * draftapplication_delete - Cancel a draft application for the current user
     * 
     * Changes status to cancelled
     */
    'delete'(
      parameters?: Parameters<Paths.DraftapplicationDelete.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.DraftapplicationDelete.Responses.$200>
  }
  ['/api/applications/{application_id}/workexperiencereference/{reference_id}']: {
    /**
     * application_workexperiencereference_update_post - Update work experience reference
     */
    'post'(
      parameters?: Parameters<Paths.ApplicationWorkexperiencereferenceUpdatePost.PathParameters> | null,
      data?: Paths.ApplicationWorkexperiencereferenceUpdatePost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationWorkexperiencereferenceUpdatePost.Responses.$200>
  }
  ['/api/applications/{application_id}/characterreference/{reference_id}']: {
    /**
     * application_characterreference_update_post - Update character reference
     */
    'post'(
      parameters?: Parameters<Paths.ApplicationCharacterreferenceUpdatePost.PathParameters> | null,
      data?: Paths.ApplicationCharacterreferenceUpdatePost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationCharacterreferenceUpdatePost.Responses.$200>
  }
  ['/api/applications/{applicationId}/characterReference/{referenceId}/resendInvite']: {
    /**
     * application_character_reference_resend_invite_post - Resend a character reference invite
     * 
     * Changes character reference invite again status to true
     */
    'post'(
      parameters?: Parameters<Paths.ApplicationCharacterReferenceResendInvitePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationCharacterReferenceResendInvitePost.Responses.$200>
  }
  ['/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite']: {
    /**
     * application_work_experience_reference_resend_invite_post - Resend a work experience reference invite
     * 
     * Changes work experience reference invite again status to true
     */
    'post'(
      parameters?: Parameters<Paths.ApplicationWorkExperienceReferenceResendInvitePost.PathParameters> | null,
      data?: any,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationWorkExperienceReferenceResendInvitePost.Responses.$200>
  }
  ['/api/applications/{application_id}/professionaldevelopment/add']: {
    /**
     * application_professionaldevelopment_add_post - Add Professional Development
     */
    'post'(
      parameters?: Parameters<Paths.ApplicationProfessionaldevelopmentAddPost.PathParameters> | null,
      data?: Paths.ApplicationProfessionaldevelopmentAddPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationProfessionaldevelopmentAddPost.Responses.$200>
  }
  ['/api/applications/{application_id}/transcriptDocuments']: {
    /**
     * application_update_transcript_post - Save application transcript documents and options
     */
    'post'(
      parameters?: Parameters<UnknownParamsObject> | null,
      data?: Paths.ApplicationUpdateTranscriptPost.RequestBody,
      config?: AxiosRequestConfig  
    ): OperationResponse<Paths.ApplicationUpdateTranscriptPost.Responses.$200>
  }
}

export type Client = OpenAPIClient<OperationMethods, PathsDictionary>


export type AddProfessionalDevelopmentResponse = Components.Schemas.AddProfessionalDevelopmentResponse;
export type Address = Components.Schemas.Address;
export type Application = Components.Schemas.Application;
export type ApplicationConfiguration = Components.Schemas.ApplicationConfiguration;
export type ApplicationOrigin = Components.Schemas.ApplicationOrigin;
export type ApplicationStatus = Components.Schemas.ApplicationStatus;
export type ApplicationStatusReasonDetail = Components.Schemas.ApplicationStatusReasonDetail;
export type ApplicationSubmissionRequest = Components.Schemas.ApplicationSubmissionRequest;
export type ApplicationTypes = Components.Schemas.ApplicationTypes;
export type CancelDraftApplicationResponse = Components.Schemas.CancelDraftApplicationResponse;
export type CertificateCondition = Components.Schemas.CertificateCondition;
export type CertificateInformation = Components.Schemas.CertificateInformation;
export type CertificatePDFGeneration = Components.Schemas.CertificatePDFGeneration;
export type CertificateStatusCode = Components.Schemas.CertificateStatusCode;
export type Certification = Components.Schemas.Certification;
export type CertificationComparison = Components.Schemas.CertificationComparison;
export type CertificationFile = Components.Schemas.CertificationFile;
export type CertificationLevel = Components.Schemas.CertificationLevel;
export type CertificationLookupRequest = Components.Schemas.CertificationLookupRequest;
export type CertificationLookupResponse = Components.Schemas.CertificationLookupResponse;
export type CertificationType = Components.Schemas.CertificationType;
export type CharacterReference = Components.Schemas.CharacterReference;
export type CharacterReferenceEvaluation = Components.Schemas.CharacterReferenceEvaluation;
export type CharacterReferenceStage = Components.Schemas.CharacterReferenceStage;
export type CharacterReferenceStatus = Components.Schemas.CharacterReferenceStatus;
export type CharacterReferenceSubmissionRequest = Components.Schemas.CharacterReferenceSubmissionRequest;
export type ChildcareAgeRanges = Components.Schemas.ChildcareAgeRanges;
export type ChildrenProgramType = Components.Schemas.ChildrenProgramType;
export type Communication = Components.Schemas.Communication;
export type CommunicationDocument = Components.Schemas.CommunicationDocument;
export type CommunicationResponse = Components.Schemas.CommunicationResponse;
export type CommunicationSeenRequest = Components.Schemas.CommunicationSeenRequest;
export type CommunicationStatus = Components.Schemas.CommunicationStatus;
export type CommunicationsStatus = Components.Schemas.CommunicationsStatus;
export type CommunicationsStatusResults = Components.Schemas.CommunicationsStatusResults;
export type ComparisonRecord = Components.Schemas.ComparisonRecord;
export type ComprehensiveReportOptions = Components.Schemas.ComprehensiveReportOptions;
export type Country = Components.Schemas.Country;
export type CourseOutlineOptions = Components.Schemas.CourseOutlineOptions;
export type DraftApplication = Components.Schemas.DraftApplication;
export type DraftApplicationResponse = Components.Schemas.DraftApplicationResponse;
export type EducationOrigin = Components.Schemas.EducationOrigin;
export type EducationRecognition = Components.Schemas.EducationRecognition;
export type FileInfo = Components.Schemas.FileInfo;
export type FileResponse = Components.Schemas.FileResponse;
export type FiveYearRenewalExplanations = Components.Schemas.FiveYearRenewalExplanations;
export type GetMessagesResponse = Components.Schemas.GetMessagesResponse;
export type HttpValidationProblemDetails = Components.Schemas.HttpValidationProblemDetails;
export type IdentificationType = Components.Schemas.IdentificationType;
export type IdentityDocument = Components.Schemas.IdentityDocument;
export type InitiatedFrom = Components.Schemas.InitiatedFrom;
export type InviteType = Components.Schemas.InviteType;
export type LikertScale = Components.Schemas.LikertScale;
export type OidcAuthenticationSettings = Components.Schemas.OidcAuthenticationSettings;
export type OneYearRenewalexplanations = Components.Schemas.OneYearRenewalexplanations;
export type OptOutReferenceRequest = Components.Schemas.OptOutReferenceRequest;
export type OutOfProvinceCertificationType = Components.Schemas.OutOfProvinceCertificationType;
export type PortalInvitation = Components.Schemas.PortalInvitation;
export type PortalInvitationQueryResult = Components.Schemas.PortalInvitationQueryResult;
export type PortalTags = Components.Schemas.PortalTags;
export type PostSecondaryInstitution = Components.Schemas.PostSecondaryInstitution;
export type PreviousName = Components.Schemas.PreviousName;
export type PreviousNameSources = Components.Schemas.PreviousNameSources;
export type PreviousNameStage = Components.Schemas.PreviousNameStage;
export type ProblemDetails = Components.Schemas.ProblemDetails;
export type ProfessionalDevelopment = Components.Schemas.ProfessionalDevelopment;
export type ProfessionalDevelopmentStatus = Components.Schemas.ProfessionalDevelopmentStatus;
export type ProfessionalDevelopmentStatusCode = Components.Schemas.ProfessionalDevelopmentStatusCode;
export type ProfileIdentification = Components.Schemas.ProfileIdentification;
export type ProgramConfirmationOptions = Components.Schemas.ProgramConfirmationOptions;
export type Province = Components.Schemas.Province;
export type ReferenceContactInformation = Components.Schemas.ReferenceContactInformation;
export type ReferenceKnownTime = Components.Schemas.ReferenceKnownTime;
export type ReferenceRelationship = Components.Schemas.ReferenceRelationship;
export type ResendReferenceInviteResponse = Components.Schemas.ResendReferenceInviteResponse;
export type SaveDraftApplicationRequest = Components.Schemas.SaveDraftApplicationRequest;
export type SendMessageRequest = Components.Schemas.SendMessageRequest;
export type SendMessageResponse = Components.Schemas.SendMessageResponse;
export type StatusCode = Components.Schemas.StatusCode;
export type SubmitApplicationResponse = Components.Schemas.SubmitApplicationResponse;
export type SubmittedApplicationStatus = Components.Schemas.SubmittedApplicationStatus;
export type SystemMessage = Components.Schemas.SystemMessage;
export type Transcript = Components.Schemas.Transcript;
export type TranscriptDocuments = Components.Schemas.TranscriptDocuments;
export type TranscriptStage = Components.Schemas.TranscriptStage;
export type TranscriptStatus = Components.Schemas.TranscriptStatus;
export type TranscriptStatusOptions = Components.Schemas.TranscriptStatusOptions;
export type UnabletoProvideReferenceReasons = Components.Schemas.UnabletoProvideReferenceReasons;
export type UpdateReferenceResponse = Components.Schemas.UpdateReferenceResponse;
export type UserInfo = Components.Schemas.UserInfo;
export type UserProfile = Components.Schemas.UserProfile;
export type VersionMetadata = Components.Schemas.VersionMetadata;
export type WorkExperienceRefStage = Components.Schemas.WorkExperienceRefStage;
export type WorkExperienceReference = Components.Schemas.WorkExperienceReference;
export type WorkExperienceReferenceCompetenciesAssessment = Components.Schemas.WorkExperienceReferenceCompetenciesAssessment;
export type WorkExperienceReferenceDetails = Components.Schemas.WorkExperienceReferenceDetails;
export type WorkExperienceReferenceStatus = Components.Schemas.WorkExperienceReferenceStatus;
export type WorkExperienceReferenceSubmissionRequest = Components.Schemas.WorkExperienceReferenceSubmissionRequest;
export type WorkExperienceTypes = Components.Schemas.WorkExperienceTypes;
export type WorkHoursType = Components.Schemas.WorkHoursType;
export type YesNoNull = Components.Schemas.YesNoNull;
