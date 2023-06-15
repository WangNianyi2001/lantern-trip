using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

// Composite: 能选择AbortType
// ParentTask: 能作为父节点

// In class Selector::
// property::
//   currentChildIndex : 当前所执行任务的子节点
// override function::
//   void OnChildExecuted(TaskStatus childStatus) : 子节点开始执行时的Callback
//   Int CurrentChildIndex() : 返回currentChildIndex，由此决定执行哪个子节点

// In class RandomSelector::
// ShuffleChilden() : OnStart() 时进行一次洗牌算法，确定执行顺序，生成一个栈

[TaskDescription("按照不同概率执行子节点, p_n为1时为最后一个结点")]
public class CustomSelector : Composite
    {
        // [BehaviorDesigner.Runtime.Tasks.Tooltip("Seed the random number generator to make things easier to debug")]
        // public int ChildTaskNum = 0;

        public SharedFloat p_0;
        public SharedFloat p_1;
        public SharedFloat p_2;
        public SharedFloat p_3;
        public SharedFloat p_4;
        public SharedFloat p_5;
        public SharedFloat p_6;

        // A list of indexes of every child task. This list is used by the Fischer-Yates shuffle algorithm.
        private List<int> childIndexList = new List<int>();
        // The random child index execution order.
        private Stack<int> childrenExecutionOrder = new Stack<int>();
        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override void OnAwake()
        {


            // Add the index of each child to a list to make the Fischer-Yates shuffle possible.
            childIndexList.Clear();
            for (int i = 0; i < children.Count; ++i) {
                childIndexList.Add(i);
            }
        }

        public override void OnStart()
        {
            // Randomize the indecies
            ShuffleChilden();
        }

        public override int CurrentChildIndex()
        {
            // Peek will return the index at the top of the stack.
            return childrenExecutionOrder.Peek();
        }

        public override bool CanExecute()
        {
            // Continue exectuion if no task has return success and indexes still exist on the stack.
            return childrenExecutionOrder.Count > 0 && executionStatus != TaskStatus.Success;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Pop the top index from the stack and set the execution status.
            if (childrenExecutionOrder.Count > 0) {
                childrenExecutionOrder.Pop();
            }
            executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Start from the beginning on an abort
            childrenExecutionOrder.Clear();
            executionStatus = TaskStatus.Inactive;
            ShuffleChilden();
        }

        public override void OnEnd()
        {
            // All of the children have run. Reset the variables back to their starting values.
            executionStatus = TaskStatus.Inactive;
            childrenExecutionOrder.Clear();
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
        }

        // 洗牌
        private void ShuffleChilden()
        {
            Random.InitState((int)Time.time);
            // Use Fischer-Yates shuffle to randomize the child index order.
            for (int i = childIndexList.Count; i > 0; --i) {
                // int j;
                // j = Random.Range(0, childIndexList.Count-1);
                int m;
                
                float j = Random.Range(0f, 1f);
                if (j < p_0.Value)
                {
                    m = 0;
                }else
                if (j < p_1.Value)
                {
                    m = 1;
                }else
                if (j < p_2.Value)
                {
                    m = 2;
                }else
                if (j < p_3.Value)
                {
                    m = 3;
                }else
                if (j < p_4.Value)
                {
                    m = 4;
                }else
                if (j < p_5.Value)
                {
                    m = 5;
                }else
                if (j < p_6.Value)
                {
                    m = 6;
                }
                else
                {
                    m = 7;
                }

                m = Mathf.Clamp(m, 0, childIndexList.Count-1);
                
                // 存入List
                int index = m;
                
                // if (childrenExecutionOrder.Contains(index))
                // {
                //     i++;
                //     continue;
                // }
                childrenExecutionOrder.Push(index);
            }
        }
    }
