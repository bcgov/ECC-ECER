#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute()]

namespace ECER.Utilities.DataverseSdk.Model
{
	
	
	/// <summary>
	/// Represents a source of entities bound to a Dataverse service. It tracks and manages changes made to the retrieved entities.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class EcerContext : Microsoft.Xrm.Sdk.Client.OrganizationServiceContext
	{
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EcerContext(Microsoft.Xrm.Sdk.IOrganizationService service) : 
				base(service)
		{
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.bcgov_config"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.bcgov_config> bcgov_configSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.bcgov_config>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.bcgov_DocumentUrl"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.bcgov_DocumentUrl> bcgov_DocumentUrlSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.bcgov_DocumentUrl>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.bcgov_tag"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.bcgov_tag> bcgov_tagSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.bcgov_tag>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.Contact"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.Contact> ContactSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.Contact>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Allegation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Allegation> ecer_AllegationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Allegation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Application"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Application> ecer_ApplicationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Application>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessment> ecer_ApplicationAssessmentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessmentResult"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessmentResult> ecer_ApplicationAssessmentResultSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationAssessmentResult>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ApplicationHistory"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationHistory> ecer_ApplicationHistorySet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ApplicationHistory>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_AssessmentTrainingLocation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_AssessmentTrainingLocation> ecer_AssessmentTrainingLocationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_AssessmentTrainingLocation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Authentication"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Authentication> ecer_AuthenticationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Authentication>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Certificate"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Certificate> ecer_CertificateSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Certificate>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Certificate_ecer_CertificateType"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Certificate_ecer_CertificateType> ecer_Certificate_ecer_CertificateTypeSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Certificate_ecer_CertificateType>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditions"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditions> ecer_CertificateConditionsSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditions>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditionsPreset"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditionsPreset> ecer_CertificateConditionsPresetSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CertificateConditionsPreset>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CertificateType"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CertificateType> ecer_CertificateTypeSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CertificateType>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_certifications"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_certifications> ecer_certificationsSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_certifications>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CertifiedLevel"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CertifiedLevel> ecer_CertifiedLevelSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CertifiedLevel>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ChangeofInformation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ChangeofInformation> ecer_ChangeofInformationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ChangeofInformation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CharacterReference"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CharacterReference> ecer_CharacterReferenceSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CharacterReference>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_City"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_City> ecer_CitySet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_City>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Comment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Comment> ecer_CommentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Comment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Communication"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Communication> ecer_CommunicationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Communication>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CommunicationContent"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CommunicationContent> ecer_CommunicationContentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CommunicationContent>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CompetencyCompetencies"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CompetencyCompetencies> ecer_CompetencyCompetenciesSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CompetencyCompetencies>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_complainant"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_complainant> ecer_complainantSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_complainant>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse> ecer_CompletedCourseSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Country"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Country> ecer_CountrySet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Country>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Course"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Course> ecer_CourseSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Course>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_DefaultContents"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_DefaultContents> ecer_DefaultContentsSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_DefaultContents>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_DenialReason"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_DenialReason> ecer_DenialReasonSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_DenialReason>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ECEProgramRepresentative"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ECEProgramRepresentative> ecer_ECEProgramRepresentativeSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ECEProgramRepresentative>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_EducationAssessment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_EducationAssessment> ecer_EducationAssessmentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_EducationAssessment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_finding"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_finding> ecer_findingSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_finding>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_GuardianReference"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_GuardianReference> ecer_GuardianReferenceSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_GuardianReference>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_health_authorities"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_health_authorities> ecer_health_authoritiesSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_health_authorities>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Investigation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Investigation> ecer_InvestigationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Investigation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Investigation_ecer_CompetencyCompe"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Investigation_ecer_CompetencyCompe> ecer_Investigation_ecer_CompetencyCompeSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Investigation_ecer_CompetencyCompe>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationHistory"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationHistory> ecer_InvestigationHistorySet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationHistory>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationParallelProcessCommunication"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationParallelProcessCommunication> ecer_InvestigationParallelProcessCommunicationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationParallelProcessCommunication>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation> ecer_InvestigationPlanningAllegationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation_ec"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation_ec> ecer_InvestigationPlanningAllegation_ecSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningAllegation_ec>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningContactsforCollaborative"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningContactsforCollaborative> ecer_InvestigationPlanningContactsforCollaborativeSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningContactsforCollaborative>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningCoreFactualIssue"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningCoreFactualIssue> ecer_InvestigationPlanningCoreFactualIssueSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningCoreFactualIssue>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningDocumentationRecord"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningDocumentationRecord> ecer_InvestigationPlanningDocumentationRecordSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningDocumentationRecord>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInterview"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInterview> ecer_InvestigationPlanningInterviewSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInterview>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInvolvedPerson"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInvolvedPerson> ecer_InvestigationPlanningInvolvedPersonSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningInvolvedPerson>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningRiskConsideration"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningRiskConsideration> ecer_InvestigationPlanningRiskConsiderationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvestigationPlanningRiskConsideration>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_investigationprocess"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_investigationprocess> ecer_investigationprocessSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_investigationprocess>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_InvolvedPerson"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_InvolvedPerson> ecer_InvolvedPersonSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_InvolvedPerson>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_licensing_officers"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_licensing_officers> ecer_licensing_officersSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_licensing_officers>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ParallelProcessCommunication"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ParallelProcessCommunication> ecer_ParallelProcessCommunicationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ParallelProcessCommunication>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PortalInvitation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PortalInvitation> ecer_PortalInvitationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PortalInvitation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PortalRole"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PortalRole> ecer_PortalRoleSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PortalRole>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PortalUser"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PortalUser> ecer_PortalUserSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PortalUser>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PortalUser_ecer_PortalRole"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PortalUser_ecer_PortalRole> ecer_PortalUser_ecer_PortalRoleSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PortalUser_ecer_PortalRole>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstitute"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstitute> ecer_PostSecondaryInstituteSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstitute>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteCampus"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteCampus> ecer_PostSecondaryInstituteCampusSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteCampus>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteInterview"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteInterview> ecer_PostSecondaryInstituteInterviewSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteInterview>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramAppli"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramAppli> ecer_PostSecondaryInstituteProgramAppliSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramAppli>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramApplicaiton"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramApplicaiton> ecer_PostSecondaryInstituteProgramApplicaitonSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteProgramApplicaiton>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteSiteVisit"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteSiteVisit> ecer_PostSecondaryInstituteSiteVisitSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PostSecondaryInstituteSiteVisit>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PreviousAddress"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PreviousAddress> ecer_PreviousAddressSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PreviousAddress>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PreviousName"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PreviousName> ecer_PreviousNameSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PreviousName>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ProfessionalDevelopment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ProfessionalDevelopment> ecer_ProfessionalDevelopmentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ProfessionalDevelopment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Program"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Program> ecer_ProgramSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Program>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_programapplicationbpf"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_programapplicationbpf> ecer_programapplicationbpfSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_programapplicationbpf>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Province"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Province> ecer_ProvinceSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Province>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_ProvincialRequirement"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_ProvincialRequirement> ecer_ProvincialRequirementSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_ProvincialRequirement>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestion"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestion> ecer_PSPInterviewQuestionSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestion>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestionTemplate"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestionTemplate> ecer_PSPInterviewQuestionTemplateSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PSPInterviewQuestionTemplate>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_pspobservation"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_pspobservation> ecer_pspobservationSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_pspobservation>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklist"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklist> ecer_PSPSiteVisitChecklistSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklist>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklistTemplate"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklistTemplate> ecer_PSPSiteVisitChecklistTemplateSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_PSPSiteVisitChecklistTemplate>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_RenewalAssessment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_RenewalAssessment> ecer_RenewalAssessmentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_RenewalAssessment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Signature"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Signature> ecer_SignatureSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Signature>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_Transcript"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_Transcript> ecer_TranscriptSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_Transcript>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceAssessment"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceAssessment> ecer_WorkExperienceAssessmentSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceAssessment>();
			}
		}
		
		/// <summary>
		/// Gets a binding to the set of all <see cref="ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceRef"/> entities.
		/// </summary>
		public System.Linq.IQueryable<ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceRef> ecer_WorkExperienceRefSet
		{
			get
			{
				return this.CreateQuery<ECER.Utilities.DataverseSdk.Model.ecer_WorkExperienceRef>();
			}
		}
	}
}
#pragma warning restore CS1591
