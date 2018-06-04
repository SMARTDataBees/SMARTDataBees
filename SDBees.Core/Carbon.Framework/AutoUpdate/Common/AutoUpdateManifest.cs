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

using Carbon.AutoUpdate.Common.Xml;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a class that provides information about an update.
	/// </summary>
	public sealed class AutoUpdateManifest
	{
		private string _id;
		private AutoUpdateProductDescriptor _product;
		private AutoUpdateHref _moreInfo;
		private AutoUpdateChangeSummaryList _changeSummaries;
		private string _urlOfUpdate;
		private long _sizeOfUpdate;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateManifest class.
		/// </summary>
		public AutoUpdateManifest()
		{
			_id = Guid.NewGuid().ToString();
			_product = new AutoUpdateProductDescriptor();
			_moreInfo = new AutoUpdateHref();
			_changeSummaries = new AutoUpdateChangeSummaryList();
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateManifest class.
		/// </summary>
		/// <param name="id">A unique identifier to assign to the manifest.</param>
		/// <param name="product">A descriptor that describes the product the update is for.</param>
		/// <param name="moreInfo">A link for displaying more information about the update.</param>
		/// <param name="changeSummaries">A collection of change summaries that can be found in the update.</param>
		/// <param name="urlOfUpdate">The url of the update.</param>
		/// <param name="sizeOfUpdate">The size of the update.</param>
		public AutoUpdateManifest(string id, AutoUpdateProductDescriptor product, AutoUpdateHref moreInfo, AutoUpdateChangeSummaryList changeSummaries, string urlOfUpdate, long sizeOfUpdate)
		{
			_id = id;
			_product = product;
			_moreInfo = moreInfo;
			_changeSummaries = changeSummaries;
			_urlOfUpdate = urlOfUpdate;
			_sizeOfUpdate = sizeOfUpdate;
		}

		#region My Public Properties

		/// <summary>
		/// Gets or sets the manifest id (This will be the update identifier used for registration key hashing)
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}	

		/// <summary>
		/// Gets or sets the product descriptor 
		/// </summary>
		public AutoUpdateProductDescriptor Product
		{
			get
			{
				return _product;
			}
			set
			{
				_product = value;
			}
		}

		/// <summary>
		/// Gets or sets the href for more information
		/// </summary>
		public AutoUpdateHref MoreInfo
		{
			get
			{
				return _moreInfo;
			}
			set
			{
				_moreInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the change summary list
		/// </summary>
		public AutoUpdateChangeSummaryList ChangeSummaries
		{
			get
			{
				return _changeSummaries;
			}
			set
			{
				_changeSummaries = value;
			}
		}

		/// <summary>
		/// Gets or sets the where the update can be downloaded from (ex: http://www.depcoinc.com/autoupdate/updates/assistant/1.0.0.0.update) (this Url can be a web link or a unc path)
		/// </summary>
		public string UrlOfUpdate
		{
			get
			{
				return _urlOfUpdate;
			}
			set
			{
				_urlOfUpdate = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of the update as it will be when downloaded
		/// </summary>
		public long SizeOfUpdate
		{
			get
			{
				return _sizeOfUpdate;
			}
			set
			{
				_sizeOfUpdate = value;
			}
		}

		#endregion		

		/// <summary>
		/// Returns the Xml representing this AutoUpdateManifest
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return XmlAutoUpdateManifestWriter.ToXml(this, System.Text.Encoding.UTF8);
		}
	}
}
