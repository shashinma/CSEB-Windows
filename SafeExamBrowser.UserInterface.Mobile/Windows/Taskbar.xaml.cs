﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.ComponentModel;
using System.Windows;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.UserInterface.Contracts.Shell.Events;
using SafeExamBrowser.UserInterface.Shared.Utilities;

namespace SafeExamBrowser.UserInterface.Mobile.Windows
{
	internal partial class Taskbar : Window, ITaskbar
	{
		private bool allowClose;
		private ILogger logger;

		public bool ShowClock
		{
			set { Dispatcher.Invoke(() => Clock.Visibility = value ? Visibility.Visible : Visibility.Collapsed); }
		}

		public bool ShowQuitButton
		{
			set { Dispatcher.Invoke(() => QuitButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed); }
		}

		public event QuitButtonClickedEventHandler QuitButtonClicked;

		internal Taskbar(ILogger logger)
		{
			this.logger = logger;

			InitializeComponent();
			InitializeTaskbar();
		}

		public void AddApplicationControl(IApplicationControl control, bool atFirstPosition = false)
		{
			if (control is UIElement uiElement)
			{
				if (atFirstPosition)
				{
					ApplicationStackPanel.Children.Insert(0, uiElement);
				}
				else
				{
					ApplicationStackPanel.Children.Add(uiElement);
				}
			}
		}

		public void AddNotificationControl(INotificationControl control)
		{
			if (control is UIElement uiElement)
			{
				NotificationStackPanel.Children.Add(uiElement);
			}
		}

		public void AddSystemControl(ISystemControl control)
		{
			if (control is UIElement uiElement)
			{
				SystemControlStackPanel.Children.Add(uiElement);
			}
		}

		public new void Close()
		{
			Dispatcher.Invoke(base.Close);
		}

		public int GetAbsoluteHeight()
		{
			return Dispatcher.Invoke(() =>
			{
				var height = (int) this.TransformToPhysical(Width, Height).Y;

				logger.Debug($"Calculated physical taskbar height is {height}px.");

				return height;
			});
		}

		public int GetRelativeHeight()
		{
			return Dispatcher.Invoke(() =>
			{
				var height = (int) Height;

				logger.Debug($"Logical taskbar height is {height}px.");

				return height;
			});
		}

		public void InitializeBounds()
		{
			Dispatcher.Invoke(() =>
			{
				Width = SystemParameters.PrimaryScreenWidth;
				Left = 0;
				Top = SystemParameters.PrimaryScreenHeight - Height;

				var position = this.TransformToPhysical(Left, Top);
				var size = this.TransformToPhysical(Width, Height);

				logger.Debug($"Set taskbar bounds to {Width}x{Height} at ({Left}/{Top}), in physical pixels: {size.X}x{size.Y} at ({position.X}/{position.Y}).");
			});
		}

		public void InitializeText(IText text)
		{
			Dispatcher.Invoke(() =>
			{
				QuitButton.ToolTip = text.Get(TextKey.Shell_QuitButton);
			});
		}

		public new void Show()
		{
			Dispatcher.Invoke(base.Show);
		}

		private void QuitButton_Clicked(CancelEventArgs args)
		{
			QuitButtonClicked?.Invoke(args);
			allowClose = !args.Cancel;
		}

		private void Taskbar_Closing(object sender, CancelEventArgs e)
		{
			if (allowClose)
			{
				foreach (var child in SystemControlStackPanel.Children)
				{
					if (child is ISystemControl systemControl)
					{
						systemControl.Close();
					}
				}
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void InitializeTaskbar()
		{
			Closing += Taskbar_Closing;
			Loaded += (o, args) => InitializeBounds();
			QuitButton.Clicked += QuitButton_Clicked;
		}
	}
}
