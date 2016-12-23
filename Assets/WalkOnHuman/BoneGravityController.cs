using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cdecpgl.WalkOnHuman {
	public class BoneGravityController : MonoBehaviour {
		public enum Accuracy { None, Rough, Fine };
		public Accuracy boneOutOfMeshCulling = Accuracy.None;
		public int layerOfGravityArea = 0;

		private int _total_bone_count = 0;
		private int _unique_bone_count = 0;
		private int _valid_bone_count = 0;
		private List<Transform> _bone_list = new List<Transform>();
		private List<SkinnedMeshRenderer> _skinned_mesh_renderer_list = new List<SkinnedMeshRenderer>();

		private GameObject _gravity_area_root;
		private struct GravityAreaData {
			public GameObject game_object;
			public SphereCollider collider;
			public GravityArea gravity_area;
			public Transform start_bone;
			public Transform end_bone;
		}

		private List<GravityAreaData> _gravity_area_data_list = new List<GravityAreaData>();

		// Use this for initialization
		private void Start() {
			//GravityAreaのルート作成
			_gravity_area_root = new GameObject("GravityAreaRoot");
			_gravity_area_root.layer = layerOfGravityArea;
			_gravity_area_root.transform.parent = transform;
			//SkinnedMeshrendererの取得
			_skinned_mesh_renderer_list.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
			//ボーンのスキャン
			ScanBones();
			CullBones();
			Debug.Log("[total/unique/valid] bone count:" + _total_bone_count + "/" + _unique_bone_count + "/" + _valid_bone_count);
			//GravityAreaの作成
			GenerateGravityArea();
			AllUpdateGravityArea();
		}

		// Update is called once per frame
		private void Update() {
			AllUpdateGravityArea();
		}

		//SkinnedMeshrendererからボーンを取得
		private void ScanBoneFromSkinnedMesh(SkinnedMeshRenderer smr) {
			var bs = smr.bones;
			_total_bone_count += bs.Length;
			foreach (var bone in smr.bones) {
				//重複しているものは除く
				if (!_bone_list.Contains(bone)) {
					_bone_list.Add(bone);
					++_unique_bone_count;
				}
			}
		}

		//ボーンをスキャン
		private void ScanBones() {
			foreach (var smr in _skinned_mesh_renderer_list) {
				ScanBoneFromSkinnedMesh(smr);
			}
		}

		//要件を満たさないボーンを削除
		private void CullBones() {
			for (int i = _bone_list.Count - 1; i >= 0; --i) {
				if (!CheckBoneAddable(_bone_list[i])) {
					_bone_list.RemoveAt(i);
				} else {
					++_valid_bone_count;
				}
			}
		}

		//追加して良いボーンか確認する
		private bool CheckBoneAddable(Transform bone) {
			return IsBoneInMesh(bone);
		}

		//ボーンがメッシュ内にあるかどうかを確認する
		private bool IsBoneInMesh(Transform bone) {
			if (boneOutOfMeshCulling == Accuracy.None) { return true; }
			//メッシュごとに確認
			foreach (var smr in _skinned_mesh_renderer_list) {
				//メッシュのバウンディングボックス外ならメッシュ外にある
				if (!smr.bounds.Contains(bone.position)) { continue; }
				if (boneOutOfMeshCulling == Accuracy.Rough) { return true; }
				return true;
				//メッシュ内にあるのかより詳細に調べる(正常に動作しない)
				//var bpos = bone.position;
				//var mesh = smr.sharedMesh;
				//var tris = mesh.triangles;
				//var vtxs = mesh.vertices;

				//if (smr.rootBone != null) {
				//	//float scale = 1.0f / bone.lossyScale.y;
				//	float scale = 1.0f / smr.rootBone.lossyScale.y;
				//	for (int i = 0; i < vtxs.Length; i++) {
				//		vtxs[i] = smr.rootBone.TransformPoint(vtxs[i]);
				//		vtxs[i] = vtxs[i] * scale;
				//	}
				//}

				//bool in_mesh = true;
				////それぞれのポリゴンの内外を判断
				//for (int idx = 0; idx < tris.Length; idx += 3) {
				//	Vector3[] tri_vtx = new Vector3[3];
				//	for (int i = 0; i < 3; ++i) {
				//		tri_vtx[i] = vtxs[tris[idx + i]];
				//	}
				//	//法線ベクトルを求める
				//	var norm_vec = Vector3.Cross(tri_vtx[2] - tri_vtx[0], tri_vtx[1] - tri_vtx[0]);
				//	var bpos_vec = bpos - tri_vtx[0];
				//	//法線ベクトルとボーンのベクトルの内積が正なら外側にある
				//	if (Vector3.Dot(norm_vec, bpos_vec) > 0) {
				//		in_mesh = false;
				//		break;
				//	}
				//}
				////メッシュ内にあったら終わり
				//if (in_mesh) { return true; }
			}
			return false;
		}

		//ボーンに沿ってGravityAreaを作成する
		private void GenerateGravityArea() {
			foreach (var bone in _bone_list) {
				var ccount = bone.childCount;
				for (int i = 0; i < ccount; ++i) {
					var cbone = bone.GetChild(i);
					//ボーンリストに含まれていないものだったらスキップする
					if (!_bone_list.Contains(cbone)) { continue; }
					GravityAreaData gad = new GravityAreaData();
					gad.start_bone = bone;
					gad.end_bone = cbone;
					//todo:コライダーをCapsuleにする
					gad.game_object = new GameObject("GravityArea_" + bone.name, typeof(SphereCollider), typeof(GravityArea));
					gad.game_object.layer = layerOfGravityArea;
					gad.game_object.transform.parent = bone;

					gad.game_object.transform.parent = _gravity_area_root.transform;
					gad.collider = gad.game_object.GetComponent<SphereCollider>();
					gad.gravity_area = gad.game_object.GetComponent<GravityArea>();
					_gravity_area_data_list.Add(gad);
				}
			}
		}

		//すべてのGravityAreaを更新
		private void AllUpdateGravityArea() {
			foreach (var gad in _gravity_area_data_list) {
				UpdateGravityArea(gad);
			}
		}

		//GravityAreaを更新
		static private void UpdateGravityArea(GravityAreaData gad) {
			var b_gpos = gad.start_bone.position;
			var cb_gpos = gad.end_bone.position;
			var b_cb_cen_gpos = (b_gpos + cb_gpos) / 2;

			var b_lpos = gad.game_object.transform.InverseTransformPoint(b_gpos);
			var cb_lpos = gad.game_object.transform.InverseTransformPoint(cb_gpos);
			var b_cb_cen_lpos = gad.game_object.transform.InverseTransformPoint(b_cb_cen_gpos);

			var ccol = gad.collider;
			var garea = gad.gravity_area;
			ccol.radius = 10;
			ccol.center = b_cb_cen_lpos;
			ccol.isTrigger = true;
			garea.gravityCoreCapsuleRadius = 0;
			garea.gravityCoreCapsuleStart = b_lpos;
			garea.gravityCoreCapsuleEnd = cb_lpos;
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(BoneGravityController))]
		public class BoneGravityControllerEditor : Editor {
			public override void OnInspectorGUI() {
				var bgc = (BoneGravityController)target;

				bgc.boneOutOfMeshCulling = (Accuracy)EditorGUILayout.EnumPopup(new GUIContent("Bone Out Of Mesh Culling", "メッシュ外にあるボーンの除外処理を設定します"), bgc.boneOutOfMeshCulling);
				bgc.layerOfGravityArea = EditorGUILayout.LayerField(new GUIContent("Layer Of Gravity Area", "生成されるGravityAreaゲームオブジェクトのレイヤーを設定します。"), bgc.layerOfGravityArea);

				if (GUI.changed) {
					EditorUtility.SetDirty(target);
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene()); //v5.5用に修正
				}
			}
		}
#endif
	}
}