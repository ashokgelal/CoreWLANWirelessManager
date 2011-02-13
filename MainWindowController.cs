
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreWlan;
using System.Text;
using MonoMac.ObjCRuntime;

namespace CoreWLANWirelessManager
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		public CWInterface	CurrentInterface { get; set; }
		
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		public void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		
		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			supportedInterfacesPopup.SelectItem (0);
			interfaceSelected(null);
		}
		
		partial void interfaceSelected (NSObject sender)
		{
			CurrentInterface = CWInterface.FromName(supportedInterfacesPopup.SelectedItem.Title);
			UpdateInterfaceInfoTab();
		}
		
		private void UpdateInterfaceInfoTab ()
		{
			bool powerState = CurrentInterface.Power;
			powerStateControl.SetSelected (true, powerState ? 0 : 1);
			bool isRunning = (CWInterfaceState)CurrentInterface.InterfaceState.Int32Value == CWInterfaceState.CWInterfaceStateRunning;
			
			if (isRunning)
				disconnectButton.Enabled = true;
			else
				disconnectButton.Enabled = false;
			
			int num = CurrentInterface.OpMode.Int32Value;
			opModeField.StringValue = powerState ? StringForOpMode ((CWOpMode)num) : String.Empty;
			
			num = CurrentInterface.SecurityMode.Int32Value;
			securityModeField.StringValue = powerState ? StringForSecurityMode ((CWSecurityMode)num) : String.Empty;
			
			num = CurrentInterface.PhyMode.Int32Value;
			phyModeField.StringValue = powerState ? StringForPhyMode ((CWPHYMode)num) : String.Empty;
			
			string str = CurrentInterface.Ssid;
			ssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			str = CurrentInterface.Bssid;
			bssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			num = CurrentInterface.TxRate.Int32Value;
			txRateField.StringValue = powerState ? String.Format ("{0} Mbps", num) : String.Empty;
			
			num = CurrentInterface.Rssi.Int32Value;
			rssiField.StringValue = powerState ? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.Noise.Int32Value;
			noiseField.StringValue = powerState ? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.TxPower.Int32Value;
			txPowerField.StringValue = powerState ? String.Format ("{0} mW", num) : String.Empty;
			
			str = CurrentInterface.CountryCode;
			countryCodeField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			NSNumber[] supportedChannelsArray = CurrentInterface.SupportedChannels;
			StringBuilder tempString = new StringBuilder ();
			channelPopup.RemoveAllItems ();
			
			foreach (NSNumber eachChannel in supportedChannelsArray)
			{
				if (eachChannel.IsEqualToNumber (supportedChannelsArray.Last ()))
					tempString.Append (eachChannel.ToString ());
				else
					tempString.AppendFormat ("{0}, ", eachChannel.ToString ());
				if (powerState)
					channelPopup.AddItem (eachChannel.ToString ());
			}
			
			supportedChannelsField.StringValue = tempString.ToString ();
			
			NSNumber[] supportedPhyModesArray = CurrentInterface.SupportedPhyModes;
			tempString = new StringBuilder ("802.11");
			
			foreach (NSNumber eachPhyMode in supportedPhyModesArray) {
				switch ((CWPHYMode)eachPhyMode.Int32Value)
				{
					case CWPHYMode.CWPHYMode11A:
						tempString.Append("a/");
						break;
					case CWPHYMode.CWPHYMode11B:
						tempString.Append("b/");
						break;
					case CWPHYMode.CWPHYMode11G:
						tempString.Append("g/");
						break;
					case CWPHYMode.CWPHYMode11N:
						tempString.Append("n");
						break;
				}
			}
			
			string phymode = tempString.ToString();
			if(phymode.EndsWith("/"))
				phymode.Remove(phymode.Length-1);
			if(phymode.Equals("802.11"))
				phymode = "None";
				
			supportedPHYModes.StringValue = phymode;
			
			channelPopup.SelectItem(CurrentInterface.Channel.ToString());
			if(!powerState || isRunning)
				channelPopup.Enabled = false;
			else
				channelPopup.Enabled = true;
		}
		
		private static string StringForPhyMode (CWPHYMode mode)
		{
			string phyModeStr = String.Empty;
			switch (mode) 
			{
				case CWPHYMode.CWPHYMode11A:
				{
					phyModeStr = "802.11a";
					break;
				}
				case CWPHYMode.CWPHYMode11B:
				{
					phyModeStr = "802.11b";
					break;
				}
				case CWPHYMode.CWPHYMode11G:
				{
					phyModeStr = "802.11g";
					break;
				}
				case CWPHYMode.CWPHYMode11N:
				{
					phyModeStr = "802.11n";
					break;
				}

			}
			return phyModeStr;
		}
		
		private static string StringForSecurityMode (CWSecurityMode mode)
		{
			string securityModeStr = String.Empty;
			switch (mode) 
			{
				case CWSecurityMode.CWSecurityModeOpen:
				{
					securityModeStr = "Open";
					break;
				}
				case CWSecurityMode.CWSecurityModeWEP:
				{
					securityModeStr = "WEP";
					break;
				}
				case CWSecurityMode.CWSecurityModeWPA_PSK:
				{
					securityModeStr = "WPA Personal";
					break;
				}
				case CWSecurityMode.CWSecurityModeWPA_Enterprise:
				{
					securityModeStr = "WPA Enterprise";
					break;
				}
				case CWSecurityMode.CWSecurityModeWPA2_PSK:
				{
					securityModeStr = "WPA2 Personal";
					break;
				}
				case CWSecurityMode.CWSecurityModeWPA2_Enterprise:
				{
					securityModeStr = "WPA2 Enterprise";
					break;
				}
				case CWSecurityMode.CWSecurityModeWPS:
				{
					securityModeStr = "WiFI Protected Setup";
					break;
				}
				case CWSecurityMode.CWSecurityModeDynamicWEP:
				{
					securityModeStr = "802.1X WEP";
					break;
				}
			}
			return securityModeStr;
		}
		
		private static string StringForOpMode (CWOpMode mode)
		{
			string opModeStr = String.Empty;
			switch (mode) 
			{
				case CWOpMode.CWOpModeIBSS:
				{
					opModeStr = "IBSS";
					break;
				}
				case CWOpMode.CWOpModeStation:
				{
					opModeStr = "Infrastructure";
					break;
				}
				case CWOpMode.CWOpModeHostAP:
				{
					opModeStr = "Host Access Point";
					break;
				}
				case CWOpMode.CWOpModeMonitorMode:
				{
					opModeStr = "Monitor Mode";
					break;
				}
			}
			return opModeStr;
		}
		
		private static CWSecurityMode SecurityModeForString (string modeStr)
		{
			switch (modeStr)
			{
			case "WEP":
				return CWSecurityMode.CWSecurityModeWEP;
			case "WPA Personal":
				return CWSecurityMode.CWSecurityModeWPA_PSK;
			case "WPA2 Personal":
				return CWSecurityMode.CWSecurityModeWPA2_PSK;
			case "WPA Enterprise":
				return CWSecurityMode.CWSecurityModeWPA_Enterprise;
			case "WPA2 Enterprise":
				return CWSecurityMode.CWSecurityModeWPA2_Enterprise;
			case "802.1X WEP":
				return CWSecurityMode.CWSecurityModeDynamicWEP;
			}
			return CWSecurityMode.CWSecurityModeOpen;
			
		}
		
		public override void AwakeFromNib ()
		{
			supportedInterfacesPopup.AddItems(CWInterface.SupportedInterfaces);
		}
	}
}

