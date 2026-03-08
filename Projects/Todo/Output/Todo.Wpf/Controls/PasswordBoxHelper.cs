using System.Windows;
using System.Windows.Controls;

namespace Todo.Wpf.Controls;

public static class PasswordBoxHelper
{
	public static readonly DependencyProperty BindPasswordProperty =
		DependencyProperty.RegisterAttached(
			"BindPassword",
			typeof(bool),
			typeof(PasswordBoxHelper),
			new PropertyMetadata(false, OnBindPasswordChanged));

	public static readonly DependencyProperty BoundPasswordProperty =
		DependencyProperty.RegisterAttached(
			"BoundPassword",
			typeof(string),
			typeof(PasswordBoxHelper),
			new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

	private static readonly DependencyProperty IsUpdatingProperty =
		DependencyProperty.RegisterAttached(
			"IsUpdating",
			typeof(bool),
			typeof(PasswordBoxHelper),
			new PropertyMetadata(false));

	public static bool GetBindPassword(DependencyObject dp) => (bool)dp.GetValue(BindPasswordProperty);
	public static void SetBindPassword(DependencyObject dp, bool value) => dp.SetValue(BindPasswordProperty, value);

	public static string GetBoundPassword(DependencyObject dp) => (string)dp.GetValue(BoundPasswordProperty);
	public static void SetBoundPassword(DependencyObject dp, string value) => dp.SetValue(BoundPasswordProperty, value);

	private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);
	private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);

	private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		if (dp is not PasswordBox passwordBox)
		{
			return;
		}

		if ((bool)e.OldValue)
		{
			passwordBox.PasswordChanged -= HandlePasswordChanged;
		}

		if ((bool)e.NewValue)
		{
			passwordBox.PasswordChanged += HandlePasswordChanged;
		}
	}

	private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
	{
		if (dp is not PasswordBox passwordBox)
		{
			return;
		}

		if (!GetBindPassword(passwordBox))
		{
			return;
		}

		if (GetIsUpdating(passwordBox))
		{
			return;
		}

		passwordBox.Password = e.NewValue as string ?? string.Empty;
	}

	private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
	{
		var passwordBox = (PasswordBox)sender;

		SetIsUpdating(passwordBox, true);
		SetBoundPassword(passwordBox, passwordBox.Password);
		SetIsUpdating(passwordBox, false);
	}
}
