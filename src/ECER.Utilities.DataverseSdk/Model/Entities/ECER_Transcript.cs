#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECER.Utilities.DataverseSdk.Model
{
	
	
	/// <summary>
	/// Status of the Transcript
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public enum ecer_transcript_statecode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Reason for the status of the Transcript
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public enum ecer_Transcript_StatusCode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Accepted = 2,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		ApplicationSubmitted = 621870006,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Draft = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		InProgress = 621870002,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Rejected = 621870005,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Submitted = 621870001,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		WaitingforDetails = 621870007,
	}
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ecer_transcript")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class ecer_Transcript : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the ecer_transcript entity
		/// </summary>
		public partial class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedByName = "createdbyname";
			public const string CreatedByYomiName = "createdbyyominame";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CreatedOnBehalfByName = "createdonbehalfbyname";
			public const string CreatedOnBehalfByYomiName = "createdonbehalfbyyominame";
			public const string ecer_Applicantid = "ecer_applicantid";
			public const string ecer_ApplicantidName = "ecer_applicantidname";
			public const string ecer_ApplicantidYomiName = "ecer_applicantidyominame";
			public const string ecer_Applicationid = "ecer_applicationid";
			public const string ecer_ApplicationidName = "ecer_applicationidname";
			public const string ecer_CampusLocation = "ecer_campuslocation";
			public const string ecer_completedcourse_Transcriptid = "ecer_completedcourse_Transcriptid";
			public const string ecer_CompletedOnline = "ecer_completedonline";
			public const string ecer_completedonlineName = "ecer_completedonlinename";
			public const string ecer_DoesECERegistryHaveTranscript = "ecer_doeseceregistryhavetranscript";
			public const string ecer_doeseceregistryhavetranscriptName = "ecer_doeseceregistryhavetranscriptname";
			public const string ecer_EducationInstitutionFullName = "ecer_educationinstitutionfullname";
			public const string ecer_EndDate = "ecer_enddate";
			public const string ecer_IsECEAssistant = "ecer_iseceassistant";
			public const string ecer_iseceassistantName = "ecer_iseceassistantname";
			public const string ecer_IsOfficialTranscriptRequested = "ecer_isofficialtranscriptrequested";
			public const string ecer_isofficialtranscriptrequestedName = "ecer_isofficialtranscriptrequestedname";
			public const string ecer_LanguageofInstruction = "ecer_languageofinstruction";
			public const string ecer_legacyeducationtranscriptid = "ecer_legacyeducationtranscriptid";
			public const string ecer_Name = "ecer_name";
			public const string ecer_PostSecondaryInstituteid = "ecer_postsecondaryinstituteid";
			public const string ecer_PostSecondaryInstituteidName = "ecer_postsecondaryinstituteidname";
			public const string ecer_PostSecondaryInstituteidYomiName = "ecer_postsecondaryinstituteidyominame";
			public const string ecer_ProgramCourseName = "ecer_programcoursename";
			public const string ecer_StartDate = "ecer_startdate";
			public const string ecer_StudentName = "ecer_studentname";
			public const string ecer_StudentNumber = "ecer_studentnumber";
			public const string ecer_transcript_Applicantid_Contact = "ecer_transcript_Applicantid_Contact";
			public const string ecer_transcript_Applicationid = "ecer_transcript_Applicationid";
			public const string ecer_TranscriptDocumentId = "ecer_transcriptdocumentid";
			public const string ecer_TranscriptDocumentIdName = "ecer_transcriptdocumentidname";
			public const string ecer_TranscriptId = "ecer_transcriptid";
			public const string Id = "ecer_transcriptid";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByName = "modifiedbyname";
			public const string ModifiedByYomiName = "modifiedbyyominame";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ModifiedOnBehalfByName = "modifiedonbehalfbyname";
			public const string ModifiedOnBehalfByYomiName = "modifiedonbehalfbyyominame";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OwnerId = "ownerid";
			public const string OwnerIdName = "owneridname";
			public const string OwnerIdYomiName = "owneridyominame";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningBusinessUnitName = "owningbusinessunitname";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string StateCode = "statecode";
			public const string statecodeName = "statecodename";
			public const string StatusCode = "statuscode";
			public const string statuscodeName = "statuscodename";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Transcript(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Transcript(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Transcript(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		public const string AlternateKeys = "ecer_legacyeducationtranscriptid";
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Transcript() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "ecer_transcriptid";
		
		public const string PrimaryNameAttribute = "ecer_name";
		
		public const string EntitySchemaName = "ecer_Transcript";
		
		public const string EntityLogicalName = "ecer_transcript";
		
		public const string EntityLogicalCollectionName = "ecer_transcripts";
		
		public const string EntitySetName = "ecer_transcripts";
		
		/// <summary>
		/// Unique identifier of the user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyname")]
		public string CreatedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyyominame")]
		public string CreatedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("createdonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyname")]
		public string CreatedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyyominame")]
		public string CreatedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicantid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_Applicantid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_applicantid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_applicantid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicantidname")]
		public string ecer_ApplicantidName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_applicantid"))
				{
					return this.FormattedValues["ecer_applicantid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicantidyominame")]
		public string ecer_ApplicantidYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_applicantid"))
				{
					return this.FormattedValues["ecer_applicantid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicationid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_Applicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_applicationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_applicationid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicationidname")]
		public string ecer_ApplicationidName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_applicationid"))
				{
					return this.FormattedValues["ecer_applicationid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_campuslocation")]
		public string ecer_CampusLocation
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_campuslocation");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_campuslocation", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_completedonline")]
		public System.Nullable<bool> ecer_CompletedOnline
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_completedonline");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_completedonline", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_completedonlinename")]
		public string ecer_completedonlineName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_completedonline"))
				{
					return this.FormattedValues["ecer_completedonline"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_doeseceregistryhavetranscript")]
		public System.Nullable<bool> ecer_DoesECERegistryHaveTranscript
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_doeseceregistryhavetranscript");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_doeseceregistryhavetranscript", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_doeseceregistryhavetranscriptname")]
		public string ecer_doeseceregistryhavetranscriptName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_doeseceregistryhavetranscript"))
				{
					return this.FormattedValues["ecer_doeseceregistryhavetranscript"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_educationinstitutionfullname")]
		public string ecer_EducationInstitutionFullName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_educationinstitutionfullname");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_educationinstitutionfullname", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_enddate")]
		public System.Nullable<System.DateTime> ecer_EndDate
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ecer_enddate");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_enddate", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_iseceassistant")]
		public System.Nullable<bool> ecer_IsECEAssistant
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_iseceassistant");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_iseceassistant", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_iseceassistantname")]
		public string ecer_iseceassistantName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_iseceassistant"))
				{
					return this.FormattedValues["ecer_iseceassistant"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_isofficialtranscriptrequested")]
		public System.Nullable<bool> ecer_IsOfficialTranscriptRequested
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_isofficialtranscriptrequested");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_isofficialtranscriptrequested", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_isofficialtranscriptrequestedname")]
		public string ecer_isofficialtranscriptrequestedName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_isofficialtranscriptrequested"))
				{
					return this.FormattedValues["ecer_isofficialtranscriptrequested"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_languageofinstruction")]
		public string ecer_LanguageofInstruction
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_languageofinstruction");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_languageofinstruction", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_legacyeducationtranscriptid")]
		public string ecer_legacyeducationtranscriptid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_legacyeducationtranscriptid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_legacyeducationtranscriptid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_name")]
		public string ecer_Name
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_name");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_name", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_postsecondaryinstituteid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_PostSecondaryInstituteid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_postsecondaryinstituteid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_postsecondaryinstituteid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_postsecondaryinstituteidname")]
		public string ecer_PostSecondaryInstituteidName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_postsecondaryinstituteid"))
				{
					return this.FormattedValues["ecer_postsecondaryinstituteid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_postsecondaryinstituteidyominame")]
		public string ecer_PostSecondaryInstituteidYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_postsecondaryinstituteid"))
				{
					return this.FormattedValues["ecer_postsecondaryinstituteid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_programcoursename")]
		public string ecer_ProgramCourseName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_programcoursename");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_programcoursename", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_startdate")]
		public System.Nullable<System.DateTime> ecer_StartDate
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ecer_startdate");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_startdate", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_studentname")]
		public string ecer_StudentName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_studentname");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_studentname", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_studentnumber")]
		public string ecer_StudentNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_studentnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_studentnumber", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_transcriptdocumentid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_TranscriptDocumentId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_transcriptdocumentid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_transcriptdocumentid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_transcriptdocumentidname")]
		public string ecer_TranscriptDocumentIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_transcriptdocumentid"))
				{
					return this.FormattedValues["ecer_transcriptdocumentid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_transcriptid")]
		public System.Nullable<System.Guid> ecer_TranscriptId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ecer_transcriptid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_transcriptid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_transcriptid")]
		public override System.Guid Id
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return base.Id;
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.ecer_TranscriptId = value;
			}
		}
		
		/// <summary>
		/// Sequence number of the import that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("importsequencenumber", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyname")]
		public string ModifiedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyyominame")]
		public string ModifiedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time when the record was modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("modifiedonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyname")]
		public string ModifiedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyyominame")]
		public string ModifiedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("overriddencreatedon", value);
			}
		}
		
		/// <summary>
		/// Owner Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
		public Microsoft.Xrm.Sdk.EntityReference OwnerId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ownerid", value);
			}
		}
		
		/// <summary>
		/// Name of the owner
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owneridname")]
		public string OwnerIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ownerid"))
				{
					return this.FormattedValues["ownerid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Yomi name of the owner
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owneridyominame")]
		public string OwnerIdYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ownerid"))
				{
					return this.FormattedValues["ownerid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for the business unit that owns the record
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		public Microsoft.Xrm.Sdk.EntityReference OwningBusinessUnit
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningbusinessunit");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunitname")]
		public string OwningBusinessUnitName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("owningbusinessunit"))
				{
					return this.FormattedValues["owningbusinessunit"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for the team that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		public Microsoft.Xrm.Sdk.EntityReference OwningTeam
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningteam");
			}
		}
		
		/// <summary>
		/// Unique identifier for the user that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		public Microsoft.Xrm.Sdk.EntityReference OwningUser
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owninguser");
			}
		}
		
		/// <summary>
		/// Status of the Transcript
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public virtual ecer_transcript_statecode? StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_transcript_statecode?)(EntityOptionSetEnum.GetEnum(this, "statecode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("statecode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecodename")]
		public string statecodeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("statecode"))
				{
					return this.FormattedValues["statecode"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Reason for the status of the Transcript
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ecer_Transcript_StatusCode? StatusCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_Transcript_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("statuscode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscodename")]
		public string statuscodeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("statuscode"))
				{
					return this.FormattedValues["statuscode"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("timezoneruleversionnumber")]
		public System.Nullable<int> TimeZoneRuleVersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("timezoneruleversionnumber", value);
			}
		}
		
		/// <summary>
		/// Time zone code that was in use when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("utcconversiontimezonecode")]
		public System.Nullable<int> UTCConversionTimeZoneCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("utcconversiontimezonecode", value);
			}
		}
		
		/// <summary>
		/// Version Number
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// 1:N ecer_completedcourse_Transcriptid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_completedcourse_Transcriptid")]
		public System.Collections.Generic.IEnumerable<ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse> ecer_completedcourse_Transcriptid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse>("ecer_completedcourse_Transcriptid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_CompletedCourse>("ecer_completedcourse_Transcriptid", null, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_transcript_Applicantid_Contact
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicantid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_transcript_Applicantid_Contact")]
		public ECER.Utilities.DataverseSdk.Model.Contact ecer_transcript_Applicantid_Contact
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.Contact>("ecer_transcript_Applicantid_Contact", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.Contact>("ecer_transcript_Applicantid_Contact", null, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_transcript_Applicationid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicationid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_transcript_Applicationid")]
		public ECER.Utilities.DataverseSdk.Model.ecer_Application ecer_transcript_Applicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Application>("ecer_transcript_Applicationid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Application>("ecer_transcript_Applicationid", null, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Transcript(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["ecer_transcriptid"] = base.Id;
                        break;
                    case "ecer_transcriptid":
                        var id = (System.Nullable<System.Guid>) value;
                        if(id == null){ continue; }
                        base.Id = id.Value;
                        Attributes[name] = base.Id;
                        break;
                    case "formattedvalues":
                        // Add Support for FormattedValues
                        FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);
                        break;
                    default:
                        Attributes[name] = value;
                        break;
                }
            }
		}
	}
}
#pragma warning restore CS1591
