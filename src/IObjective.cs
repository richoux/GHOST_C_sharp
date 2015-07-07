using System;
using System.Collections.Generic;

namespace ghost
{
  public interface IObjective<out TypeSetVariables, out TypeVariable> where TypeSetVariables : ISetVariables<TypeVariable> where TypeVariable : IVariable
  {
    double Cost( TypeSetVariables variables );
    int HeuristicVariable( List<int> indexes, TypeSetVariables variables ); 
    int HeuristicValue( List<double> globalCosts, ref double bestEstimatedCost, ref int bestValue );
    double PostprocessSatisfaction( TypeSetVariables variables,
                                    ref double bestCost,
                                    List<int> solution,
                                    double satTimeout );
    double PostprocessOptimization( TypeSetVariables variables,
                                    ref double bestCost,
                                    double optTimeout );
    Random Random { get; set; }
  }
}

