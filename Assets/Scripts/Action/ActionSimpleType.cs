using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimpleType : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.SIMPLETYPE;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("SimpleType");
	}

	public void OnSimpleTypeFinished ()
	{
		// 这里非常有意思，为什么要考虑用线程？
		// 动作事件在一帧时间里会多次调用，导致下一个相同动作还没开始就结束了
		// 这里就确保回调事件比动作事件慢一帧
		StartCoroutine (wait ());
	}

	IEnumerator wait ()
	{
		yield return new WaitForEndOfFrame ();
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
