using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Todo.Wpf.Controls;

public class HighlightTextBlock : TextBlock
{
	public static readonly DependencyProperty HighlightTextProperty = DependencyProperty.Register(
		nameof(HighlightText),
		typeof(string),
		typeof(HighlightTextBlock),
		new PropertyMetadata(string.Empty, OnTextOrHighlightChanged));

	static HighlightTextBlock()
	{
		TextProperty.OverrideMetadata(typeof(HighlightTextBlock), new FrameworkPropertyMetadata(string.Empty, OnTextOrHighlightChanged));
	}

	public string HighlightText
	{
		get => (string)GetValue(HighlightTextProperty);
		set => SetValue(HighlightTextProperty, value);
	}

	private static void OnTextOrHighlightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		if (dependencyObject is HighlightTextBlock textBlock)
		{
			textBlock.UpdateInlines();
		}
	}

	private void UpdateInlines()
	{
		Inlines.Clear();

		var text = Text ?? string.Empty;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(HighlightText))
		{
			Inlines.Add(new Run(text));
			return;
		}

		var remaining = text;
		while (remaining.Length > 0)
		{
			var index = remaining.IndexOf(HighlightText, StringComparison.OrdinalIgnoreCase);
			if (index < 0)
			{
				Inlines.Add(new Run(remaining));
				break;
			}

			if (index > 0)
			{
				Inlines.Add(new Run(remaining[..index]));
			}

			Inlines.Add(new Run(remaining.Substring(index, HighlightText.Length))
			{
				Background = Brushes.Gold,
				Foreground = Brushes.Black,
			});

			remaining = remaining[(index + HighlightText.Length)..];
		}
	}
}
