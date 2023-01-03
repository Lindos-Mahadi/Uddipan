package md54e9bdd3760bc4e5840a2ad3c7341d882;


public class MemberProductModel
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("PMS.Droid.Classes.OffLineHelpers.MemberProductModel, MobileApp", MemberProductModel.class, __md_methods);
	}


	public MemberProductModel ()
	{
		super ();
		if (getClass () == MemberProductModel.class)
			mono.android.TypeManager.Activate ("PMS.Droid.Classes.OffLineHelpers.MemberProductModel, MobileApp", "", this, new java.lang.Object[] {  });
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
