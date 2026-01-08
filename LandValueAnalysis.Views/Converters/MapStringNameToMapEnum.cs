using System.Globalization;
using System.Windows.Data;
using LandValueAnalysis.Services.Factories;
using LandValueAnalysis.Models.Shared;

namespace LandValueAnalysis.Views.Converters;

public sealed class MapStringNameToMapEnum : IValueConverter
{
    public static readonly Dictionary<string, DataView> _stringEnumRepresentations = new Dictionary<string, DataView>()
    {
        ["Building Footprint"] = DataView.BuildingFootprint,
        ["Land Value Per Acre ($)"] = DataView.LandValuePerAcre,
        ["Net Infrastrure Deficit ($)"] = DataView.NetInfrastructureDeficit,
        ["Normalized Land Use Classification"] = DataView.LandUseClassification,
        ["Average Building Footprint"] = DataView.AverageBuildingFootprint,
        ["Average Land Value Per Acre ($)"] = DataView.AverageLandValuePerAcre,
        ["Lots - Building Footprint"] = DataView.LotScaleFootprints,
        ["Lots - Land Value Per Acre ($)"] = DataView.LotScale_LV_PerAcre
    };
    public static readonly Dictionary<DataView, string> _enumStringRepresentations = new Dictionary<DataView, string>()
    {
        [DataView.BuildingFootprint] = "Building Footprint",
        [DataView.LandValuePerAcre] = "Land Value Per Acre ($)",
        [DataView.NetInfrastructureDeficit] = "Net Infrastrure Deficit ($)",
        [DataView.LandUseClassification] = "Normalized Land Use Classification",
        [DataView.AverageBuildingFootprint] = "Average Building Footprint",
        [DataView.AverageLandValuePerAcre] = "Average Land Value Per Acre ($)",
        [DataView.LotScaleFootprints] = "Lots - Building Footprint",
        [DataView.LotScale_LV_PerAcre] = "Lots - Land Value Per Acre ($)"
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        => _enumStringRepresentations[(DataView)value];

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => _stringEnumRepresentations[(string)value];
}
