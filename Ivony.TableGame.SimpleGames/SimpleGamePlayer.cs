﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ivony.TableGame;
using Ivony.TableGame.Basics;

namespace Ivony.TableGame.SimpleGames
{
  public class SimpleGamePlayer : BasicGamePlayer
  {

    public SimpleGamePlayer( string codeName, IGameHost gameHost, IPlayerHost playerHost )
      : base( codeName, gameHost, playerHost )
    {
      Game = (SimpleGame) gameHost.Game;

      DefenceEffect = new DefenceEffectSlot();
      SpecialEffect = new SpecialEffectSlot();

      Health = 100;

    }


    public SimpleGame Game
    {
      get;
      private set;
    }


    public SimpleGameCard[] Cards
    {
      get { return CardCollection.Cast<SimpleGameCard>().ToArray(); }
    }



    /// <summary>
    /// 生命值
    /// </summary>
    public int Health
    {
      get;
      internal set;
    }

    public override async Task Play()
    {


      DealCards();

      GameHost.Game.AnnounceMessage( "轮到 {0} 出牌", CodeName );


      if ( SpecialEffect.Effect is DevilEffect )
      {
        var point = 10;
        SpecialEffect.Clear();
        Health += point;
        PlayerHost.WriteMessage( "您赢得了恶魔的契约，增加 HP {0} 点", point );
      }


      PlayerHost.WriteMessage( "HP:{0,-3}{1}{2} 卡牌:{3}", Health, DefenceEffect, SpecialEffect, string.Join( ", ", Cards.Select( item => item.Name ) ) );


      do
      {
        PlayCommand command = null;
        string commandText = null;

        try
        {
          commandText = await PlayerHost.Console.ReadLine( "请出牌： " );
        }
        catch ( TaskCanceledException )
        {
          Game.AnnounceSystemMessage( "{0} 操作超时", CodeName );
          commandText = new[] { "1", "2", "3", "4", "5" }.RandomItem();
          PlayerHost.WriteWarningMessage( "操作已超时，随机打出第 {0} 张牌", commandText );
        }


        try
        {
          command = ParseCommand( commandText );
        }
        catch ( FormatException )
        {
          PlayerHost.WriteMessage( "输入的命令格式错误" );
          continue;
        }


        if ( command != null )
          await command.Execute();


      } while ( false );

    }



    public SpecialEffectSlot SpecialEffect
    {
      get;
      private set;
    }


    public DefenceEffectSlot DefenceEffect
    {
      get;
      private set;
    }



    public override object GetGameInformation()
    {
      return new
      {
        Players = Game.Players.ToDictionary( item => item.CodeName, item => item.Health ),
        Cards = Cards,
      };
    }





    private PlayCommand ParseCommand( string commandText )
    {

      int cardIndex;

      if ( !int.TryParse( commandText, out cardIndex ) || cardIndex > 5 || cardIndex < 1 )
        throw new FormatException();


      return new PlayCommand( this, (SimpleGameCard) Cards[cardIndex - 1], Game.Players.Where( item => item != this ).ToArray().RandomItem() );
    }

    internal void RemoveCard( SimpleGameCard card )
    {
      CardCollection.Remove( card );
    }

    internal void RemoveAllCard()
    {
      CardCollection.RemoveAll( item => true );
    }


    /// <summary>
    /// 给玩家发牌
    /// </summary>
    public void DealCards()
    {
      CardCollection.AddRange( Game.CardDealer.DealCards( 5 - Cards.Length ) );
    }
  }
}
