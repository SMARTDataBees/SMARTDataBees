using Carbon.Plugins.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die einer Assembly zugeordnet sind.
[assembly: AssemblyTitle("SDBees.Plugins")]
[assembly: AssemblyDescription("SMARTDataBees plugin library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SMARTDataBees")]
[assembly: AssemblyProduct("SDBees.Plugins")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf FALSE werden die Typen in dieser Assembly
// für COM-Komponenten unsichtbar.  Wenn Sie auf einen Typ in dieser Assembly von
// COM aus zugreifen müssen, sollten Sie das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("7f6dc800-2d96-49a0-a8c8-8502314c49af")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder Standardwerte für die Build- und Revisionsnummern verwenden,
// indem Sie "*" wie unten gezeigt eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

//Plugins AEC
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Building.AECBuilding))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Curtainwall.AECCurtainwall))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Curtainwall.AECCurtainwallArea))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Door.AECDoor))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Room.AECRoom))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Level.AECLevel))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Level.AECSubLevel))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Wall.AECWall))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Window.AECWindow))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.AEC.Zone.AECZone))]

//MEP
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.MEP.BuildingStatistics.MEPBuildingsStats))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.MEP.ContractData.MEPContractData))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.MEP.Element.MEPElement))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.MEP.System.MEPSystem))]
[assembly: PluginDefinition(typeof(SDBees.Core.Plugins.MEP.CutOut.MEPCutOut))]

[assembly: PluginDefinition(typeof(SDBees.Plugins.TreeviewHelper.ListViewHelper))]




