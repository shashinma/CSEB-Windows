﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;

namespace SafeExamBrowser.Communication.Contracts.Data
{
	/// <summary>
	/// The base class for respones, from which a response must inherit in order to be sent to an interlocutor as reply to <see cref="ICommunication.Send(Message)"/>.
	/// </summary>
	[Serializable]
	public abstract class Response
	{
		public override string ToString()
		{
			return GetType().Name;
		}
	}
}
