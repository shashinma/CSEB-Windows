﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using SafeExamBrowser.SystemComponents.Contracts;
using BatteryChargeStatus = System.Windows.Forms.BatteryChargeStatus;
using OperatingSystem = SafeExamBrowser.SystemComponents.Contracts.OperatingSystem;

namespace SafeExamBrowser.SystemComponents
{
	public class SystemInfo : ISystemInfo
	{
		public bool HasBattery { get; private set; }
		public string Manufacturer { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public OperatingSystem OperatingSystem { get; private set; }
		public string MacAddress { get; private set; }
		public string[] PlugAndPlayDeviceIds { get; private set; }

		public string OperatingSystemInfo
		{
			get { return $"{OperatingSystemName()}, {Environment.OSVersion.VersionString} ({Architecture()})"; }
		}

		public SystemInfo()
		{
			InitializeBattery();
			InitializeMachineInfo();
			InitializeOperatingSystem();
			InitializeMacAddress();
			InitializePnPDevices();
		}

		private void InitializeBattery()
		{
			var status = SystemInformation.PowerStatus.BatteryChargeStatus;

			HasBattery = !status.HasFlag(BatteryChargeStatus.NoSystemBattery) && !status.HasFlag(BatteryChargeStatus.Unknown);
		}

		private void InitializeMachineInfo()
		{
			var model = default(string);
			var systemFamily = default(string);

			try
			{
				using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
				using (var results = searcher.Get())
				using (var system = results.Cast<ManagementObject>().First())
				{
					foreach (var property in system.Properties)
					{
						if (property.Name.Equals("Manufacturer"))
						{
							Manufacturer = Convert.ToString(property.Value);
						}
						else if (property.Name.Equals("Model"))
						{
							model = Convert.ToString(property.Value);
						}
						else if (property.Name.Equals("Name"))
						{
							Name = Convert.ToString(property.Value);
						}
						else if (property.Name.Equals("SystemFamily"))
						{
							systemFamily = Convert.ToString(property.Value);
						}
					}
				}

				Model = string.Join(" ", systemFamily, model);
			}
			catch (Exception)
			{
				Manufacturer = "";
				Model = "";
				Name = "";
			}
		}

		private void InitializeOperatingSystem()
		{
			// IMPORTANT:
			// In order to be able to retrieve the correct operating system version via System.Environment.OSVersion, the executing
			// assembly needs to define an application manifest where the supported Windows versions are specified!
			var major = Environment.OSVersion.Version.Major;
			var minor = Environment.OSVersion.Version.Minor;

			// See https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions for mapping source...
			if (major == 6)
			{
				if (minor == 1)
				{
					OperatingSystem = OperatingSystem.Windows7;
				}
				else if (minor == 2)
				{
					OperatingSystem = OperatingSystem.Windows8;
				}
				else if (minor == 3)
				{
					OperatingSystem = OperatingSystem.Windows8_1;
				}
			}
			else if (major == 10)
			{
				OperatingSystem = OperatingSystem.Windows10;
			}
		}

		private string OperatingSystemName()
		{
			switch (OperatingSystem)
			{
				case OperatingSystem.Windows7:
					return "Windows 7";
				case OperatingSystem.Windows8:
					return "Windows 8";
				case OperatingSystem.Windows8_1:
					return "Windows 8.1";
				case OperatingSystem.Windows10:
					return "Windows 10";
				default:
					return "Unknown Windows Version";
			}
		}

		private string Architecture()
		{
			return Environment.Is64BitOperatingSystem ? "x64" : "x86";
		}

		private void InitializeMacAddress()
		{
			using (var searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE DNSDomain IS NOT NULL"))
			using (var results = searcher.Get())
			{
				if (results != null && results.Count > 0)
				{
					using (var networkAdapter = results.Cast<ManagementObject>().First())
					{
						foreach (var property in networkAdapter.Properties)
						{
							if (property.Name.Equals("MACAddress"))
							{
								MacAddress = Convert.ToString(property.Value).Replace(":", "").ToUpper();
							}
						}
					}
				}
				else
				{
					MacAddress = "000000000000";
				}
			}
		}

		private void InitializePnPDevices()
		{
			var deviceList = new List<string>();

			using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT DeviceID FROM Win32_PnPEntity"))
			using (var results = searcher.Get())
			{
				foreach (ManagementObject queryObj in results)
				{
					using (queryObj) 
					{ 
						foreach (var property in queryObj.Properties)
						{
							if (property.Name.Equals("DeviceID"))
							{
								deviceList.Add(Convert.ToString(property.Value).ToLower());
							}
						}
					}
				}

				PlugAndPlayDeviceIds = deviceList.ToArray();
			}
		}
	}
}
