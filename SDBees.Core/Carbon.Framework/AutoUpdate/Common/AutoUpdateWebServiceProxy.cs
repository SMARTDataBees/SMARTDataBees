//	============================================================================
//
//  .,-:::::   :::.    :::::::..   :::::::.      ...   :::.    :::.
//	,;;;'````'   ;;`;;   ;;;;``;;;;   ;;;'';;'  .;;;;;;;.`;;;;,  `;;;
//	[[[         ,[[ '[[,  [[[,/[[['   [[[__[[\.,[[     \[[,[[[[[. '[[
//	$$$        c$$$cc$$$c $$$$$$c     $$""""Y$$$$$,     $$$$$$ "Y$c$$
//	`88bo,__,o, 888   888,888b "88bo,_88o,,od8P"888,_ _,88P888    Y88
//	"YUMMMMMP"YMM   ""` MMMM   "W" ""YUMMMP"   "YMMMMMP" MMM     YM
//
//	============================================================================
//
//	This file is a part of the Carbon Framework.
//
//	Copyright (C) 2005 Mark (Code6) Belles 
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//	============================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using Carbon.Common;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Defines a web service proxy that is responsible for querying the Carbon Framework AutoUpdateWebService
	/// </summary>
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name="AutoUpdateServiceSoap", Namespace="http://tempuri.org/")]
	public class AutoUpdateWebServiceProxy : SoapHttpClientProtocol
	{				
		/// <summary>
		/// Initializes a new instance of the AutoUpdateWebService class
		/// </summary>
		public AutoUpdateWebServiceProxy() 
		{
			Url = CarbonConfig.AutoUpdateWebServiceUrl;
		}
        
		/// <summary>
		/// Initializes a new instance of the AutoUpdateWebService class. Uses the default credentials of the application for authentication.
		/// </summary>
		/// <param name="url">The url of the Xml web service to connect to.</param>
		public AutoUpdateWebServiceProxy(string url) :
			this(url, CredentialCache.DefaultCredentials)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateWebService class.
		/// </summary>
		/// <param name="url">The url of the Xml web service to connect to.</param>
		/// <param name="credentials">The credentials to use to authenticate with the web service.</param>
		public AutoUpdateWebServiceProxy(string url, ICredentials credentials)
		{
			Url = url;
			PreAuthenticate = true;
			Credentials = credentials;
		}

		#region Version 1.0.0
//		
//		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryLatestVersion", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
//		public string QueryLatestVersion(string appName) 
//		{			
//			object[] results = this.Invoke("QueryLatestVersion", new object[] {
//																				  appName});
//			return ((string)(results[0]));
//		}
//        		
//		public System.IAsyncResult BeginQueryLatestVersion(string appName, System.AsyncCallback callback, object asyncState) 
//		{
//			return this.BeginInvoke("QueryLatestVersion", new object[] {
//																		   appName}, callback, asyncState);
//		}
//        		
//		public string EndQueryLatestVersion(System.IAsyncResult asyncResult) 
//		{
//			object[] results = this.EndInvoke(asyncResult);
//			return ((string)(results[0]));
//		}
//
		#endregion		

		#region Version 2.0.0

		/// <summary>
		/// Synchronously connects to the specified Xml web service and returns an Xml node containing an AutoUpdateManifest describing
		/// the latest version available from that web service.
		/// </summary>
		/// <param name="productName">The name of the product that is checking.</param>
		/// <param name="currentVersion">The version of the product that is checking.</param>
		/// <param name="productId">The id of the product that is checking.</param>
		/// <returns></returns>
		[SoapDocumentMethod("http://tempuri.org/QueryLatestVersion", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public XmlNode QueryLatestVersion(string productName, string currentVersion, string productId)
		{
			var results = Invoke("QueryLatestVersion", new object[] {
				productName,
				currentVersion,
				productId});
			return ((XmlNode)(results[0]));
		}
		
		public IAsyncResult BeginQueryLatestVersion(string productName, string currentVersion, string productId, AsyncCallback callback, object asyncState)
		{
			return BeginInvoke("QueryLatestVersion", new object[] {
				 productName,
				 currentVersion,
				 productId}, callback, asyncState);
		}
		
		public XmlNode EndQueryLatestVersion(IAsyncResult asyncResult)
		{
			var results = EndInvoke(asyncResult);
			return ((XmlNode)(results[0]));
		}

		#endregion

	}
}
