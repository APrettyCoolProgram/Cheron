using System.Windows;

/// <summary>
/// Specifies theme information for the Ubiquitus WPF application.
/// Defines where WPF should look for theme-specific and generic resource dictionaries.
/// </summary>
/// <remarks>
/// <para>
/// The first parameter (ResourceDictionaryLocation.None) indicates that theme-specific 
/// resource dictionaries are not used in this application. WPF will not search for 
/// theme-specific resources in separate assemblies.
/// </para>
/// <para>
/// The second parameter (ResourceDictionaryLocation.SourceAssembly) specifies that 
/// the generic resource dictionary is located in the same assembly as the application. 
/// This is the fallback location when a resource is not found in page or application 
/// resource dictionaries.
/// </para>
/// </remarks>
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
