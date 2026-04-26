using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using System;

namespace Todo.Avalonia.Android;

[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
	public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return base.CustomizeAppBuilder(builder)
			.WithInterFont();
	}
}
