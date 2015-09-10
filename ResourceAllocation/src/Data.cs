using System.Collections.Generic;

namespace RA
{
  public class Unit
  {
    public Unit(int costMineral, 
                int costGas,
                double costSupply,
                int groundAttack,
                int airAttack,
                int groundCooldown,
                int airCooldown,
                int hitPoints,
                int armor,
                int shield,
                int range,
                int sight)
    {
      CostMineral = costMineral;
      CostGas = costGas;
      CostSupply = costSupply;
      GroundAttack = groundAttack;
      AirAttack = airAttack;
      GroundCooldown = groundCooldown;
      AirCooldown = airCooldown;
      HitPoints = hitPoints;
      Armor = armor;
      Shield = shield;
      Range = range;
      Sight = sight;
    }

    public int CostMineral { get; set; }
    public int CostGas { get; set; }
    public double CostSupply { get; set; }
    public int GroundAttack { get; set; }
    public int AirAttack { get; set; }
    public int GroundCooldown { get; set; }
    public int AirCooldown { get; set; }
    public int HitPoints { get; set; }
    public int Armor { get; set; }
    public int Shield { get; set; }
    public int Range { get; set; }
    public int Sight { get; set; }
  }

  public class Data
  {
    public Data()
    {
      Dataset = new Dictionary<string, Unit>();

      // Protoss
      Dataset.Add( "Zealot", new Unit(100, 0, 2, 16, 0, 22, 0, 100, 1, 60, 1, 7 ) );
      Dataset.Add( "Dragoon", new Unit(125, 50, 2, 20, 20, 30, 30, 100, 1, 80, 4, 8) );
      Dataset.Add( "DarkTemplar", new Unit(125, 100, 2, 40, 0, 30, 0, 80, 1, 40, 1, 7) );
      Dataset.Add( "Reaver", new Unit(200, 100, 4, 100, 0, 60, 0, 100, 0, 80, 8, 10) );
      Dataset.Add( "Archon", new Unit(100, 300, 4, 30, 30, 20, 20, 10, 0, 350, 2, 8) );
      Dataset.Add( "Scout", new Unit(275, 125, 3, 8, 28, 30, 22, 150, 0, 100, 4, 8) );
      Dataset.Add( "Corsair", new Unit(150, 100, 2, 0, 5, 0, 8, 100, 1, 80, 5, 9) );

      // Terran
      Dataset.Add( "Marine", new Unit(50, 0, 1, 6, 6, 15, 15, 40, 0, 0, 4, 7) );
      Dataset.Add( "Firebat", new Unit(50, 25, 1, 16, 0, 22, 0, 50, 1, 0, 2, 7) );
      Dataset.Add( "Ghost", new Unit(25, 75, 1, 10, 10, 22, 22, 45, 0, 0, 7, 9) );
      Dataset.Add( "Vulture", new Unit(75, 0, 2, 20, 0, 30, 0, 80, 0, 0, 5, 8) );
      Dataset.Add( "SiegeTank_tankMode", new Unit(150, 100, 2, 30, 0, 37, 0, 150, 1, 0, 7, 10) );
      Dataset.Add( "SiegeTank_siegeMode", new Unit(150, 100, 2, 70, 0, 75, 0, 150, 1, 0, 12, 10) );
      Dataset.Add( "Goliath", new Unit(100, 50, 2, 12, 20, 22, 22, 125, 1, 0, 5, 8) );
      Dataset.Add( "Wraith", new Unit(150, 100, 2, 8, 20, 30, 22, 120, 0, 0, 5, 7) );
      Dataset.Add( "Battlecruiser", new Unit(400, 300, 6, 25, 25, 30, 30, 500, 3, 0, 6, 11) );
      Dataset.Add( "Valkyrie", new Unit(250, 125, 3, 0, 6, 0, 64, 200, 2, 0, 6, 8) );

      // Zerg
      Dataset.Add( "Zergling", new Unit(25, 0, 0.5, 5, 0, 8, 0, 35, 0, 0, 1, 5) );
      Dataset.Add( "Hydralisk", new Unit(75, 25, 1, 10, 10, 15, 15, 80, 0, 0, 4, 6) );
      Dataset.Add( "Lurker", new Unit(125, 125, 2, 20, 0, 37, 0, 125, 1, 0, 6, 8) );
      Dataset.Add( "Ultralisk", new Unit(200, 200, 4, 20, 0, 15, 0, 400, 1, 0, 1, 7) );
      Dataset.Add( "Mutalisk", new Unit(100, 100, 2, 9, 9, 30, 30, 120, 0, 0, 3, 7) );
      Dataset.Add( "Guardian", new Unit(150, 200, 2, 20, 0, 30, 0, 150, 2, 0, 8, 11) );
      Dataset.Add( "Devourer", new Unit(250, 150, 2, 0, 25, 0, 100, 250, 2, 0, 6, 10) );
    }

    public Dictionary<string, Unit> Dataset { get; private set; }
  }
}

