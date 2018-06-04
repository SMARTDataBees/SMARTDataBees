using Carbon.Configuration;
using Carbon.Configuration.Providers.Custom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace SDBees.Core.Global
{
	public static class SDBeesGlobalVars
	{
		public const string SDBeesRoleIdMEP = "MEPmanual"; // only for manual qualification in MEP used
		public const string SDBeesRoleIdAEC = "AECmanual"; // only for manual qualification in AEC used

		#region AllUsers and LocalUsers Configuration
		private const string m_SDBees = "Global";
		public static XmlConfigurationCategory SDBeesLocalUsersConfiguration()
		{
			XmlConfigurationCategory cat = LocalUsersConfigurationProvider.Current.Configuration.Categories[m_SDBees, true];

			return cat;
		}

		public static XmlConfigurationCategory SDBeesAllUsersConfiguration()
		{
			XmlConfigurationCategory cat = AllUsersConfigurationProvider.Current.Configuration.Categories[m_SDBees, true];

			return cat;
		}

		internal static void SetupInternalConfigurations()
		{
			//SetUpDisplaymentConfiguration();
			SetupUIUnitConfigurations();
		}

		private const string m_UIUnitsLength = "LengthUnits";
		private const string m_UIUnitsArea = "AreaUnits";
		private const string m_UIDecimalPlaces = "Decimal rounding";
		private const string m_UIUnitsCategory = "UI Units";
		private const string m_UIRegion = "Region";
		private const string m_UIRegionCategory = "UI Region";
		private static void SetupUIUnitConfigurations()
		{
			try
			{
				// Add unit configuration
				XmlConfigurationCategory catSDBees = SDBeesLocalUsersConfiguration();

				AddLengthUnitsToConfiguration(catSDBees);

				AddAreaUnitsToConfiguration(catSDBees);

				AddDecimalPlacesToConfiguration(catSDBees);

				AddRegionConfiguration(catSDBees);
			}
			catch (Exception ex)
			{
			}
		}


		private static void AddRegionConfiguration(XmlConfigurationCategory catSDBees)
		{
			XmlConfigurationOption opRegion = new XmlConfigurationOption();
			opRegion.DisplayName = m_UIRegion;
			opRegion.ElementName = m_UIRegion;
			opRegion.Category = m_UIRegionCategory;
			opRegion.ValueAssemblyQualifiedName = typeof(string).AssemblyQualifiedName;
			opRegion.TypeConverterAssemblyQualifiedName = typeof(GuiTools.TypeConverters.CountryInfoTypeConverter).AssemblyQualifiedName;
			opRegion.Value = GuiTools.TypeConverters.CountryInfoTypeConverter.GetDefaultCountryDisplayName("en-US");
			opRegion.Description = "Region settings for UI";
			//opAnullarSpace.EditorAssemblyQualifiedName
			if (!catSDBees.Options.Contains(opRegion))
				catSDBees.Options.Add(opRegion);
		}

		private static void AddDecimalPlacesToConfiguration(XmlConfigurationCategory catSDBees)
		{
			XmlConfigurationOption opPrecision = new XmlConfigurationOption();
			opPrecision.DisplayName = m_UIDecimalPlaces;
			opPrecision.ElementName = m_UIDecimalPlaces;
			opPrecision.Category = m_UIUnitsCategory;
			opPrecision.ValueAssemblyQualifiedName = typeof(Int16).AssemblyQualifiedName;
			opPrecision.TypeConverterAssemblyQualifiedName = typeof(GuiTools.TypeConverters.DecimalPlacesTypeConverter).AssemblyQualifiedName;
			opPrecision.Value = 3;
			opPrecision.Description = "Decimal places for rounding in ui";
			//opAnullarSpace.EditorAssemblyQualifiedName
			if (!catSDBees.Options.Contains(opPrecision))
				catSDBees.Options.Add(opPrecision);
		}

		private static void AddAreaUnitsToConfiguration(XmlConfigurationCategory catSDBees)
		{
			XmlConfigurationOption opLengthArea = new XmlConfigurationOption();
			opLengthArea.DisplayName = m_UIUnitsArea;
			opLengthArea.ElementName = m_UIUnitsArea;
			opLengthArea.Category = m_UIUnitsCategory;
			opLengthArea.ValueAssemblyQualifiedName = typeof(SDBees.Core.Model.Math.AreaUnits).AssemblyQualifiedName;
			opLengthArea.Value = DefaultAreaUnits;
			opLengthArea.Description = "Configuration for area units in ui";
			//opAnullarSpace.EditorAssemblyQualifiedName
			if (!catSDBees.Options.Contains(opLengthArea))
				catSDBees.Options.Add(opLengthArea);
		}

		private static void AddLengthUnitsToConfiguration(XmlConfigurationCategory catSDBees)
		{
			XmlConfigurationOption opLengthUnits = new XmlConfigurationOption();
			opLengthUnits.DisplayName = m_UIUnitsLength;
			opLengthUnits.ElementName = m_UIUnitsLength;
			opLengthUnits.Category = m_UIUnitsCategory;
			opLengthUnits.ValueAssemblyQualifiedName = typeof(SDBees.Core.Model.Math.LengthUnits).AssemblyQualifiedName;
			opLengthUnits.Value = DefaultLengthUnits;
			opLengthUnits.Description = "Configuration for length units in ui";
			//opAnullarSpace.EditorAssemblyQualifiedName
			if (!catSDBees.Options.Contains(opLengthUnits))
				catSDBees.Options.Add(opLengthUnits);
		}
		public static string GetRegionDisplayName()
		{
			return SDBeesLocalUsersConfiguration().Options[m_UIRegion, true].Value.ToString();
		}

		public static string GetRegionCode()
		{
			return GuiTools.TypeConverters.CountryInfoTypeConverter.GetIsoCodeForDisplayname(GetRegionDisplayName());
		}

		public static CultureInfo GetCultureInfo()
		{
			string regionCode = GetRegionCode();
			CultureInfo cInf = CultureInfo.GetCultureInfo(regionCode);

			return cInf;
		}

		public static SDBees.Core.Model.Math.LengthUnits GetLengthUnits()
		{
			SDBees.Core.Model.Math.LengthUnits lu = DefaultLengthUnits;

			if (!Enum.TryParse<SDBees.Core.Model.Math.LengthUnits>(SDBeesLocalUsersConfiguration().Options[m_UIUnitsLength, true].Value.ToString(), out lu))
			{
				System.Windows.Forms.MessageBox.Show("Error while retriving LengthUnits!\nPlease check Configuration!");
			}
			return lu;
		}

		public static SDBees.Core.Model.Math.AreaUnits GetAreaUnits()
		{
			SDBees.Core.Model.Math.AreaUnits au = DefaultAreaUnits;

			if (!Enum.TryParse<SDBees.Core.Model.Math.AreaUnits>(SDBeesLocalUsersConfiguration().Options[m_UIUnitsArea, true].Value.ToString(), out au))
			{
				System.Windows.Forms.MessageBox.Show("Error while retriving AreaUnits!\nPlease check Configuration!");
			}
			return au;
		}

		public static Int16 GetDecimalPlaces()
		{
			Int16 au = 2;

			if (!Int16.TryParse(SDBeesLocalUsersConfiguration().Options[m_UIDecimalPlaces, true].Value.ToString(), out au))
			{
				System.Windows.Forms.MessageBox.Show("Error while retriving precision!\nPlease check Configuration!");
			}
			return au;
		}

		public static string GetFormattingForDecimalPlaces()
		{
			string result = "0.";
			int dp = GetDecimalPlaces();

			for (int i = 0; i < dp; i++)
			{
				result += "0";
			}
			return result;
		}

		public static CultureInfo GetCurrentCulture()
		{
			return Thread.CurrentThread.CurrentCulture;
		}

		public static SDBees.Core.Model.Math.LengthUnits DefaultLengthUnits
		{
			get
			{
				return SDBees.Core.Model.Math.LengthUnits.Inches;
			}
		}

		public static SDBees.Core.Model.Math.AreaUnits DefaultAreaUnits
		{
			get
			{
				return SDBees.Core.Model.Math.AreaUnits.Square_Inches;
			}
		}

		const string m_DisplaymentCategory = "Displayment";
		private static void SetUpDisplaymentConfiguration()
		{
			XmlConfigurationCategory catSDBees = SDBeesLocalUsersConfiguration();

			//AddViewAdminDisplayment(catSDBees);
		}

		//const string m_ViewAdmin = "ViewAdmin";
		//private static void AddViewAdminDisplayment(XmlConfigurationCategory catSDBees)
		//{
		//    XmlConfigurationOption opViewAdminDisplay = new XmlConfigurationOption();
		//    opViewAdminDisplay.DisplayName = m_ViewAdmin;
		//    opViewAdminDisplay.ElementName = m_ViewAdmin;
		//    opViewAdminDisplay.Category = m_DisplaymentCategory;
		//    opViewAdminDisplay.ValueAssemblyQualifiedName = typeof(bool).AssemblyQualifiedName;
		//    opViewAdminDisplay.Value = false;
		//    opViewAdminDisplay.Description = "Show ViewAdmin in frontend?";
		//    if (!catSDBees.Options.Contains(opViewAdminDisplay))
		//        catSDBees.Options.Add(opViewAdminDisplay);
		//}        

		#endregion

		#region .config section

		private const string m_DisallowEditSchemaList = "DisallowEditSchema";
		public static List<string> GetEditSchemaDissallowList()
		{
			List<string> _dissallow = new List<string>();
			if (SDBees.Core.Global.GlobalManager.Current != null)
			{
				try
				{
					_dissallow.AddRange(SDBees.Core.Global.GlobalManager.Current.Config.AppSettings.Settings[m_DisallowEditSchemaList].Value.ToString().Split(';'));
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}
			return _dissallow;
		}

		private const string m_ViewAdminDisplayment = "ViewAdminVisible";
		public static bool GetViewAdminDisplayment()
		{
			bool _showViewAdmin = false;

			// Replaced to .config file to handle seperate settings for SDBees and derived Apps
			if(SDBees.Core.Global.GlobalManager.Current != null)
			{
				try
				{
					if(!Boolean.TryParse(SDBees.Core.Global.GlobalManager.Current.Config.AppSettings.Settings[m_ViewAdminDisplayment].Value.ToString(), out _showViewAdmin))
						System.Windows.Forms.MessageBox.Show("Error while retriving displament setting for ViewAdmin!\nPlease check Configuration!");
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}
			return _showViewAdmin;
		}

		private const string m_ReportingManagerDisplayment = "ReportingManagerVisible";
		public static bool GetReportingManagerDisplayment()
		{
			bool _showReportingManager = false;
			if (SDBees.Core.Global.GlobalManager.Current != null)
			{
				try
				{
					if (!Boolean.TryParse(SDBees.Core.Global.GlobalManager.Current.Config.AppSettings.Settings[m_ReportingManagerDisplayment].Value.ToString(), out _showReportingManager))
						System.Windows.Forms.MessageBox.Show("Error while retriving displament setting for ReportingManager!\nPlease check Configuration!");
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}

			return _showReportingManager;
		}

		private const string m_EDMManagerDisplayment = "EDMManagerVisible";
		public static bool GetEDMManagerDisplayment()
		{
			bool _showEDMManager = false;
			if (SDBees.Core.Global.GlobalManager.Current != null)
			{
				try
				{
					if (!Boolean.TryParse(SDBees.Core.Global.GlobalManager.Current.Config.AppSettings.Settings[m_EDMManagerDisplayment].Value.ToString(), out _showEDMManager))
						System.Windows.Forms.MessageBox.Show("Error while retriving displament setting for EDMManager!\nPlease check Configuration!");
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}

			return _showEDMManager;
		}

		private const string m_LoginDlgPropertiesEnabled = "LoginDlgPropertiesEnabled";
		public static bool GetLoginDlgPropertiesEnabled()
		{
			bool _enableLoginDlgProps = false;
			if (SDBees.Core.Global.GlobalManager.Current != null)
			{
				try
				{
					if (!Boolean.TryParse(SDBees.Core.Global.GlobalManager.Current.Config.AppSettings.Settings[m_LoginDlgPropertiesEnabled].Value.ToString(), out _enableLoginDlgProps))
						System.Windows.Forms.MessageBox.Show("Error while retriving enable setting for LoginDlg!\nPlease check Configuration!");
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}

			return _enableLoginDlgProps;
		}
		#endregion

		#region progress dlg
		static Carbon.UI.ProgressWindow m_ProgressDlg;

		/// <summary>
		/// Progressdialog in SDBees style
		/// </summary>
		/// <returns></returns>
		public static void ProgressWindowShow(System.Windows.Forms.IWin32Window owner, string title, string heading, string description, string extendedDescription)
		{
			if (m_ProgressDlg == null)
				m_ProgressDlg = new Carbon.UI.ProgressWindow();

			m_ProgressDlg.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			m_ProgressDlg.Show(owner);
			m_ProgressDlg.SetTitle(title);
			m_ProgressDlg.SetHeading(heading);
			m_ProgressDlg.SetDescription(description);
			m_ProgressDlg.SetExtendedDescription(extendedDescription);

			m_ProgressDlg.SetMarqueeMoving(true, true);
			m_ProgressDlg.UseWaitCursor = true;
			m_ProgressDlg.Refresh();
		}

		//public static void ProgressWindowStep()
		//{
		//    if (m_ProgressDlg != null)
		//        m_ProgressDlg.
		//}

		public static void ProgressWindowHide()
		{
			m_ProgressDlg.Hide();
		}
		#endregion
	}
}
