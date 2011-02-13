using System;
namespace CoreWLANWirelessManager
{
	public enum CWInterfaceState
	{
		CWInterfaceStateInactive = 0,
		CWInterfaceStateScanning,
		CWInterfaceStateAuthenticating,
		CWInterfaceStateAssociating,
		CWInterfaceStateRunning
	}
	
	public enum CWSecurityMode{
		CWSecurityModeOpen	= 0,
		CWSecurityModeWEP,
		CWSecurityModeWPA_PSK,
		CWSecurityModeWPA2_PSK,
		CWSecurityModeWPA_Enterprise,
		CWSecurityModeWPA2_Enterprise,
		CWSecurityModeWPS,
		CWSecurityModeDynamicWEP
	}
	
	public enum CWOpMode{
		CWOpModeStation	= 0,
		CWOpModeIBSS,
		CWOpModeMonitorMode,
		CWOpModeHostAP
	}
	
	public enum CWPHYMode{
		CWPHYMode11A	= 0,
		CWPHYMode11B,
		CWPHYMode11G,
		CWPHYMode11N
	}
}

