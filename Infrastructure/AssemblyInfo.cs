using System.Resources;
using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
[assembly: XmlnsDefinition("http://www.infrastructure.cnc.com/", "Infrastructure")]
[assembly: XmlnsDefinition("http://www.infrastructure.cnc.com/converters", "Infrastructure.Resources.Converters")]
[assembly: XmlnsDefinition("http://www.infrastructure.cnc.com/resources", "Infrastructure.Resources.Strings")]
[assembly: NeutralResourcesLanguage("en-US")]