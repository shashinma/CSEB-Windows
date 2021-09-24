﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.WindowsApi.Contracts.Events
{
	/// <summary>
	/// The callback for a mouse hook. Return <c>true</c> to consume (i.e. block) the user input, otherwise <c>false</c>.
	/// </summary>
	public delegate bool MouseHookCallback(MouseButton button, MouseButtonState state, MouseInformation info);
}
