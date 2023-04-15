using UnityEngine;

namespace LanternTrip {
	/// <summary>
	/// 在对象字段的序列化值为 null 时，在 inspector 里显示「创建」的按钮；
	/// 不为 null 时，展开绘制子字段。
	/// </summary>
	public class InstanceAttribute : PropertyAttribute {
	}
}