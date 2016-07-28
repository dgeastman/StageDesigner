using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageDesigner
{
    public class EngineData
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int BurnTime { get; set; }
        public float FuelVolume { get; set; }
        public float FuelMass { get; set; }
        public float TankMass { get; set; }
        public float EngineMass { get; set; }
    }
}
