using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShakeWave
{
	
	public class DestorySelf : MonoBehaviour
	{
		public float mLiveTime = 4.0f;

		// Update is called once per frame
		void Update ()
		{
			mLiveTime -= Time.deltaTime;

			if ( mLiveTime <= 0.0f )
				GameObject.Destroy (this.gameObject);
		}
	}

}
