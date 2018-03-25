package md593465df1b33ce7b139d48da70fe416b2;


public class DevicesActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("HomeAutomation.DevicesActivity, HomeAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DevicesActivity.class, __md_methods);
	}


	public DevicesActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DevicesActivity.class)
			mono.android.TypeManager.Activate ("HomeAutomation.DevicesActivity, HomeAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
