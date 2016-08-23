using System;
using System.Collections.Generic;
using System.Linq;

namespace RA
{
  public abstract class Constraint : ghost.Constraint<SetVariables, Variable>
  {
    protected Constraint( SetVariables variables, double resource ) 
        : base( variables ) 
    { 
      Resource = resource;
      Data = new Data();
    }

    protected double Cost( string resourceType )
    {
      var costs = new double[ Variables.GetNumberVariables() ];
      for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
      {
        // optimizable with reflexion to call the switch just once and fix the resource we are interested by?
        switch( resourceType )
        { 
          case "mineral":
            costs[ i ] = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostMineral;
            break;
          case "gas":
            costs[ i ] = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostGas;
            break;
          case "supply":
            costs[ i ] = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostSupply;
            break;
        }
      }

      double total = costs.Sum();

      if( total <= Resource )
        return 0;
      else
      {
//        double surplus = total - Resource;
//        for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
//          variableCost[ i ] += costs[ i ] >= surplus ? costs[ i ] : 0;
      }

      return total;
    }

    public void UpdateProjectedCost( double cost, string resourceType )
    {
      double surplus = cost - Resource;
      double resourceCost = 0.0;

      for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
      {
        // optimizable with reflexion to call the switch just once and fix the resource we are interested by?
        switch( resourceType )
        { 
        case "mineral":
          resourceCost = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostMineral;
          break;
        case "gas":
          resourceCost = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostGas;
          break;
        case "supply":
          resourceCost = Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostSupply;
          break;
        }
        if( resourceCost >= surplus ) 
          Variables.AddProjectedCost( i, resourceCost );
      }                                   
    }

    protected Dictionary<int, double> SimulateCost( int currentVariableIndex,                                                    
                                                    string resourceType )
    {
      // for each value in currentVariableIndex's domain, save the constraint cost value.
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
        Variables.SetValue( currentVariableIndex, pos );
        simCosts[ pos ] = Cost( resourceType );
      }

      Variables.SetValue( currentVariableIndex, backup );
      return simCosts;
    }


    public Data Data { get; private set;}
    public double Resource { get; private set; }
  }


  /***********/
  /* Mineral */
  /***********/  
  public class MineralConstraint : Constraint
  {
    public MineralConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost()
    {
      return Cost( "mineral" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      return SimulateCost( currentVariableIndex, "mineral" );
    }

    override public void UpdateProjectedCost( double cost )
    {
      UpdateProjectedCost( cost, "mineral" );
    }
  }


  /*******/
  /* Gas */
  /*******/  
  public class GasConstraint : Constraint
  {
    public GasConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost()
    {
      return Cost( "gas" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      return SimulateCost( currentVariableIndex, "gas" );
    }

    override public void UpdateProjectedCost( double cost )
    {
      UpdateProjectedCost( cost, "gas" );
    }
  }


  /**********/
  /* Supply */
  /**********/  
  public class SupplyConstraint : Constraint
  {
    public SupplyConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost()
    {
      return Cost( "supply" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      return SimulateCost( currentVariableIndex, "supply" );
    }

    override public void UpdateProjectedCost( double cost )
    {
      UpdateProjectedCost( cost, "supply" );
    }
  }

}

