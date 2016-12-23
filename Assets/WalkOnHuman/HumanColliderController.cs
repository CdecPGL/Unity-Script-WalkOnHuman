using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cdecpgl.WalkOnHuman {
	public class HumanColliderController : MonoBehaviour {
		public int layerOfCollider = 0;
		public bool isTreateNonSkinnedMesh = true;
		public bool enableRealtimeUpdate = false;
		public int updateIntervalByFrame = 1;
		public bool enableTimeLagUpdate = false;
		public int handleCountPerOnce = 1;

		//Colliderをすべて更新
		public void UpdateAllColliders() {
			foreach (var dat in _skinned_mesh_data_list) {
				UpdateMeshColliderBySkinnedMeshRenderer(dat);
			}
		}

		private struct MeshData {
			public MeshCollider mesh_colldier;
			public MeshFilter mesh_filter;
		}
		private struct SkinnedMeshData {
			public MeshCollider mesh_colldier;
			public SkinnedMeshRenderer skinned_mesh_renderer;
		}
		private List<SkinnedMeshData> _skinned_mesh_data_list = new List<SkinnedMeshData>();
		private List<MeshData> _mesh_data_list = new List<MeshData>();
		private int _frame_count = 0;
		private int _last_update_data_index = 0;

		// Use this for initialization
		private void Start() {
			//自分とすべての子からデータを取得
			RecursiveDataSetter(transform);
			//スキンメッシュを更新
			UpdateAllColliders();
			//通常メッシュを更新
			if (isTreateNonSkinnedMesh) {
				foreach (var dat in _mesh_data_list) {
					UpdateMeshColliderByMeshRenderer(dat);
				}
			}
		}

		// Update is called once per frame
		private void Update() {
			if (enableRealtimeUpdate) {
				if (_frame_count % updateIntervalByFrame == 0) {
					//MeshColliderを更新
					if (enableTimeLagUpdate) {
						//データ数が設定された処理数より少なかったら、データ数を処置数にする。
						int handle_count = handleCountPerOnce;
						if (handle_count > _skinned_mesh_data_list.Count) { handle_count = _skinned_mesh_data_list.Count; }
						//処理数だけ処理を行う
						for (int i = 0; i < handle_count; ++i) {
							int idx = _last_update_data_index + i + 1;
							idx %= _skinned_mesh_data_list.Count;
							var dat = _skinned_mesh_data_list[idx];
							UpdateMeshColliderBySkinnedMeshRenderer(dat);
						}
						//最後に処理を行ったデータのインデックスを更新
						_last_update_data_index += handle_count;
						_last_update_data_index %= _skinned_mesh_data_list.Count;
					} else {
						foreach (var dat in _skinned_mesh_data_list) {
							UpdateMeshColliderBySkinnedMeshRenderer(dat);
						}
					}
				}
				++_frame_count;
			}
		}

		private void RecursiveDataSetter(Transform trans) {
			var smr = trans.GetComponent<SkinnedMeshRenderer>();
			var mf = trans.GetComponent<MeshFilter>();
			if (smr != null) {
				var mc = trans.GetComponent<MeshCollider>();
				if (mc == null) {
					mc = trans.gameObject.AddComponent<MeshCollider>();
				}
				mc.gameObject.layer = layerOfCollider;
				SkinnedMeshData dat = new SkinnedMeshData();
				dat.mesh_colldier = mc;
				dat.skinned_mesh_renderer = smr;
				_skinned_mesh_data_list.Add(dat);
			} else if (isTreateNonSkinnedMesh && (mf != null)) {
				var mc = trans.GetComponent<MeshCollider>();
				if (mc == null) {
					mc = trans.gameObject.AddComponent<MeshCollider>();
				}
				mc.gameObject.layer = layerOfCollider;
				MeshData dat = new MeshData();
				dat.mesh_colldier = mc;
				dat.mesh_filter = mf;
				_mesh_data_list.Add(dat);
			}
			var cc = trans.childCount;
			for (int i = 0; i < cc; ++i) {
				RecursiveDataSetter(trans.GetChild(i));
			}
		}

		private void UpdateMeshColliderBySkinnedMeshRenderer(SkinnedMeshData dat) {
			Mesh m = new Mesh();

			dat.skinned_mesh_renderer.BakeMesh(m);
			//rootBoneがあるときは拡大されてしまい、ないときはそのままなので修正する。原因を探る必要があり。
			if (dat.skinned_mesh_renderer.rootBone != null) {
				//https://forum.unity3d.com/threads/bakemesh-scales-wrong.442212/?_ga=1.243039813.639197362.1469713088
				Vector3[] verts = m.vertices;
				//float scale = 1.0f / transform.lossyScale.y;
				float scale = 1.0f / dat.skinned_mesh_renderer.rootBone.lossyScale.y;
				for (int i = 0; i < verts.Length; i++) {
					verts[i] = verts[i] * scale;
				}
				m.vertices = verts;
				m.RecalculateBounds();
			}

			dat.mesh_colldier.sharedMesh = null;
			dat.mesh_colldier.sharedMesh = m;
		}

		private void UpdateMeshColliderByMeshRenderer(MeshData dat) {
			dat.mesh_colldier.sharedMesh = null;
			dat.mesh_colldier.sharedMesh = dat.mesh_filter.sharedMesh;
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(HumanColliderController))]
		public class HumanColliderControllerEditor : Editor {
			public override void OnInspectorGUI() {
				var hcc = (HumanColliderController)target;

				hcc.layerOfCollider = EditorGUILayout.LayerField(new GUIContent("Layer Of Collider", "生成されるColliderのレイヤーを設定します。"), hcc.layerOfCollider);
				hcc.isTreateNonSkinnedMesh = EditorGUILayout.Toggle(new GUIContent("Is Treate Non Skinned Mesh", "有効にすると、MeshFilterを持つゲームオブジェクトにもMeshColliderが割り当てられます。"), hcc.isTreateNonSkinnedMesh);
				hcc.enableRealtimeUpdate = EditorGUILayout.Toggle(new GUIContent("Enable Realtime Update", "有効にすると、SkinnedMeshRendererの形状変化がリアルタイムにMeshColliderに反映されます。"), hcc.enableRealtimeUpdate);
				if (hcc.enableRealtimeUpdate) {
					++EditorGUI.indentLevel;
					hcc.updateIntervalByFrame = EditorGUILayout.IntField(new GUIContent("Update Interval By Frame", "MeshColliderの更新間隔をフレーム単位で指定します。"), hcc.updateIntervalByFrame);
					if (hcc.updateIntervalByFrame < 1) { hcc.updateIntervalByFrame = 1; }
					hcc.enableTimeLagUpdate = EditorGUILayout.Toggle(new GUIContent("Enable Time Lag Update", "有効にすると、一度にではなく分割してMeshColliderが更新されるようになります。"), hcc.enableTimeLagUpdate);
					if (hcc.enableTimeLagUpdate) {
						++EditorGUI.indentLevel;
						hcc.handleCountPerOnce = EditorGUILayout.IntField(new GUIContent("Handle Count Per Once", "フレームあたり何個のMeshColliderを更新するかを設定します。"), hcc.handleCountPerOnce);
						if (hcc.handleCountPerOnce < 1) { hcc.handleCountPerOnce = 1; }
						--EditorGUI.indentLevel;
					}
					--EditorGUI.indentLevel;
				}

				if (GUI.changed) {
					EditorUtility.SetDirty(target);
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene()); //v5.5用に修正
				}
			}
		}
#endif
	}
}