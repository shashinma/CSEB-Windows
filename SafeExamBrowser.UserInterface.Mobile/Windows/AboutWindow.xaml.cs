﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using SafeExamBrowser.Configuration.Contracts;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;
using SafeExamBrowser.UserInterface.Contracts.Windows.Events;

namespace SafeExamBrowser.UserInterface.Mobile.Windows
{
	internal partial class AboutWindow : Window, IWindow
	{
		private AppConfig appConfig;
		private IText text;
		private WindowClosingEventHandler closing;

		event WindowClosingEventHandler IWindow.Closing
		{
			add { closing += value; }
			remove { closing -= value; }
		}

		internal AboutWindow(AppConfig appConfig, IText text)
		{
			this.appConfig = appConfig;
			this.text = text;

			InitializeComponent();
			InitializeAboutWindow();
		}

		public void BringToForeground()
		{
			Activate();
		}

		private void InitializeAboutWindow()
		{
			Closing += (o, args) => closing?.Invoke();
			MainText.Inlines.InsertBefore(MainText.Inlines.FirstInline, new Run(text.Get(TextKey.AboutWindow_LicenseInfo)));
			Title = text.Get(TextKey.AboutWindow_Title);

			VersionInfo.Inlines.Add(new Run($"{text.Get(TextKey.Version)} {appConfig.ProgramInformationalVersion}"));
			VersionInfo.Inlines.Add(new LineBreak());
			VersionInfo.Inlines.Add(new Run($"{text.Get(TextKey.Build)} {appConfig.ProgramBuildVersion}") { FontSize = 10, Foreground = Brushes.Gray });
			VersionInfo.Inlines.Add(new LineBreak());
			VersionInfo.Inlines.Add(new LineBreak());
			VersionInfo.Inlines.Add(new Run(appConfig.ProgramCopyright) { FontSize = 12, Foreground = Brushes.Gray });
		}
	}
}
