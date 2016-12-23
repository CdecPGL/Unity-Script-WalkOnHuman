using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cdecpgl.WalkOnHuman {
	public class AngleFreeFollowCamera : MonoBehaviour {
		public Transform target;
		[SerializeField, Tooltip("ターゲットから後ろ方向へどのくらい離れるかを設定します。")]
		public float distance = 5;
		public float height = 5;
		[SerializeField, Range(0.0f, 1.0f), Tooltip("値を大きくするほど、位置の追従が早くなります。")]
		public float positionLerpRate = 0.5f;
		[SerializeField, Range(0.0f, 1.0f), Tooltip("値を大きくするほど、回転度の追従が早くなります。")]
		public float rotationLerpRate = 0.5f;

		// Update is called once per frame
		private void Update() {
			if (target != null) {
				//位置を設定
				var target_pos = target.position;
				target_pos += (-target.forward).normalized * distance;
				target_pos += target.up.normalized * height;
				transform.position = Vector3.Slerp(transform.position, target_pos, positionLerpRate);
				//向きを設定
				var target_rotation = target.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, rotationLerpRate);
			}
		}
	}
}
