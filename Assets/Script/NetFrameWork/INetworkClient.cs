using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 客户端接口
/// </summary>
public interface INetworkClient
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    event Action<GameMessage> MessageHandler;

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ipaddr"></param>
    /// <param name="port"></param>
    void Connect(string connectStr);

    /// <summary>
    /// 发送数据
    /// </summary>
    void Send(GameMessage msg);

    /// <summary>
    /// 是否连接
    /// </summary>
    bool Connected { get; }

    /// <summary>
    /// 关闭连接
    /// </summary>
    void Close();
}