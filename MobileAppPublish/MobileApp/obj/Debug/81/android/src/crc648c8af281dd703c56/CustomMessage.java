package crc648c8af281dd703c56;


public class CustomMessage
	extends android.widget.Toast
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("PMS.Droid.CustomMessage, MobileApp", CustomMessage.class, __md_methods);
	}


	public CustomMessage (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomMessage.class)
			mono.android.TypeManager.Activate ("PMS.Droid.CustomMessage, MobileApp", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
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
