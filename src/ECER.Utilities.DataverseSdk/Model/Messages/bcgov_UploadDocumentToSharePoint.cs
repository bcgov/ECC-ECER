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
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("bcgov_UploadDocumentToSharePoint")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_UploadDocumentToSharePointRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
		public string DocumentId
		{
			get
			{
				if (this.Parameters.Contains("DocumentId"))
				{
					return ((string)(this.Parameters["DocumentId"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["DocumentId"] = value;
			}
		}
		
		public string FileName
		{
			get
			{
				if (this.Parameters.Contains("FileName"))
				{
					return ((string)(this.Parameters["FileName"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["FileName"] = value;
			}
		}
		
		public string Body
		{
			get
			{
				if (this.Parameters.Contains("Body"))
				{
					return ((string)(this.Parameters["Body"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["Body"] = value;
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
		
		public string RegardingObjectName
		{
			get
			{
				if (this.Parameters.Contains("RegardingObjectName"))
				{
					return ((string)(this.Parameters["RegardingObjectName"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["RegardingObjectName"] = value;
			}
		}
		
		public string Tag1
		{
			get
			{
				if (this.Parameters.Contains("Tag1"))
				{
					return ((string)(this.Parameters["Tag1"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["Tag1"] = value;
			}
		}
		
		public string Tag2
		{
			get
			{
				if (this.Parameters.Contains("Tag2"))
				{
					return ((string)(this.Parameters["Tag2"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["Tag2"] = value;
			}
		}
		
		public string Tag3
		{
			get
			{
				if (this.Parameters.Contains("Tag3"))
				{
					return ((string)(this.Parameters["Tag3"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Parameters["Tag3"] = value;
			}
		}
		
		public Microsoft.Xrm.Sdk.OptionSetValue Origin
		{
			get
			{
				if (this.Parameters.Contains("Origin"))
				{
					return ((Microsoft.Xrm.Sdk.OptionSetValue)(this.Parameters["Origin"]));
				}
				else
				{
					return default(Microsoft.Xrm.Sdk.OptionSetValue);
				}
			}
			set
			{
				this.Parameters["Origin"] = value;
			}
		}
		
		public bcgov_UploadDocumentToSharePointRequest()
		{
			this.RequestName = "bcgov_UploadDocumentToSharePoint";
			this.FileName = default(string);
			this.Body = default(string);
			this.RegardingObjectId = default(string);
			this.RegardingObjectName = default(string);
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/bcgov/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("bcgov_UploadDocumentToSharePoint")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_UploadDocumentToSharePointResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public bcgov_UploadDocumentToSharePointResponse()
		{
		}
		
		public bool IsSuccess
		{
			get
			{
				if (this.Results.Contains("IsSuccess"))
				{
					return ((bool)(this.Results["IsSuccess"]));
				}
				else
				{
					return default(bool);
				}
			}
		}
		
		public string Result
		{
			get
			{
				if (this.Results.Contains("Result"))
				{
					return ((string)(this.Results["Result"]));
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
