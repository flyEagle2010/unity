using UnityEngine;  
using System.Collections;  

public class AnimationScript : MonoBehaviour  
{  
	void Start()  
	{  
		Animation animation = this.GetComponent<Animation>();//动画控制器  
		animation.Play("idle");//上来直接播放idle动画  
	}  
	void OnGUI()  
	{  
		if (GUI.Button(new Rect(0, 0, 100, 30), "行走"))  
		{  
			GetComponent<Animation>().Play("run");  
		}  
		if (GUI.Button(new Rect(100, 0, 100, 30), "停止"))  
		{  
			GetComponent<Animation>().Play("idle");  
		}  
		if (GUI.Button(new Rect(200, 0, 100, 30), "攻击"))  
		{  
			GetComponent<Animation>().Play("attack");  
			GetComponent<Animation>().PlayQueued("idle");//播放完attack之后再播放idle  
		}  
	}  
} 