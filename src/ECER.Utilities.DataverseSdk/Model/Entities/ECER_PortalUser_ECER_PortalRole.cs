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
	
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("ecer_portaluser_ecer_portalrole")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class ecer_PortalUser_ecer_PortalRole : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the ecer_portaluser_ecer_portalrole entity
		/// </summary>
		public partial class Fields
		{
			public const string ecer_portalroleid = "ecer_portalroleid";
			public const string ecer_PortalUser_ecer_PortalRole1 = "ecer_PortalUser_ecer_PortalRole";
			public const string ecer_PortalUser_ecer_PortalRoleId = "ecer_portaluser_ecer_portalroleid";
			public const string Id = "ecer_portaluser_ecer_portalroleid";
			public const string ecer_portaluserid = "ecer_portaluserid";
			public const string VersionNumber = "versionnumber";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_PortalUser_ecer_PortalRole(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_PortalUser_ecer_PortalRole(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_PortalUser_ecer_PortalRole(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_PortalUser_ecer_PortalRole() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "ecer_portaluser_ecer_portalroleid";
		
		public const string EntitySchemaName = "ecer_PortalUser_ecer_PortalRole";
		
		public const string EntityLogicalName = "ecer_portaluser_ecer_portalrole";
		
		public const string EntityLogicalCollectionName = null;
		
		public const string EntitySetName = "ecer_portaluser_ecer_portalroleset";
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_portalroleid")]
		public System.Nullable<System.Guid> ecer_portalroleid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ecer_portalroleid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_portaluser_ecer_portalroleid")]
		public System.Nullable<System.Guid> ecer_PortalUser_ecer_PortalRoleId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ecer_portaluser_ecer_portalroleid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_portaluser_ecer_portalroleid")]
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
				base.Id = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ecer_portaluserid")]
		public System.Nullable<System.Guid> ecer_portaluserid
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("ecer_portaluserid");
			}
		}
		
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
		/// N:N ecer_PortalUser_ecer_PortalRole
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("ecer_PortalUser_ecer_PortalRole")]
		public System.Collections.Generic.IEnumerable<ECER.Utilities.DataverseSdk.Model.ecer_PortalRole> ecer_PortalUser_ecer_PortalRole1
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_PortalRole>("ecer_PortalUser_ecer_PortalRole", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<ECER.Utilities.DataverseSdk.Model.ecer_PortalRole>("ecer_PortalUser_ecer_PortalRole", null, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public ecer_PortalUser_ecer_PortalRole(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (value != null && name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["ecer_portaluser_ecer_portalroleid"] = base.Id;
                        break;
                    case "ecer_portaluser_ecer_portalroleid":
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
