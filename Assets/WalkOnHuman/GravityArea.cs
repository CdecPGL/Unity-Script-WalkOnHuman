using UnityEngine;

namespace cdecpgl.WalkOnHuman {
	[RequireComponent(typeof(Collider))]
	public class GravityArea : MonoBehaviour {
		[SerializeField, Tooltip("重力の中心を示すカプセルの始点座標をローカル座標で設定します")]
		public Vector3 gravityCoreCapsuleStart = new Vector3();
		[SerializeField, Tooltip("重力の中心を示すカプセルの終点座標をローカル座標で設定します")]
		public Vector3 gravityCoreCapsuleEnd = new Vector3();
		[SerializeField, Tooltip("重力の中心を示すカプセルの半径をローカル座標で設定します")]
		public float gravityCoreCapsuleRadius = 0.5f;

		private void OnDrawGizmosSelected() {
			//重力中心カプセルの中心線描画
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.TransformPoint(gravityCoreCapsuleStart), transform.TransformPoint(gravityCoreCapsuleEnd));
			float radius = 0.5f;
			Gizmos.DrawSphere(transform.TransformPoint(gravityCoreCapsuleStart), radius);
			Gizmos.DrawSphere(transform.TransformPoint(gravityCoreCapsuleEnd), radius);
			//重力中心カプセルの両端と中間を描画
			Gizmos.color = Color.yellow;
			float global_radius = transform.TransformVector(new Vector3(gravityCoreCapsuleRadius, 1, 1)).x;
			Gizmos.DrawWireSphere(transform.TransformPoint(gravityCoreCapsuleStart), global_radius);
			Gizmos.DrawWireSphere(transform.TransformPoint(gravityCoreCapsuleEnd), global_radius);
			Gizmos.DrawWireSphere(transform.TransformPoint((gravityCoreCapsuleStart + gravityCoreCapsuleEnd) / 2), global_radius);
		}
	}
}