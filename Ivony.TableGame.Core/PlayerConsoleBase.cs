﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.TableGame
{

  /// <summary>
  /// 定义玩家控制台的抽象
  /// </summary>
  public abstract class PlayerConsoleBase
  {




    /// <summary>
    /// 创建玩家控制台对象
    /// </summary>
    /// <param name="playerHost">控制台所关联的玩家宿主</param>
    protected PlayerConsoleBase( IPlayerHost playerHost )
    {
      PlayerHost = playerHost;
    }




    /// <summary>
    /// 获取控制台所关联的玩家宿主
    /// </summary>
    protected IPlayerHost PlayerHost { get; private set; }



    /// <summary>
    /// 派生类实现此方法向客户端推送消息
    /// </summary>
    /// <param name="message">要推送的消息</param>
    public abstract void WriteMessage( GameMessage message );



    /// <summary>
    /// 从玩家客户端读取一条消息
    /// </summary>
    /// <param name="prompt">提示信息</param>
    /// <param name="token">取消标识</param>
    /// <returns>返回一个 Task 用于等待玩家响应</returns>
    public abstract Task<string> ReadLine( string prompt, CancellationToken token );


    /// <summary>
    /// 从玩家客户端读取一条消息
    /// </summary>
    /// <param name="prompt">提示信息</param>
    /// <param name="defaultValue">若玩家超时没有响应，所需要使用的默认值</param>
    /// <param name="token">取消标识</param>
    /// <returns>返回一个 Task 用于等待玩家响应</returns>
    public Task<string> ReadLine( string prompt, string defaultValue, CancellationToken token )
    {
      return ReadLine( prompt, defaultValue, DefaultTimeout, token );
    }


    /// <summary>
    /// 从玩家客户端读取一条消息
    /// </summary>
    /// <param name="prompt">提示信息</param>
    /// <param name="defaultValue">若玩家超时没有响应，所需要使用的默认值</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="token">取消标识</param>
    /// <returns>返回一个 Task 用于等待玩家响应</returns>
    public async Task<string> ReadLine( string prompt, string defaultValue, TimeSpan timeout, CancellationToken token )
    {

      var timeoutToken = new CancellationTokenSource( timeout ).Token;
      try
      {
        return await ReadLine( prompt, CancellationTokenSource.CreateLinkedTokenSource( timeoutToken, token ).Token );
      }
      catch ( OperationCanceledException )
      {
        if ( token.IsCancellationRequested )
          throw;

        return defaultValue;
      }

    }



    /// <summary>
    /// 让客户端在多个选项中选择一个
    /// </summary>
    /// <param name="prompt">提示信息</param>
    /// <param name="options">选项列表</param>
    /// <param name="token">取消标识</param>
    /// <returns>获取一个 Task 用于等待用户选择，并返回选择结果</returns>
    public virtual Task<Option> Choose( string prompt, Option[] options, CancellationToken token )
    {

      return ChooseImplement( prompt, options, token );
    }


    /// <summary>
    /// 派生类实现此方法以实现 Choose 功能
    /// </summary>
    /// <param name="prompt">提示信息</param>
    /// <param name="options">选项列表</param>
    /// <param name="token">取消标识</param>
    /// <returns>获取一个 Task 用于等待用户选择，并返回选择结果</returns>
    protected abstract Task<Option> ChooseImplement( string prompt, Option[] options, CancellationToken token );


    /// <summary>
    /// 让客户端在多个选项中选择一个
    /// </summary>
    /// <typeparam name="T">选项类型</typeparam>
    /// <param name="prompt">提示信息</param>
    /// <param name="options">选项列表</param>
    /// <param name="token">取消标识</param>
    /// <returns>获取一个 Task 用于等待用户选择，并返回选择结果</returns>
    public async Task<T> Choose<T>( string prompt, IOption<T>[] options, CancellationToken token ) where T : class
    {

      var dictionary = options.ToDictionary( item => item.OptionDescriptor, item => item.OptionValue );
      var option = await Choose( prompt, dictionary.Keys.ToArray(), token );

      return dictionary[option];
    }



    /// <summary>
    /// 让客户端在多个选项中选择一个
    /// </summary>
    /// <typeparam name="T">选项类型</typeparam>
    /// <param name="prompt">提示信息</param>
    /// <param name="options">选项列表</param>
    /// <param name="defaultOption">默认选项</param>
    /// <param name="token">取消标识</param>
    /// <returns>获取一个 Task 用于等待用户选择，并返回选择结果</returns>
    public Task<T> Choose<T>( string prompt, IOption<T>[] options, T defaultOption, CancellationToken token ) where T : class
    {
      return Choose( prompt, options, defaultOption, DefaultTimeout, token );
    }

    /// <summary>
    /// 让客户端在多个选项中选择一个
    /// </summary>
    /// <typeparam name="T">选项类型</typeparam>
    /// <param name="prompt">提示信息</param>
    /// <param name="options">选项列表</param>
    /// <param name="defaultOption">默认选项</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="token">取消标识</param>
    /// <returns>获取一个 Task 用于等待用户选择，并返回选择结果</returns>
    public async Task<T> Choose<T>( string prompt, IOption<T>[] options, T defaultOption, TimeSpan timeout, CancellationToken token ) where T : class
    {

      var timeoutToken = new CancellationTokenSource( timeout ).Token;
      try
      {
        return await Choose( prompt, options, CancellationTokenSource.CreateLinkedTokenSource( timeoutToken, token ).Token );
      }
      catch ( TaskCanceledException )
      {
        if ( token.IsCancellationRequested )
          throw;

        return defaultOption;
      }

    }


    /// <summary>
    /// 派生类重写此方法获取默认超时时间
    /// </summary>
    protected TimeSpan DefaultTimeout { get { return TimeSpan.FromMinutes( 1 ); } }




    /// <summary>
    /// 定义从 PlayerHostBase 到 PlayerConsoleBase 的隐式类型转换
    /// </summary>
    public static implicit operator PlayerConsoleBase( PlayerHostBase playerHost )
    {
      return playerHost.Console;
    }

    /// <summary>
    /// 定义从 GamePlayerBase 到 PlayerConsoleBase 的隐式类型转换
    /// </summary>
    public static implicit operator PlayerConsoleBase( GamePlayerBase player )
    {
      return player.PlayerHost.Console;
    }


  }
}
