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
	/// Status of the Communication
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
	public enum ecer_communication_statecode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 1,
	}
	
	/// <summary>
	/// Reason for the status of the Communication
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
	public enum ecer_Communication_StatusCode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Acknowledged = 621870002,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Draft = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Inactive = 2,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		NotifiedRecipient = 621870001,
	}
	
	/// <summary>
	/// Provide 2-way communication between Registrant and Assessor at Portal
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ecer_communication")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.6")]
	public partial class ecer_Communication : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the ecer_communication entity
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
			public const string ecer_Acknowledged = "ecer_acknowledged";
			public const string ecer_acknowledgedName = "ecer_acknowledgedname";
			public const string ecer_Applicationid = "ecer_applicationid";
			public const string ecer_ApplicationidName = "ecer_applicationidname";
			public const string ecer_communication_Applicationid = "ecer_communication_Applicationid";
			public const string ecer_communication_Investigation_ecer_investig = "ecer_communication_Investigation_ecer_investig";
			public const string Referencingecer_communication_ParentCommunicationid = "ecer_communication_ParentCommunicationid";
			public const string ecer_CommunicationId = "ecer_communicationid";
			public const string Id = "ecer_communicationid";
			public const string ecer_communicationlineitem_Communicationid = "ecer_communicationlineitem_Communicationid";
			public const string ecer_contact_ecer_communication_122 = "ecer_contact_ecer_communication_122";
			public const string ecer_DateAcknowledged = "ecer_dateacknowledged";
			public const string ecer_DateNotified = "ecer_datenotified";
			public const string ecer_FromOracle = "ecer_fromoracle";
			public const string ecer_InitiatedFrom = "ecer_initiatedfrom";
			public const string ecer_initiatedfromName = "ecer_initiatedfromname";
			public const string ecer_Investigation = "ecer_investigation";
			public const string ecer_InvestigationName = "ecer_investigationname";
			public const string ecer_Message = "ecer_message";
			public const string ecer_Name = "ecer_name";
			public const string ecer_NotifyRecipient = "ecer_notifyrecipient";
			public const string ecer_notifyrecipientName = "ecer_notifyrecipientname";
			public const string ecer_ParentCommunicationid = "ecer_parentcommunicationid";
			public const string ecer_ParentCommunicationidName = "ecer_parentcommunicationidname";
			public const string ecer_Registrantid = "ecer_registrantid";
			public const string ecer_RegistrantidName = "ecer_registrantidname";
			public const string ecer_RegistrantidYomiName = "ecer_registrantidyominame";
			public const string ecer_Reply = "ecer_reply";
			public const string ecer_Type = "ecer_type";
			public const string ecer_typeName = "ecer_typename";
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
			public const string Referencedecer_communication_ParentCommunicationid = "Referencedecer_communication_ParentCommunicationid";
			public const string StateCode = "statecode";
			public const string statecodeName = "statecodename";
			public const string StatusCode = "statuscode";
			public const string statuscodeName = "statuscodename";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Communication(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Communication(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Communication(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Communication() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "ecer_communicationid";
		
		public const string PrimaryNameAttribute = "ecer_name";
		
		public const string EntitySchemaName = "ecer_Communication";
		
		public const string EntityLogicalName = "ecer_communication";
		
		public const string EntityLogicalCollectionName = "ecer_communications";
		
		public const string EntitySetName = "ecer_communications";
		
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
		
		/// <summary>
		/// Recipient acknowledge message read
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_acknowledged")]
		public System.Nullable<bool> ecer_Acknowledged
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_acknowledged");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_acknowledged", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_acknowledgedname")]
		public string ecer_acknowledgedName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_acknowledged"))
				{
					return this.FormattedValues["ecer_acknowledged"];
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
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_communicationid")]
		public System.Nullable<System.Guid> ecer_CommunicationId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ecer_communicationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_communicationid", value);
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
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_communicationid")]
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
				this.ecer_CommunicationId = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_dateacknowledged")]
		public System.Nullable<System.DateTime> ecer_DateAcknowledged
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ecer_dateacknowledged");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_dateacknowledged", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_datenotified")]
		public System.Nullable<System.DateTime> ecer_DateNotified
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("ecer_datenotified");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_datenotified", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_fromoracle")]
		public string ecer_FromOracle
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_fromoracle");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_fromoracle", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_initiatedfrom")]
		public virtual ecer_InitiatedFrom? ecer_InitiatedFrom
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_InitiatedFrom?)(EntityOptionSetEnum.GetEnum(this, "ecer_initiatedfrom")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_initiatedfrom", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_initiatedfromname")]
		public string ecer_initiatedfromName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_initiatedfrom"))
				{
					return this.FormattedValues["ecer_initiatedfrom"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_investigation")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_Investigation
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_investigation");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_investigation", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_investigationname")]
		public string ecer_InvestigationName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_investigation"))
				{
					return this.FormattedValues["ecer_investigation"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_message")]
		public string ecer_Message
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_message");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_message", value);
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
		
		/// <summary>
		/// Toggle to send email notification to recipient 
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_notifyrecipient")]
		public System.Nullable<bool> ecer_NotifyRecipient
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ecer_notifyrecipient");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_notifyrecipient", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_notifyrecipientname")]
		public string ecer_notifyrecipientName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_notifyrecipient"))
				{
					return this.FormattedValues["ecer_notifyrecipient"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Self Referred to allow thread communications
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_parentcommunicationid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_ParentCommunicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_parentcommunicationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_parentcommunicationid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_parentcommunicationidname")]
		public string ecer_ParentCommunicationidName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_parentcommunicationid"))
				{
					return this.FormattedValues["ecer_parentcommunicationid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_registrantid")]
		public Microsoft.Xrm.Sdk.EntityReference ecer_Registrantid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ecer_registrantid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_registrantid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_registrantidname")]
		public string ecer_RegistrantidName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_registrantid"))
				{
					return this.FormattedValues["ecer_registrantid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_registrantidyominame")]
		public string ecer_RegistrantidYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_registrantid"))
				{
					return this.FormattedValues["ecer_registrantid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_reply")]
		public string ecer_Reply
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("ecer_reply");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_reply", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_type")]
		public virtual ecer_CommunicationTypes? ecer_Type
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_CommunicationTypes?)(EntityOptionSetEnum.GetEnum(this, "ecer_type")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ecer_type", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_typename")]
		public string ecer_typeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ecer_type"))
				{
					return this.FormattedValues["ecer_type"];
				}
				else
				{
					return default(string);
				}
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
		/// Status of the Communication
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public virtual ecer_communication_statecode? StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_communication_statecode?)(EntityOptionSetEnum.GetEnum(this, "statecode")));
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
		/// Reason for the status of the Communication
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual ecer_Communication_StatusCode? StatusCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ecer_Communication_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// 1:N ecer_communication_ParentCommunicationid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<ECER.Utilities.DataverseSdk.Model.ecer_Communication> Referencedecer_communication_ParentCommunicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_Communication>("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_Communication>("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
			}
		}
		
		/// <summary>
		/// 1:N ecer_communicationlineitem_Communicationid
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_communicationlineitem_Communicationid")]
		public System.Collections.Generic.IEnumerable<ECER.Utilities.DataverseSdk.Model.ecer_CommunicationLineItem> ecer_communicationlineitem_Communicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_CommunicationLineItem>("ecer_communicationlineitem_Communicationid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_CommunicationLineItem>("ecer_communicationlineitem_Communicationid", null, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_communication_Applicationid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_applicationid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_communication_Applicationid")]
		public ECER.Utilities.DataverseSdk.Model.ecer_Application ecer_communication_Applicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Application>("ecer_communication_Applicationid", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Application>("ecer_communication_Applicationid", null, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_communication_Investigation_ecer_investig
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_investigation")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_communication_Investigation_ecer_investig")]
		public ECER.Utilities.DataverseSdk.Model.ecer_Investigation ecer_communication_Investigation_ecer_investig
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Investigation>("ecer_communication_Investigation_ecer_investig", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Investigation>("ecer_communication_Investigation_ecer_investig", null, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_communication_ParentCommunicationid
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_parentcommunicationid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public ECER.Utilities.DataverseSdk.Model.ecer_Communication Referencingecer_communication_ParentCommunicationid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Communication>("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.ecer_Communication>("ecer_communication_ParentCommunicationid", Microsoft.Xrm.Sdk.EntityRole.Referencing, value);
			}
		}
		
		/// <summary>
		/// N:1 ecer_contact_ecer_communication_122
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_registrantid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_contact_ecer_communication_122")]
		public ECER.Utilities.DataverseSdk.Model.Contact ecer_contact_ecer_communication_122
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<ECER.Utilities.DataverseSdk.Model.Contact>("ecer_contact_ecer_communication_122", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<ECER.Utilities.DataverseSdk.Model.Contact>("ecer_contact_ecer_communication_122", null, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_Communication(object anonymousType) : 
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
                        Attributes["ecer_communicationid"] = base.Id;
                        break;
                    case "ecer_communicationid":
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
