using System;
using UnityEngine;

public class TimeLine
{
    public float TimeSpeed { get; set; }

    private bool m_isStart; //是否开始

    private bool m_isPause; //是否暂停

    //当前计时
    private float m_curTime;

    //重置事件
    private Action m_reset;

    //每帧回调
    private Action<float> m_update;

    public TimeLine()
    {
        TimeSpeed = 1;

        Reset();
    }


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="id"></param>
    /// <param name="methord"></param>
    public void AddEvent(float delay, int id, Action<int> methord)
    {
        LineEvent param = new LineEvent(delay, id, methord);
        m_update += param.Invoke;
        m_reset += param.Reset;
    }

    public void Start()
    {
        Reset();

        m_isStart = true;
        m_isPause = false;
    }

    public void Pause()
    {
        m_isPause = true;
    }

    public void Resume()
    {
        m_isPause = false;
    }

    private void Reset()
    {
        m_curTime = 0;
        m_isStart = false;
        m_isPause = false;
        if (null != m_reset)
        {
            m_reset();
        }
    }

    internal void Loop(float deltaTime)
    {
        if (!m_isStart || m_isPause)
        {
            return;
        }

        m_curTime += deltaTime;
        if (null != m_update)
        {
            m_update(m_curTime);
        }
    }

    /// <summary>
    /// 时间线事件
    /// </summary>
    private class LineEvent
    {
        public float Delay { get; protected set; }
        public int Id { get; protected set; }

        public Action<int> Methord { get; protected set; }

        //是不是已经执行过了
        private bool m_isInvoke = false;

        public LineEvent(float delay, int id, Action<int> methord)
        {
            Delay = delay;
            Id = id;
            Methord = methord;

            Reset();
        }


        internal void Reset()
        {
            m_isInvoke = false;
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        /// <param name="time"></param>
        internal void Invoke(float time)
        {
            if (time < Delay)
            {
                return;
            }

            if (!m_isInvoke && null != Methord)
            {
                m_isInvoke = true;
                Methord(Id);
            }
        }
    }
}