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
	
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/ecer/")]
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("ecer_CLEANUPDeleteContactApplicationsAction")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class ecer_CLEANUPDeleteContactApplicationsActionRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
		public string ContactID
		{
			get
			{
				if (this.Parameters.Contains("ContactID"))
				{
					return ((string)(this.Parameters["ContactID"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["ContactID"] = value;
			}
		}
		
		public Microsoft.Xrm.Sdk.EntityReference Target
		{
			get
			{
				if (this.Parameters.Contains("Target"))
				{
					return ((Microsoft.Xrm.Sdk.EntityReference)(this.Parameters["Target"]));
				}
				else
				{
					return default(Microsoft.Xrm.Sdk.EntityReference);
				}
			}
			set
			{
				this.Parameters["Target"] = value;
			}
		}
		
		public ecer_CLEANUPDeleteContactApplicationsActionRequest()
		{
			this.RequestName = "ecer_CLEANUPDeleteContactApplicationsAction";
			this.ContactID = default(string);
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/ecer/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("ecer_CLEANUPDeleteContactApplicationsAction")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class ecer_CLEANUPDeleteContactApplicationsActionResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public ecer_CLEANUPDeleteContactApplicationsActionResponse()
		{
		}
		
		public bool Completed
		{
			get
			{
				if (this.Results.Contains("Completed"))
				{
					return ((bool)(this.Results["Completed"]));
				}
				else
				{
					return default(bool);
				}
			}
		}
	}
}
#pragma warning restore CS1591
