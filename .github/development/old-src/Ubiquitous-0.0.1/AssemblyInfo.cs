using System.Windows;

/// <summary>
/// Configures WPF theme resource dictionary locations
/// </summary>
/// <remarks>
/// This assembly attribute specifies where WPF should look for theme-specific and generic
/// resource dictionaries when resolving resources. The current configuration uses:
/// - ResourceDictionaryLocation.None for theme-specific resources (no separate theme dictionaries)
/// - ResourceDictionaryLocation.SourceAssembly for generic resources (resources are in this assembly)
/// </remarks>
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
