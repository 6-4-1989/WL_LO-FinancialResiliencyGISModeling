using LandValueScraper.Models;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace LandValueScraper.Services;

public sealed class DeserializeAddressGeoJson
{
    private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources\\AddressList.txt");

    private readonly IConfiguration _configuration;
    private readonly IPreparedGeometry _bbox;

    public DeserializeAddressGeoJson(IConfiguration configuration)
    {
        _configuration = configuration;

        _bbox = GetBbox();
    }

    private IPreparedGeometry GetBbox()
    {
        string bboxWkt = _configuration["bboxWkt"] ?? throw new Exception("\'bboxWkt\' does not exist in appsettings.json.");
        WKTReader reader = new WKTReader();
        Geometry polygon = reader.Read(bboxWkt);

        return (polygon is Polygon) ? PreparedGeometryFactory.Prepare(polygon) : throw new Exception("bbox must be polygon!");
    }

    //deserializes the address data for a selected city
    public List<DeserializedAddressGeoJsonDTO> Deserialize(string city)
    {
        List<DeserializedAddressGeoJsonDTO> deserializedGeoJson = new List<DeserializedAddressGeoJsonDTO>();

        //parse each object individually because geojson schemas are like that :middlefinger:
        using (StreamReader streamReader = new StreamReader(_filePath))
        {
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();

                if (line.Contains($"\"city\": \"{city}\"", StringComparison.OrdinalIgnoreCase)
                    || line.Contains($"\"city\": \"ADDRESS\""))
                {
                    DeserializedAddressGeoJsonDTO deserializedGeoJsonDTO = JsonConvert.DeserializeObject<DeserializedAddressGeoJsonDTO>(line);

                    if (IsWithinBounds(deserializedGeoJsonDTO.geometry.coordinates))
                    {
                        deserializedGeoJson.Add(deserializedGeoJsonDTO);
                    }
                }
            }
        }
        return deserializedGeoJson;
    }
    private bool IsWithinBounds(double[] coords)
    {
        Point point = new Point(new Coordinate(coords[0], coords[1]));
        return _bbox.Contains(point);
    }
}


