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
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("bcgov_DownloadDocumentFromSharePoint")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_DownloadDocumentFromSharePointRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
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
		
		public bcgov_DownloadDocumentFromSharePointRequest()
		{
			this.RequestName = "bcgov_DownloadDocumentFromSharePoint";
			this.Target = default(Microsoft.Xrm.Sdk.EntityReference);
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/bcgov/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("bcgov_DownloadDocumentFromSharePoint")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class bcgov_DownloadDocumentFromSharePointResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public bcgov_DownloadDocumentFromSharePointResponse()
		{
		}
		
		public string Body
		{
			get
			{
				if (this.Results.Contains("Body"))
				{
					return ((string)(this.Results["Body"]));
				}
				else
				{
					return default(string);
				}
			}
		}
		
		public string FileName
		{
			get
			{
				if (this.Results.Contains("FileName"))
				{
					return ((string)(this.Results["FileName"]));
				}
				else
				{
					return default(string);
				}
			}
		}
		
		public string FileSize
		{
			get
			{
				if (this.Results.Contains("FileSize"))
				{
					return ((string)(this.Results["FileSize"]));
				}
				else
				{
					return default(string);
				}
			}
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
		
		public string MimeType
		{
			get
			{
				if (this.Results.Contains("MimeType"))
				{
					return ((string)(this.Results["MimeType"]));
				}
				else
				{
					return default(string);
				}
			}
		}
		
		public System.DateTime ReceivedDate
		{
			get
			{
				if (this.Results.Contains("ReceivedDate"))
				{
					return ((System.DateTime)(this.Results["ReceivedDate"]));
				}
				else
				{
					return default(System.DateTime);
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
