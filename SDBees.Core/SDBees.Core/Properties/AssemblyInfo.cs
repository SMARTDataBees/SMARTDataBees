using System.Reflection;
using System.Runtime.InteropServices;
using Carbon.Plugins.Attributes;
using SDBees.Core.Connectivity;
using SDBees.Core.Connectivity.SDBeesLink;
using SDBees.Core.Global;
using SDBees.Core.Main.Systemtray;
using SDBees.DB;
using SDBees.Demoplugins.Dummys;
using SDBees.EDM;
using SDBees.Main.Window;
using SDBees.Reporting;
using SDBees.UserAdmin;
using SDBees.ViewAdmin;

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

[assembly: AssemblyFileVersion("1.0.900.*")]
[assembly: AssemblyVersion("1.0.900.*")]

//SDBees Core plugins
[assembly: PluginDefinition(typeof(GlobalManager))]
[assembly: PluginDefinition(typeof(SDBeesDBConnection))]
[assembly: PluginDefinition(typeof(ConnectivityManager))]
[assembly: PluginDefinition(typeof(SDBeesExternalDocumentAdmin))]
[assembly: PluginDefinition(typeof(ProcessIcon))]
[assembly: PluginDefinition(typeof(ViewAdmin))]

[assembly: PluginDefinition(typeof(ViewAdminLinkHelper))]

[assembly: PluginDefinition(typeof(MainWindowApplication))]

[assembly: PluginDefinition(typeof(UserAdmin))]
[assembly: PluginDefinition(typeof(EDMManager))]
[assembly: PluginDefinition(typeof(ReportingManager))]

[assembly: PluginDefinition(typeof(Dummy1))]
[assembly: PluginDefinition(typeof(Dummy2))]

