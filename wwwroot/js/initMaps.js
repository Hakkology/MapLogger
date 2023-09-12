// Write your JavaScript code.

function initMaps(cesiumAccessToken){

    Cesium.Ion.defaultAccessToken = cesiumAccessToken;
    let isAnimating = false; // for animation controls, testing...
    let isInitializing = true; // to avoid excessive logging...

    // ol init
    var openlayersView = new ol.View({
        center: ol.proj.fromLonLat([32.8597, 39.9208]),
        zoom: 14
    });

    var olMap = new ol.Map({
        target: 'map-container',
        layers: [new ol.layer.Tile({source: new ol.source.OSM()})],
        view: openlayersView
    });

    // cesium init
    const viewer = new Cesium.Viewer('cesiumContainer', {
        terrainProvider: Cesium.createWorldTerrain(),
        animation: false,            
        baseLayerPicker: false,     
        fullscreenButton: false,    
        geocoder: false,            
        homeButton: false,         
        infoBox: false,             
        sceneModePicker: false,    
        selectionIndicator: false,  
        timeline: false,           
        navigationHelpButton: false, 
        navigationInstructionsInitiallyVisible: false, 
        scene3DOnly: true,          
        skyAtmosphere: false,    
        shouldAnimate: true,        
        pickTranslucentDepth: true,
    });    

    viewer.forceResize();
  
    // Initial position.
    viewer.camera.flyTo({
        destination: Cesium.Cartesian3.fromDegrees(32.8597, 39.9208, 1800),
        orientation: {
          heading: Cesium.Math.toRadians(0.0),
          pitch: Cesium.Math.toRadians(-90.0),
        },
        complete: function() {
          isInitializing = false;
        }
    });

    // Grab the coords for olmap and transfer to cesium viewer by left click.
    olMap.on('click', function(evt) {
        if (isInitializing) {
            return;
        }
        var coords = ol.proj.toLonLat(evt.coordinate);
        var terrainSamplePosition = Cesium.Cartographic.fromDegrees(coords[0], coords[1]);
        var terrainProvider = viewer.terrainProvider;

        console.log('Open Layer Map Clicked Coordinates:', coords[0], coords[1]);
        LogCoordstoServer("olmap", coords[0], coords[1]);

        Cesium.sampleTerrainMostDetailed(terrainProvider, [terrainSamplePosition]).then(function(samples) {
            var groundElevation = samples[0].height;
            var cameraElevation = groundElevation + 1000;
            //console.log('Sampled Ground Elevation:', groundElevation);

            isAnimating = true;
            openlayersView.animate({center: ol.proj.fromLonLat([coords[0], coords[1]]), duration: 1000}, function() {
                viewer.camera.flyTo({
                    destination: Cesium.Cartesian3.fromDegrees(coords[0], coords[1], cameraElevation),
                    complete: function() {
                        isAnimating = false;

                    }
                });
            });
        });
    });

    // Grab the coords for olmap and transfer to cesium viewer by drag move.
    olMap.on('moveend', function() {
        if (isInitializing) {
            return;
        }
        var coords = ol.proj.toLonLat(openlayersView.getCenter());
        var terrainSamplePosition = Cesium.Cartographic.fromDegrees(coords[0], coords[1]);

        console.log('Open Layer Map Clicked Coordinates:', coords[0], coords[1]);
        LogCoordstoServer("olmap", coords[0], coords[1]);
        var terrainProvider = viewer.terrainProvider;

        Cesium.sampleTerrainMostDetailed(terrainProvider, [terrainSamplePosition]).then(function(samples) {
            var groundElevation = samples[0].height;
            var cameraElevation = groundElevation + 1000;

            //console.log('Sampled Ground Elevation:', groundElevation);

            isAnimating = true;
            viewer.camera.flyTo({
                destination: Cesium.Cartesian3.fromDegrees(coords[0], coords[1], cameraElevation),
                complete: function() {
                    isAnimating = false;

                }
            });
        });
    });
    
    // Grab the coords for cesium viewer and transfer to olmap by drag move.
    viewer.camera.moveEnd.addEventListener(function() {
        if (isInitializing) {
            return;
        }
        if (!isAnimating) {
            const cameraPosition = viewer.camera.positionCartographic;
            const longitude = Cesium.Math.toDegrees(cameraPosition.longitude);
            const latitude = Cesium.Math.toDegrees(cameraPosition.latitude);

            var terrainSamplePosition = Cesium.Cartographic.fromDegrees(longitude, latitude);
            var terrainProvider = viewer.terrainProvider;

            console.log('Cesium View Moved to Coordinates:', longitude, latitude);
            LogCoordstoServer("cesium", longitude, latitude);
            
            Cesium.sampleTerrainMostDetailed(terrainProvider, [terrainSamplePosition]).then(function(samples) {
                const groundElevation = samples[0].height;
                const cameraElevation = groundElevation + 1000;
                //console.log('Sampled Ground Elevation:', groundElevation);
                viewer.camera.flyTo({
                    destination: Cesium.Cartesian3.fromDegrees(longitude, latitude, cameraElevation),
                    duration: 0  
                });

                openlayersView.animate({center: ol.proj.fromLonLat([longitude, latitude])});
            });
        }
    });




    // Grab the coords for cesium viewer and transfer to olmap by left click.
    var handler = new Cesium.ScreenSpaceEventHandler(viewer.scene.canvas);
    handler.setInputAction(function(movement) {
        if (isInitializing) {
            return;
        }
        if (!isAnimating) {
            var location = viewer.camera.getPickRay(movement.position);
            var cartesian = viewer.scene.globe.pick(location, viewer.scene);
            
            if (Cesium.defined(cartesian)) {
                var cartographic = Cesium.Cartographic.fromCartesian(cartesian);
                var longitude = Cesium.Math.toDegrees(cartographic.longitude);
                var latitude = Cesium.Math.toDegrees(cartographic.latitude);

                console.log('Cesium View Clicked Coordinates:', longitude, latitude);
                LogCoordstoServer("cesium", longitude, latitude);

                var terrainSamplePosition = Cesium.Cartographic.fromDegrees(longitude, latitude);
                var terrainProvider = viewer.terrainProvider;
                
                Cesium.sampleTerrainMostDetailed(terrainProvider, [terrainSamplePosition]).then(function(samples) {
                    const groundElevation = samples[0].height;
                    const cameraElevation = groundElevation + 1000; 
                    //console.log('Sampled Ground Elevation:', groundElevation);

                    isAnimating = true;
                    viewer.camera.flyTo({
                        destination: Cesium.Cartesian3.fromDegrees(longitude, latitude, cameraElevation),
                        complete: function() {

                            isAnimating = false;
                        }
                    });
                });
            }
        }
    }, Cesium.ScreenSpaceEventType.LEFT_CLICK);

}

function LogCoordstoServer(type, longitude, latitude) {
    $.ajax({
        url: '/Home/CoordinateLogger',
        type: 'POST',
        data: {
            type: type,
            longitude: longitude,
            latitude: latitude,
            
        },
        success: function(response) {
            console.log('Logged successfully!', response);
        },
        error: function(error) {
            console.log('Failed to log:', error);
        }
    });
}