using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
	public enum PageType {SaveAndLoad, Config}

	public Animator anim;
	public PageType pageType;

	public virtual void Open() {
		anim.SetTrigger("Open");
	}

	public virtual void Close(bool closeAllMenus = false) {
		anim.SetTrigger("Close");

		if(closeAllMenus)
			VNMenuManager.instance.CloseRoot();
	}
}
