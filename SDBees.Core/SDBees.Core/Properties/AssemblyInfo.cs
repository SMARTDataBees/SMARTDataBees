using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Carbon.Plugins.Attributes;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("SDBees.Core")]
[assembly: AssemblyDescription("SMARTDataBees core library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SMARTDataBees")]
[assembly: AssemblyProduct("SDBees.Core")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("37750fa8-e3d3-4ddd-a5ad-f6ae00424f51")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyFileVersion("2.0.0.*")]
[assembly: AssemblyVersion("2.0.0.*")]

//SDBees Core plugins
[assembly: PluginDefinition(typeof(SDBees.Core.Global.GlobalManager))]
[assembly: PluginDefinition(typeof(SDBees.DB.SDBeesDBConnection))]
[assembly: PluginDefinition(typeof(SDBees.Core.Connectivity.ConnectivityManager))]
[assembly: PluginDefinition(typeof(SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocumentAdmin))]
[assembly: PluginDefinition(typeof(SDBees.Core.Main.Systemtray.ProcessIcon))]
[assembly: PluginDefinition(typeof(SDBees.ViewAdmin.ViewAdmin))]

[assembly: PluginDefinition(typeof(SDBees.ViewAdmin.ViewAdminLinkHelper))]

[assembly: PluginDefinition(typeof(SDBees.Main.Window.MainWindowApplication))]

[assembly: PluginDefinition(typeof(SDBees.UserAdmin.UserAdmin))]
[assembly: PluginDefinition(typeof(SDBees.EDM.EDMManager))]
[assembly: PluginDefinition(typeof(SDBees.Reporting.ReportingManager))]

[assembly: PluginDefinition(typeof(SDBees.Demoplugins.Dummys.Dummy1))]
[assembly: PluginDefinition(typeof(SDBees.Demoplugins.Dummys.Dummy2))]

