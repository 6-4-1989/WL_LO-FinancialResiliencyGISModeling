using LandValueScraper.Models;
using Newtonsoft.Json.Linq;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace LandValueScraper.Services;

public sealed class ScrapeLandValues
{
    private enum Endpoints
    {
        assessment,
        taxlot
    }
    private const string _baseUri = "https://maps.clackamas.us/jericho/data/select/";
    private const string _userAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.1058";
    private const string _epsg4326Wkt = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
    private const string _epsg2913Wkt = "PROJCS[\"NAD_1983_HARN_StatePlane_Oregon_North_FIPS_3601_Feet_Intl\",GEOGCS[\"GCS_North_American_1983_HARN\",DATUM[\"D_North_American_1983_HARN\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",8202099.738],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-120.5],PARAMETER[\"Standard_Parallel_1\",46.0],PARAMETER[\"Standard_Parallel_2\",44.3333333333333],PARAMETER[\"Latitude_Of_Origin\",43.6666666666667],UNIT[\"foot\",0.3048]]";

    //Caching coord transforms to prevent memory leak
    private static readonly Lazy<ICoordinateTransformation> _coordinateTransform = new Lazy<ICoordinateTransformation>(() =>
    {
        CoordinateSystemFactory systemFactory = new CoordinateSystemFactory();
        CoordinateTransformationFactory transformationFactory = new CoordinateTransformationFactory();

        CoordinateSystem initialCoordinateSystem = systemFactory.CreateFromWkt(_epsg4326Wkt);
        CoordinateSystem finalCoordinateSystem = systemFactory.CreateFromWkt(_epsg2913Wkt);

        return transformationFactory.CreateFromCoordinateSystems(initialCoordinateSystem, finalCoordinateSystem);
    });

    private readonly HttpClient _httpClient;

    public ScrapeLandValues(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string[]?> ScrapeObject(DeserializedAddressGeoJsonDTO addressDTOs, string city)
    {
        string convertedWktCoordString = ConvertCoordsToEPSG2913WktString(addressDTOs.geometry.coordinates);
        List<KeyValuePair<string, string>> coordHeader = CreateHeader("geometry", convertedWktCoordString);

        string taxlotJson = await GetAddressDataAsync(Endpoints.taxlot, coordHeader);

        if (IsWithinBoundsAndValid(taxlotJson, city))
        {
            string assessmentJson = await GetAddressDataAsync(Endpoints.assessment, coordHeader);
            return new string[] { taxlotJson, assessmentJson };
        }
        return null;
    }


    private string ConvertCoordsToEPSG2913WktString(double[] coordPair)
    {
        double[] finalCoords = _coordinateTransform.Value.MathTransform.Transform(coordPair);
        return $"POINT({finalCoords[0]} {finalCoords[1]})";
    }

    private List<KeyValuePair<string, string>> CreateHeader(string key, string value) => 
        new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(key, value) };
    
    private async Task<string> GetAddressDataAsync(Endpoints endpoint, List<KeyValuePair<string,string>> coordHeader)
    {
        using FormUrlEncodedContent encodedCoordHeader = new FormUrlEncodedContent(coordHeader);

        using HttpRequestMessage requestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_baseUri + endpoint.ToString()),
            Content = encodedCoordHeader
        };
        requestMessage.Headers.Add("User-Agent", _userAgent);

        using HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(requestMessage);
        string jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();
        return jsonResponse;
    }

    private bool IsWithinBoundsAndValid(string taxlotJson, string city) =>
        taxlotJson.Contains($"\"jurisdiction\":\"{city}\"", StringComparison.OrdinalIgnoreCase) &&
        (taxlotJson.Contains("\"landclass\":\"101\"") ||
        taxlotJson.Contains("\"landclass\":\"102\"") ||
        taxlotJson.Contains("\"landclass\":\"201\"") ||
        taxlotJson.Contains("\"landclass\":\"202\"") ||
        taxlotJson.Contains("\"landclass\":\"701\""));
}