using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace <# Project.Namespace#>.Wpf;

public partial class App : Application
{
	static App()
	{
		var language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
		var metadata = new FrameworkPropertyMetadata(language);
		FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), metadata);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>