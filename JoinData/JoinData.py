import geopandas as gp
from numpy import float64, shape
import pandas as pd
from pathlib import Path
from decimal import *
import json

#gets the directory
dir = Path(__file__).resolve().parent

#join directory & file path together
geoJsonPath = dir / "WL_ClackamasData.ndgeojson"
shapefilePath = dir / "Building_Footprints_-2978958205140820763.zip"

#read line-delimited geojson into geoframe 
clackamasDataList = []

with open(geoJsonPath, encoding='utf-8-sig') as file:
    for line in file:
        dict = json.loads(line)
        clackamasDataList.append(dict)


clackamasGeoDataFrame = gp.GeoDataFrame.from_features(clackamasDataList)

#read shapefile into geoframe
shapefileGeoDataFrame = gp.read_file(shapefilePath, use_arrow=True)

'''
reproject since geopandas is funny with reference systems
also get rid of unneeded columns
'''
shapefileGeoDataFrame.drop(["ORBLD_ID","County","CONTRIBUTO","SOURCE","SOURCE_TYP",\
    "SOURCE_DAT","LAG","ROOF_MEAN","ROOF_MAX","YEAR_BUILT","SQ_FT","REVIEW_IMG",\
    "REVIEW_I_1", "REVIEW_DAT"], inplace=True, axis=1)
shapefileGeoDataFrame = shapefileGeoDataFrame.to_crs(epsg=2913)
clackamasGeoDataFrame = clackamasGeoDataFrame.set_crs(epsg=2913, allow_override=True)

print(clackamasGeoDataFrame.info())
print(shapefileGeoDataFrame.info())

#spatial join to keep only intersecting polygons
joinedData = clackamasGeoDataFrame.sjoin(shapefileGeoDataFrame)

#calculate intersection area of building and lot and divide by lot size to get building footprint
getcontext().prec = 4

joinedData["building footprint"] = joinedData.apply(
    lambda row: 
    Decimal(shapefileGeoDataFrame.loc[row["index_right"]].geometry
        .intersection(row.geometry).area)
        / Decimal(row.geometry.area)
    , axis=1
    )

#drop building geometry indexes since not needed anymore
joinedData.drop(["index_right"], inplace=True, axis=1)

#elimnate duplicate records from multiple buildings on one lot
aggregateFunctions = { 
    'building footprint': 'sum',
    'geometry': 'first',
    'land use': 'first',
    'lot size (acres)': 'first',
    'land value/acre ($)': 'first'
    }

joinedData = joinedData.groupby("address", as_index=False).agg(aggregateFunctions)

print(joinedData)

#reset to geodata frame
joinedData = gp.GeoDataFrame(joinedData, geometry='geometry', crs=clackamasGeoDataFrame.crs)

#change coordinate system to WGS84 for convenience
joinedData = joinedData.to_crs(epsg=4326)

joinedData.to_file("WestLinnLots.gpkg", layer="West Linn Lots", driver="GPKG")