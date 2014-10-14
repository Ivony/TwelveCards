﻿using Ivony.TableGame.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.SimpleGames
{
  public class SimpleGame : BasicGame<SimpleGamePlayer, SimpleGameCard>
  {



    private string[] names = new[] { "张三", "李四", "王五" };


    public SimpleGame( string name ) : base( name ) { }


    protected override CardDealer CreateCardDealer()
    {
      var dealer = new UnlimitedCardDealer();
      dealer.RegisterCard( () => new AngelCard(), 3 );
      dealer.RegisterCard( () => new DevilCard(), 25 );
      dealer.RegisterCard( () => new CleanCard(), 30 );
      dealer.RegisterCard( () => new ShieldCard(), 20 );
      dealer.RegisterCard( () => new PeepCard(), 15 );
      dealer.RegisterCard( () => new AttackCard( 1 ), 50 );
      dealer.RegisterCard( () => new AttackCard( 2 ), 80 );
      dealer.RegisterCard( () => new AttackCard( 3 ), 50 );
      dealer.RegisterCard( () => new AttackCard( 4 ), 20 );
      dealer.RegisterCard( () => new AttackCard( 5 ), 10 );
      dealer.RegisterCard( () => new AttackCard( 6 ), 7 );
      dealer.RegisterCard( () => new AttackCard( 7 ), 5 );
      dealer.RegisterCard( () => new AttackCard( 8 ), 3 );
      dealer.RegisterCard( () => new AttackCard( 9 ), 2 );
      dealer.RegisterCard( () => new AttackCard( 10 ), 1 );

      return dealer;
    }


    protected override GamePlayer TryJoinGameCore( IGameHost gameHost, IPlayerHost playerHost )
    {
      lock ( SyncRoot )
      {
        if ( PlayerCollection.Count == 3 )
          return null;


        var name = names[PlayerCollection.Count];

        var player = new SimpleGamePlayer( name, gameHost, playerHost );

        PlayerCollection.Add( player );

        if ( PlayerCollection.Count == 3 )
          gameHost.Run();

        return player;

      }
    }
  }
}
