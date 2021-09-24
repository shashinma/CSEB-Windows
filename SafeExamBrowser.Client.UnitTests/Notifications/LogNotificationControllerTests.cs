﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.IO.Packaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SafeExamBrowser.Client.Notifications;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Windows;

namespace SafeExamBrowser.Client.UnitTests.Notifications
{
	[TestClass]
	public class LogNotificationControllerTests
	{
		private Mock<ILogger> logger;
		private Mock<IText> text;
		private Mock<IUserInterfaceFactory> uiFactory;

		[TestInitialize]
		public void Initialize()
		{
			logger = new Mock<ILogger>();
			text = new Mock<IText>();
			uiFactory = new Mock<IUserInterfaceFactory>();

			// Ensure that the pack scheme is known before executing the unit tests, see https://stackoverflow.com/a/6005606
			_ = PackUriHelper.UriSchemePack;
		}

		[TestMethod]
		public void MustCloseWindowWhenTerminating()
		{
			var window = new Mock<IWindow>();
			var sut = new LogNotification(logger.Object, text.Object, uiFactory.Object);

			uiFactory.Setup(u => u.CreateLogWindow(It.IsAny<ILogger>())).Returns(window.Object);

			sut.Activate();
			sut.Terminate();

			window.Verify(w => w.Close());
		}

		[TestMethod]
		public void MustOpenOnlyOneWindow()
		{
			var window = new Mock<IWindow>();
			var sut = new LogNotification(logger.Object, text.Object, uiFactory.Object);

			uiFactory.Setup(u => u.CreateLogWindow(It.IsAny<ILogger>())).Returns(window.Object);

			sut.Activate();
			sut.Activate();
			sut.Activate();
			sut.Activate();
			sut.Activate();

			uiFactory.Verify(u => u.CreateLogWindow(It.IsAny<ILogger>()), Times.Once);
			window.Verify(u => u.Show(), Times.Once);
			window.Verify(u => u.BringToForeground(), Times.Exactly(4));
		}

		[TestMethod]
		public void MustNotFailToTerminateIfNotStarted()
		{
			var sut = new LogNotification(logger.Object, text.Object, uiFactory.Object);

			sut.Terminate();
		}
	}
}
