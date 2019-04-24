using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
namespace YEvent
{
    

    /// <summary>
    ///  事件处理中心 根据 EventTpye 处理事件
    /// </summary>
    public class EventCenter
    {
        /// <summary>
        /// 存放 所有事件   枚举EventType 作为Key 值就是委托
        /// </summary>
        private static Dictionary<EventType, Delegate> m_EventTable = new Dictionary<EventType, Delegate>();

        /// <summary>
        /// 在添加事件的时候 调用  检查 表中是否存在该 类型的事件，不存在就添加 进去，
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        private static void OnListenerAdding(EventType eventType, Delegate callBack)
        {
            if (!m_EventTable.ContainsKey(eventType))
            {
                m_EventTable.Add(eventType, null);
            }
            Delegate d = m_EventTable[eventType];
            if (d != null && d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
            }
        }
        /// <summary>
        ///  移除 监听的时候调用 ， 检测 表中是否 存在该事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="callBack"></param>
        private static void OnListenerRemoving(EventType eventType, Delegate callBack)
        {
            if (m_EventTable.ContainsKey(eventType))
            {
                Delegate d = m_EventTable[eventType];
                if (d == null)
                {
                    throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", eventType));
                }
                else if (d.GetType() != callBack.GetType())
                {
                    throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1}，要移除的委托类型为{2}", eventType, d.GetType(), callBack.GetType()));
                }
            }
            else
            {
                throw new Exception(string.Format("移除监听错误：没有事件码{0}", eventType));
            }
        }
        private static void OnListenerRemoved(EventType eventType)
        {
            if (m_EventTable[eventType] == null)
            {
                m_EventTable.Remove(eventType);
            }
        }

        #region 添加事件
        //no parameters
        public static void AddListener(EventType eventType, CallBack callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
        }
        //Single parameters
        public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
        }
        //two parameters
        public static void AddListener<T, X>(EventType eventType, CallBack<T, X> callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] + callBack;
        }
        //three parameters
        public static void AddListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
        }
        //four parameters
        public static void AddListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
        }
        //five parameters
        public static void AddListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
        {
            OnListenerAdding(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
        }
        #endregion

        #region  移除事件
        //no parameters
        public static void RemoveListener(EventType eventType, CallBack callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
        //single parameters
        public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
        //two parameters
        public static void RemoveListener<T, X>(EventType eventType, CallBack<T, X> callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
        //three parameters
        public static void RemoveListener<T, X, Y>(EventType eventType, CallBack<T, X, Y> callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
        //four parameters
        public static void RemoveListener<T, X, Y, Z>(EventType eventType, CallBack<T, X, Y, Z> callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }
        //five parameters
        public static void RemoveListener<T, X, Y, Z, W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack)
        {
            OnListenerRemoving(eventType, callBack);
            m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
            OnListenerRemoved(eventType);
        }

        #endregion

        #region 广播事件
        //no parameters
        public static void Broadcast(EventType eventType)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack callBack = d as CallBack;
                if (callBack != null)
                {
                    callBack();
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        //single parameters
        public static void Broadcast<T>(EventType eventType, T arg)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack<T> callBack = d as CallBack<T>;
                if (callBack != null)
                {
                    callBack(arg);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        //two parameters
        public static void Broadcast<T, X>(EventType eventType, T arg1, X arg2)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack<T, X> callBack = d as CallBack<T, X>;
                if (callBack != null)
                {
                    callBack(arg1, arg2);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        //three parameters
        public static void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        //four parameters
        public static void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3, arg4);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        //five parameters
        public static void Broadcast<T, X, Y, Z, W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
                if (callBack != null)
                {
                    callBack(arg1, arg2, arg3, arg4, arg5);
                }
                else
                {
                    throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
                }
            }
        }
        #endregion
    }
}
