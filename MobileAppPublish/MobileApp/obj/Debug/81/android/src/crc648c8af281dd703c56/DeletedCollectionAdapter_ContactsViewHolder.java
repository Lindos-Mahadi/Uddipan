package crc648c8af281dd703c56;


public class DeletedCollectionAdapter_ContactsViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("PMS.Droid.DeletedCollectionAdapter+ContactsViewHolder, MobileApp", DeletedCollectionAdapter_ContactsViewHolder.class, __md_methods);
	}


	public DeletedCollectionAdapter_ContactsViewHolder ()
	{
		super ();
		if (getClass () == DeletedCollectionAdapter_ContactsViewHolder.class)
			mono.android.TypeManager.Activate ("PMS.Droid.DeletedCollectionAdapter+ContactsViewHolder, MobileApp", "", this, new java.lang.Object[] {  });
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
