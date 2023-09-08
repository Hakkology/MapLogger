// Write your JavaScript code.

const terrainCache = new Map(); 

function getElevationWithOffset(longitude, latitude) {
    return new Promise((resolve, reject) => {
        const cacheKey = `${longitude.toFixed(4)},${latitude.toFixed(4)}`; 
        
        if (terrainCache.has(cacheKey)) {
            resolve(terrainCache.get(cacheKey) + 1000); 
            return;
        }
        
        var terrainProvider = Cesium.createWorldTerrain();
        var positions = [
            Cesium.Cartographic.fromDegrees(longitude, latitude)
        ];

        Cesium.sampleTerrainMostDetailed(terrainProvider, positions).then(function(updatedPositions) {
            terrainCache.set(cacheKey, updatedPositions[0].height);
            resolve(updatedPositions[0].height + 1000); 
        }).catch(reject);
    });
}