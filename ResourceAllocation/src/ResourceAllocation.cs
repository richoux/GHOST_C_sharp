using System;
using System.Collections.Generic;

namespace RA
{
  class MainClass
  {
    static List< Variable > MakeTerranUnits( ghost.Domain domain )
    {
      return new List< Variable > {
        new Variable( "Marine", domain, Race.Terran ),
        new Variable( "Firebat", domain, Race.Terran ),
        new Variable( "Ghost", domain, Race.Terran ),
        new Variable( "Vulture", domain, Race.Terran ),
        new Variable( "SiegeTank_tankMode", domain, Race.Terran ),
        new Variable( "SiegeTank_siegeMode", domain, Race.Terran ),
        new Variable( "Goliath", domain, Race.Terran ),
        new Variable( "Wraith", domain, Race.Terran ),
        new Variable( "Battlecruiser", domain, Race.Terran ),
        new Variable( "Valkyrie", domain, Race.Terran )
      };
    }
      
    public static void Main( string[] args )
    {
      int mineral, gas, supply;
      Data data = new Data();

      if( args.Length != 3 )
      {
        System.Console.WriteLine( "Usage: ResourceAllocation #mineral #gas #supply" );
        //return;
        mineral = 1000;
        gas = 700;
        supply = 19;
      }
      else
      {
        mineral = Convert.ToInt32( args[ 0 ] );
        gas = Convert.ToInt32( args[ 1 ] );
        supply = Convert.ToInt32( args[ 2 ] );
      }

      int maxSize = Math.Max( mineral / 25, Math.Max( gas / 25, supply ) );
      var domain = new ghost.Domain( maxSize, 0 );

      var listUnits = MakeTerranUnits( domain );
      var setUnits = new SetVariables( listUnits );

      var constraints = new List< Constraint > {
        new MineralConstraint( setUnits, mineral ),
        new GasConstraint( setUnits, gas ),
        new SupplyConstraint( setUnits, supply )
      };

      var objective = new MaxDPS( "max DPS", mineral, gas, supply );
      var solver = new ghost.Solver< Variable, SetVariables, Constraint >( setUnits, constraints, objective );

      Console.WriteLine( "Start solving trivial test" );
      solver.solve( 20, 150 );

      int mineralUsed = 0;
      int gasUsed = 0;
      double supplyUsed = 0.0;
      double DPS = 0.0;

      for( int i = 0 ; i < setUnits.GetNumberVariables() ; ++i )
      {
        mineralUsed += setUnits.GetValue( i ) * data.Dataset[ setUnits.Name( i ) ].CostMineral;
        gasUsed += setUnits.GetValue( i ) * data.Dataset[ setUnits.Name( i ) ].CostGas;
        supplyUsed += setUnits.GetValue( i ) * data.Dataset[ setUnits.Name( i ) ].CostSupply;
        if( data.Dataset[ setUnits.Name( i ) ].GroundAttack > 0 )
          DPS += setUnits.GetValue( i ) * ( (double)data.Dataset[ setUnits.Name( i ) ].GroundAttack / data.Dataset[ setUnits.Name( i ) ].GroundCooldown );
      }

      DPS *= 24;
      Console.WriteLine( "DPS = " + DPS );
      Console.WriteLine( "Mineral left: " + (mineral - mineralUsed) + ", gas left: " + (gas - gasUsed) + ", supply left: " + (supply - supplyUsed) );
    }
  }
}
