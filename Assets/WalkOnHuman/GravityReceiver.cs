using System.Collections.Generic;
using UnityEngine;

namespace cdecpgl.WalkOnHuman {
	public class GravityReceiver : MonoBehaviour {
		[SerializeField, Tooltip("重力による加速度を設定します。")]
		public float acceleration = 1.0f;
		[SerializeField, Tooltip("重力方向を下向きとして、オブジェクトの向きを合わせるかどうかを設定します。")]
		public bool isAdjustRotation = true;
		[SerializeField, Tooltip("RigidBodyによる重力を強制的に無効化します。")]
		public bool forceToInvalidateRigidBodyGravity = false;

		private Rigidbody _my_rigidbody;
		private Vector3 _gravity_direction_vector; //重力方向を示すベクトル。正規化されている。
		private struct GravityData {
			public Vector3 direction_vector; //重力の方向を表すベクトル。正規化されている。
			public float distance_sgr;
		}
		private List<GravityData> _unhandled_gravity_list = new List<GravityData>(); //未処理の重力がリストとして保持されている。

		private static readonly float DISTANCE_LOWER_LIMIT = 0.1f; //距離の最小値を示す定数

		// Use this for initialization
		private void Start() {
			_my_rigidbody = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		private void Update() {
			HandleGravityList();
			if (isAdjustRotation) {
				var up_vec = -_gravity_direction_vector;
				AdjustRotation(up_vec);
			}
		}

		private void FixedUpdate() {
			if (forceToInvalidateRigidBodyGravity) {
				_my_rigidbody.useGravity = false;
			}
			_my_rigidbody.AddForce(_gravity_direction_vector * acceleration, ForceMode.Acceleration);
		}

		private void OnTriggerStay(Collider other) {
			var ga = other.GetComponent<GravityArea>();
			//GravityAreaを持っているものの重力方向を計算する
			if (ga != null) {
				Vector3 my_pos = transform.position;
				Vector3 seg_s = ga.transform.TransformPoint(ga.gravityCoreCapsuleStart);
				Vector3 seg_e = ga.transform.TransformPoint(ga.gravityCoreCapsuleEnd);
				var h = Distance.GetNearestPositionOnSegment(my_pos, seg_s, seg_e);
				GravityData gd;
				gd.direction_vector = (h - my_pos).normalized;
				float dis = (h - my_pos).magnitude;
				if (dis < DISTANCE_LOWER_LIMIT) { dis = DISTANCE_LOWER_LIMIT; }
				gd.distance_sgr = Mathf.Pow(dis, 2);
				_unhandled_gravity_list.Add(gd);
			}
		}

		//受けた重力リストを処理する
		private void HandleGravityList() {
			if (_unhandled_gravity_list.Count > 0) {
				_gravity_direction_vector.Set(0, 0, 0);
				//距離を加味した重力ベクトルの平均を求める。
				foreach (var gd in _unhandled_gravity_list) {
					_gravity_direction_vector += gd.direction_vector / gd.distance_sgr;
				}
				_unhandled_gravity_list.Clear();
				_gravity_direction_vector.Normalize();
			}
		}

		//自分の方向を重力に合わせる
		private void AdjustRotation(Vector3 up_vec) {
			//正面位置を保持しておく
			var look_tar = transform.position + transform.forward;
			//上を向ける
			transform.up = up_vec;
			//正面方向を自分の高さに合わせる
			look_tar = transform.InverseTransformPoint(look_tar);
			look_tar.y = 0;
			look_tar = transform.TransformPoint(look_tar);
			//正面方向に向かせる
			transform.LookAt(look_tar, up_vec);
		}

		private void OnDrawGizmosSelected() {
			//重力方向を描画
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(transform.position, _gravity_direction_vector);
		}
	}

	//距離を求める関数群
	public class Distance {
		//直線上の、点と最短距離にある位置を求める(点座標、直線始点、直線方向)
		public static Vector3 GetNearestPositionOnLine(Vector3 point, Vector3 line_s, Vector3 line_d) {
			float length_sqrt = line_d.sqrMagnitude;
			var t = Vector3.Dot(line_d, point - line_s) / length_sqrt;
			var h = line_s + t * line_d;
			return h;
		}
		//線分上の、点と最短距離にある位置を求める(点座標、線分始点、線分終点)
		public static Vector3 GetNearestPositionOnSegment(Vector3 point, Vector3 seg_s, Vector3 seg_e) {
			//点が始点の外側
			if (IsSharpAngle(point, seg_s, seg_e) == false) {
				return seg_s;
			}
			//点が終点の外側
			else if (IsSharpAngle(point, seg_e, seg_s) == false) {
				return seg_e;
			}
			//点が線分内
			else {
				return GetNearestPositionOnLine(point, seg_s, seg_e - seg_s);
			}
		}
		//角point1,point2,point3が鋭角かどうかを調べる
		public static bool IsSharpAngle(Vector3 point1, Vector3 point2, Vector3 point3) {
			return Vector3.Dot(point1 - point2, point3 - point2) > 0;
		}
	}
}