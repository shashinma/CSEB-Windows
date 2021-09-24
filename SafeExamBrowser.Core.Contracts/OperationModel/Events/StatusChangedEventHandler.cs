﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.I18n.Contracts;

namespace SafeExamBrowser.Core.Contracts.OperationModel.Events
{
	/// <summary>
	/// Event handler used to indicate that the status of an <see cref="IOperation"/> has changed.
	/// </summary>
	public delegate void StatusChangedEventHandler(TextKey status);
}
