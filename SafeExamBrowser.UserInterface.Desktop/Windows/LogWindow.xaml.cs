﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Events;
using SafeExamBrowser.UserInterface.Desktop.ViewModels;

namespace SafeExamBrowser.UserInterface.Desktop.Windows
{
	internal partial class LogWindow : Window, IWindow
	{
		private ILogger logger;
		private LogViewModel model;
		private WindowClosingEventHandler closing;

		event WindowClosingEventHandler IWindow.Closing
		{
			add { closing += value; }
			remove { closing -= value; }
		}

		internal LogWindow(ILogger logger, IText text)
		{
			InitializeComponent();

			this.logger = logger;
			this.model = new LogViewModel(text, ScrollViewer, LogContent);

			InitializeLogWindow();
		}

		public void BringToForeground()
		{
			Dispatcher.Invoke(() =>
			{
				if (WindowState == WindowState.Minimized)
				{
					WindowState = WindowState.Normal;
				}

				Activate();
			});
		}

		public new void Close()
		{
			Dispatcher.Invoke(base.Close);
		}

		public new void Hide()
		{
			Dispatcher.Invoke(base.Hide);
		}

		public new void Show()
		{
			Dispatcher.Invoke(base.Show);
		}

		private void InitializeLogWindow()
		{
			DataContext = model;
			Closing += LogWindow_Closing;
			Loaded += LogWindow_Loaded;
		}

		private void LogWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Cursor = Cursors.Wait;

			var log = logger.GetLog();

			foreach (var content in log)
			{
				model.Notify(content);
			}

			logger.Subscribe(model);
			logger.Debug("Opened log window.");

			Cursor = Cursors.Arrow;
		}

		private void LogWindow_Closing(object sender, CancelEventArgs e)
		{
			logger.Unsubscribe(model);
			logger.Debug("Closed log window.");

			closing?.Invoke();
		}
	}
}
