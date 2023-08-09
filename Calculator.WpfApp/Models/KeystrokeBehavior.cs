using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace Calculator.WpfApp.Models;

internal class KeystrokeBehavior : Behavior<Window>
{
	public Key PressedKey { get; set; }

	protected override void OnAttached()
	{
		Window window = AssociatedObject;
		if (window != null)
		{
			window.KeyDown += Window_KeyDown;
		}
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == PressedKey)
		{
			(sender as Window)?.Close();
		}
	}
}