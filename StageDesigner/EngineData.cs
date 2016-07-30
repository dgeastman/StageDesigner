using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StageDesigner
{
    public class EngineData
    {
        public string PartName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public float BurnTime { get; set; }
        public float FuelVolume { get; set; }
        public float FuelMass { get; set; }
        public float TankMass { get; set; }
        public float EngineMass { get; set; }
        public bool isRcs { get; set; }
    }
}
