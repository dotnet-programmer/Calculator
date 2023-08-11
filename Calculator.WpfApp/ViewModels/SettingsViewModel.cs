using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Calculator.WpfApp.Commands;
using Calculator.WpfApp.Models;
using Calculator.WpfApp.Properties;
using ControlzEx.Theming;

namespace Calculator.WpfApp.ViewModels;

internal class SettingsViewModel : BaseViewModel
{
	public SettingsViewModel()
	{
		SetCommands();
		SetThemes();
	}

	#region Property binding

	private ObservableCollection<ThemeScheme> _themes;
	public ObservableCollection<ThemeScheme> Themes
	{
		get => _themes;
		set { _themes = value; OnPropertyChanged(); }
	}

	private ThemeScheme? _selectedTheme;
	public ThemeScheme? SelectedTheme
	{
		get => _selectedTheme;
		set { _selectedTheme = value; OnPropertyChanged(); }
	}

	private bool _isWindowsThemeUse;
	public bool IsWindowsThemeUse
	{
		get => _isWindowsThemeUse;
		set { _isWindowsThemeUse = value; OnPropertyChanged(); }
	}

	#endregion Property binding

	public ICommand MoveUpSelectionCommand { get; private set; }
	public ICommand MoveDownSelectionCommand { get; private set; }
	public ICommand GridClickCommand { get; private set; }
	public ICommand ConfirmClickCommand { get; private set; }
	public ICommand WindowsThemeUseCommand { get; private set; }

	private void SetCommands()
	{
		MoveUpSelectionCommand = new RelayCommand(MoveUpSelection);
		MoveDownSelectionCommand = new RelayCommand(MoveDownSelection);
		GridClickCommand = new RelayCommand(GridClick);
		ConfirmClickCommand = new RelayCommand(ConfirmClick);
		WindowsThemeUseCommand = new RelayCommand(WindowsThemeUse);
	}

	private void MoveUpSelection(object commandParameter)
	{
		if (((DataGrid)commandParameter).SelectedIndex > 0)
		{
			((DataGrid)commandParameter).SelectedIndex--;
		}
	}

	private void MoveDownSelection(object commandParameter)
	{
		if (((DataGrid)commandParameter).SelectedIndex < Themes.Count)
		{
			((DataGrid)commandParameter).SelectedIndex++;
		}
	}

	private void GridClick(object commandParameter)
	{
		Window window = (Window)commandParameter;
		var grid = window.FindName("DgSettings") as DataGrid;

		if (grid.SelectedIndex > -1)
		{
			IsWindowsThemeUse = false;
			if (SelectedTheme != null)
			{
				ThemeManager.Current.ChangeTheme(window, $"{SelectedTheme.BaseScheme}.{SelectedTheme.ColorScheme}");
			}
		}
	}

	private void ConfirmClick(object commandParameter)
	{
		if (IsWindowsThemeUse && IsWindowsThemeUse != Settings.Default.IsWindowsThemeUse)
		{
			var dialogResult = MessageBox.Show($"Do you want to save the theme scheme?{Environment.NewLine}These changes require an application restart.", "Confirm save", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (dialogResult == MessageBoxResult.Yes)
			{
				Settings.Default.IsWindowsThemeUse = IsWindowsThemeUse;
				Settings.Default.Save();
				RestartApplication();
			}
		}
		else if (SelectedTheme != null)
		{
			var dialogResult = MessageBox.Show("Do you want to save theme scheme?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (dialogResult == MessageBoxResult.Yes)
			{
				ThemeManager.Current.ChangeTheme(Application.Current, $"{SelectedTheme.BaseScheme}.{SelectedTheme.ColorScheme}");
				Settings.Default.IsWindowsThemeUse = IsWindowsThemeUse;
				Settings.Default.BaseScheme = SelectedTheme.BaseScheme;
				Settings.Default.ColorScheme = SelectedTheme.ColorScheme;
				Settings.Default.Save();
			}
		}
		(commandParameter as Window)?.Close();
	}

	private void WindowsThemeUse(object commandParameter)
	{
		if (IsWindowsThemeUse)
		{
			((DataGrid)commandParameter).SelectedIndex = -1;
		}
		else
		{
			((DataGrid)commandParameter).SelectedIndex = 0;
		}
	}

	private void SetThemes()
	{
		IsWindowsThemeUse = Settings.Default.IsWindowsThemeUse;
		Themes = new(ThemeManager.Current.Themes.Select(x => new ThemeScheme { BaseScheme = x.BaseColorScheme, ColorScheme = x.ColorScheme }).OrderBy(x => x.BaseScheme));

		if (!IsWindowsThemeUse)
		{
			SelectedTheme = Themes.First(x => x.BaseScheme == Settings.Default.BaseScheme && x.ColorScheme == Settings.Default.ColorScheme);
		}
	}

	private void RestartApplication()
	{
		var appLocation = Application.ResourceAssembly.Location;
		Process.Start(appLocation.EndsWith(".dll") ? appLocation.Replace(".dll", ".exe") : appLocation);
		Application.Current.Shutdown();
	}
}