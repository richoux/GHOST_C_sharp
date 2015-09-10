using System;

namespace RA
{
  public enum Race { Terran, Protoss, Zerg };

  public class Variable : ghost.Variable, ICloneable
  {
    public Variable( string name, ghost.Domain domain, Race race, int value = 0 ) 
        : base( name, name, domain, value ) 
    {
      Race = race;
    }

    #region ICloneable implementation
    public object Clone()
    {
      return new Variable( Name, (ghost.Domain)Domain.Clone(), Race, GetValue() );
    }
    #endregion

    public string RaceString()  
    { 
      return Race.ToString();
    }

    public Race Race { get; private set; }
  }
}

