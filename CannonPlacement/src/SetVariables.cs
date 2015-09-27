using System.Collections.Generic;

namespace CannonPlacement
{
  public class SetVariables : ghost.SetVariables<Variable>
  {
    public SetVariables( List<Variable> variables, int width, int height )
      : base( variables )
    {
      Grid = new int[width, height];
    }

    public int[,] Grid { get; set; } 
  }
}

