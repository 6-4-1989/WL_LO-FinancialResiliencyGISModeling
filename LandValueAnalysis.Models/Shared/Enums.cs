using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandValueAnalysis.Models.Shared;

public enum DataView
{
    BuildingFootprint = 0,
    LandValuePerAcre = 1,
    NetInfrastructureDeficit = 2,
    LandUseClassification = 3,
    AverageBuildingFootprint = 4,
    AverageLandValuePerAcre = 5,
    LotScaleFootprints = 6,
    LotScale_LV_PerAcre = 7,
    Neighborhoods = 8
}
public enum MapMode
{
    TwoDimensional,
    ThreeDimensional
}
