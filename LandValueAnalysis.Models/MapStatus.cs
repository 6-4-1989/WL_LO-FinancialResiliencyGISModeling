using Esri.ArcGISRuntime.Mapping;
using LandValueAnalysis.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandValueAnalysis.Models;

public class MapStatus
{
    public MapMode CurrentMapMode { get; }
    public GeoModel CurrentMap { get; }

    public MapStatus(MapMode currentMapMode, GeoModel currentMap)
    {
        CurrentMapMode = currentMapMode;
        CurrentMap = currentMap;
    }
}
