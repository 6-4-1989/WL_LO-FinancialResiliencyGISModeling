using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandValueScraper.Models;
/*mimics deserialized json schema,
yes I didn't follow naming conventions deal with it */
public record DeserializedAddressGeoJsonDTO(
    string type,
    Properties properties,
    Geom geometry
);

public record Properties(
    string hash,
    string number,
    string street,
    string unit,
    string city,
    string district,
    string region,
    string postcode,
    string id
    );

public record Geom(
    string type,
    double[] coordinates
    );
