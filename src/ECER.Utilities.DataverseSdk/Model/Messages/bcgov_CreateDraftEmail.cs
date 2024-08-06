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
	
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/bcgov/")]
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("bcgov_CreateDraftEmail")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_CreateDraftEmailRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
		public string Subject
		{
			get
			{
				if (this.Parameters.Contains("Subject"))
				{
					return ((string)(this.Parameters["Subject"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["Subject"] = value;
			}
		}
		
		public string RegardingObjectTypeName
		{
			get
			{
				if (this.Parameters.Contains("RegardingObjectTypeName"))
				{
					return ((string)(this.Parameters["RegardingObjectTypeName"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["RegardingObjectTypeName"] = value;
			}
		}
		
		public string RegardingObjectId
		{
			get
			{
				if (this.Parameters.Contains("RegardingObjectId"))
				{
					return ((string)(this.Parameters["RegardingObjectId"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["RegardingObjectId"] = value;
			}
		}
		
		public string DocumentIds
		{
			get
			{
				if (this.Parameters.Contains("DocumentIds"))
				{
					return ((string)(this.Parameters["DocumentIds"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["DocumentIds"] = value;
			}
		}
		
		public bcgov_CreateDraftEmailRequest()
		{
			this.RequestName = "bcgov_CreateDraftEmail";
			this.Subject = default(string);
			this.RegardingObjectTypeName = default(string);
			this.RegardingObjectId = default(string);
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/bcgov/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("bcgov_CreateDraftEmail")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_CreateDraftEmailResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public bcgov_CreateDraftEmailResponse()
		{
		}
		
		public string EmailId
		{
			get
			{
				if (this.Results.Contains("EmailId"))
				{
					return ((string)(this.Results["EmailId"]));
				}
				else
				{
					return default(string);
				}
			}
		}
		
		public string UserMessage
		{
			get
			{
				if (this.Results.Contains("UserMessage"))
				{
					return ((string)(this.Results["UserMessage"]));
				}
				else
				{
					return default(string);
				}
			}
		}
	}
}
#pragma warning restore CS1591
