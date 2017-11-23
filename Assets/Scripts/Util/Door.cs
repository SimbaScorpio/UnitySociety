using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class Door : MonoBehaviour
	{
		public Transform left;
		public Transform right;

		public float speed = 5f;
		public float currentRatio = 1f;

		// 0-idle 1-open 2-close
		public int state = 0;
		public int personInRange = 0;
		// 30s to force close if no one enter
		private float preventError = 30f;
		private float preventCount = 0f;

		void OnTriggerEnter (Collider collider)
		{
			if (collider.tag == "Player") {
				personInRange += 1;
				state = 1;
				preventCount = 0;
			}
		}

		void OnTriggerExit (Collider collider)
		{
			if (collider.tag == "Player") {
				personInRange -= 1;
				personInRange = personInRange < 0 ? 0 : personInRange;
				if (personInRange == 0)
					state = 2;
			}
		}

		void Update ()
		{
			if (state == 0) {
				return;
			} else if (state == 1) {
				currentRatio -= Time.deltaTime * speed;
				currentRatio = Mathf.Clamp (currentRatio, 0f, 1f);
				Scale (currentRatio);

				if (currentRatio <= 0.01f) {
					preventCount += Time.deltaTime;
					if (preventCount > preventError) {
						state = 2;
						personInRange = 0;
					}
				}

			} else if (state == 2) {
				currentRatio += Time.deltaTime * speed;
				currentRatio = Mathf.Clamp (currentRatio, 0f, 1f);
				Scale (currentRatio);
			}
		}

		void Scale (float ratio)
		{
			if (left != null)
				left.localScale = new Vector3 (ratio, left.localScale.y, left.localScale.z);
			if (right != null)
				right.localScale = new Vector3 (ratio, right.localScale.y, right.localScale.z);
		}
	}
}