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
	/// The mouse buttons which can be detected by a mouse hook.
	/// </summary>
	public enum MouseButton
	{
		Unknown = 0,
		Auxiliary,
		Left,
		Middle,
		Right
	}
}
