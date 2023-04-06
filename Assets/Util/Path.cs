using System;
using System.Collections;
using UnityEngine;

namespace LanternTrip {
	[Serializable]
	public class PathNode {
		public Transform PathPoint;//路径点
		public float MoveTime = 0f;//移动时间
		[HideInInspector] public float cur_MoveTime = 0f;//当前移动时间
		public float WaitTime = 0f;//等待时间
		[HideInInspector] public Vector3 Speed;//移动速度

	}

	public class Path {
		// https://developer.mozilla.org/en-US/docs/Web/CSS/animation-direction#examples
		public enum MoveMode {
			Normal,
			Reverse,
			Loop,
			Alternate,
		}
		public PathNode[] path = new PathNode[0];   // 路径点
		private int Id = 0;
		public Transform target;    // 要移动的物体

		// public Vector3 startPoint => path[0].PathPoint.position;
		// public Vector3 targetPoint => path[path.Length - 1].PathPoint.position;


		// 在void Start()中调用
		public void Init(PathNode[] _path, Transform _target) {
			path = _path;
			target = _target;

			InitMoveTime();

			path[0].Speed = (path[0].PathPoint.position - target.position) / path[0].MoveTime;
			for(int i = 1; i < path.Length; i++) {
				path[i].Speed = (path[i].PathPoint.position - path[i - 1].PathPoint.position) / path[i].MoveTime;
			}
		}

		private void InitMoveTime() {
			foreach(var node in path) {
				node.cur_MoveTime = node.MoveTime;
			}
		}

		// 在void OnDrawGizmos()中调用
		public void Display() {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(target.position, path[0].PathPoint.position);
			for(int i = 0; i < path.Length - 1; i++) {
				if(path[i].PathPoint && path[i + 1].PathPoint) {
					Gizmos.DrawLine(path[i].PathPoint.position, path[i + 1].PathPoint.position);
				}
			}
		}

		public void MoveAlongPath(MoveMode moveMode) {


			switch(moveMode) {
				case MoveMode.Normal:
					CoroutineHelper.Run(MoveNormal());
					break;
				case MoveMode.Reverse:
					CoroutineHelper.Run(MoveReverse());
					break;
				case MoveMode.Loop:
					CoroutineHelper.Run(MoveLoop());
					break;
				case MoveMode.Alternate:
					CoroutineHelper.Run(MoveAlternate());
					break;
				default:
					break;
			}
		}


		IEnumerator MoveNormal() {
			path[0].Speed = (path[0].PathPoint.position - target.position) / path[0].MoveTime;
			while(Id < path.Length) {
				PathNode p = path[Id];
				//当移动的时间大于0时让物体向下一个点移动
				if(p.cur_MoveTime > 0) {
					p.cur_MoveTime -= Time.deltaTime;
					target.position += p.Speed * Time.deltaTime;
				}
				else {
					//当等待的时间大于0时，物体停止不动等待时间归零
					target.position = p.PathPoint.position;
					if(p.WaitTime > 0) {
						p.WaitTime -= Time.deltaTime;
					}
					else {
						Id++;
					}
				}

				yield return null;
			}

		}

		IEnumerator MoveReverse() {
			target.position = path[path.Length - 1].PathPoint.position;
			while(Id < path.Length - 1) {
				PathNode p_pre = path[path.Length - 2 - Id];
				PathNode p = path[path.Length - 1 - Id];
				//当移动的时间大于0时让物体向下一个点移动
				if(p.cur_MoveTime > 0) {
					p.cur_MoveTime -= Time.deltaTime;
					target.position -= p.Speed * Time.deltaTime;
				}
				else {
					//当等待的时间大于0时，物体停止不动等待时间归零
					target.position = p_pre.PathPoint.position;
					if(p.WaitTime > 0) {
						p.WaitTime -= Time.deltaTime;
					}
					else {
						Id++;
					}
				}

				yield return null;

			}
		}

		// 一直走正循环
		IEnumerator MoveLoop() {
			while(true) {
				Id = 0;
				InitMoveTime();
				yield return CoroutineHelper.Run(MoveNormal());
			}
		}

		// 交替走正负循环
		IEnumerator MoveAlternate() {
			while(true) {
				Id = 0;
				InitMoveTime();
				yield return CoroutineHelper.Run(MoveNormal());

				Id = 0;
				InitMoveTime();
				yield return CoroutineHelper.Run(MoveReverse());
			}
		}
	}
}
