package md595c6c22f067718339f23215c28b6f4b2;


public class DownloadHelper
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AsyncProgressReporting.Common.DownloadHelper, HomeAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DownloadHelper.class, __md_methods);
	}


	public DownloadHelper () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DownloadHelper.class)
			mono.android.TypeManager.Activate ("AsyncProgressReporting.Common.DownloadHelper, HomeAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public DownloadHelper (android.widget.ProgressBar p0, android.widget.ImageView p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == DownloadHelper.class)
			mono.android.TypeManager.Activate ("AsyncProgressReporting.Common.DownloadHelper, HomeAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Widget.ProgressBar, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Widget.ImageView, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}

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
