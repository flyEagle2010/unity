using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditorInternal.UI
using UnityEngine.UI;


public class ScreenAdapt : MonoBehaviour {
	private float standard_width = 1344f;        	//初始宽度  1920 *70   1344*750 (iphone)
	private float standard_height = 756f;       	//初始高度   1080 *70
	private float device_width = 0f;                //当前设备宽度  
	private float device_height = 0f;               //当前设备高度  
	private float adjustor = 0f;         			//屏幕矫正比例  
	// Use this for initialization
	void Start () {

		//获取设备宽高  
		device_width = Screen.width;  
		device_height = Screen.height;  
		//计算宽高比例  
		float standard_aspect = standard_width / standard_height;  
		float device_aspect = device_width / device_height;  
		//计算矫正比例  
		if (device_aspect < standard_aspect)  
		{  
			adjustor = standard_aspect / device_aspect;  
		}  


		Canvas canvas = transform.GetComponent<Canvas> ();

		CanvasScaler canvasScalerTemp = transform.GetComponent<CanvasScaler>();  

		if (adjustor == 0)  
		{  
			canvasScalerTemp.matchWidthOrHeight = 1;  
		}else{  
			canvasScalerTemp.matchWidthOrHeight = 0;  
		}  
	}

	// Update is called once per frame
	void Update () {
		
	}
}
