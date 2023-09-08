// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function initMaps(cesiumAccessToken){

    Cesium.Ion.defaultAccessToken = cesiumAccessToken;

    var openlayersView = new ol.View({
        center: ol.proj.fromLonLat([32.8597, 39.9208]),
        zoom: 14
    });

    var olMap = new ol.Map({
        target: 'map-container',
        layers: [new ol.layer.Tile({source: new ol.source.OSM()})],
        view: openlayersView
    });

    viewer.forceResize();
  
    // Initial position.
    viewer.camera.flyTo({
      destination: Cesium.Cartesian3.fromDegrees(32.8597, 39.9208, 1800),
      orientation: {
        heading: Cesium.Math.toRadians(0.0),
        pitch: Cesium.Math.toRadians(-90.0),
      }
    });

    // Grab the coords for olmap and transfer to cesium viewer.
    olMap.on('click', function(evt) {
        var coords = ol.proj.toLonLat(evt.coordinate);
        viewer.camera.flyTo({
            destination: Cesium.Cartesian3.fromDegrees(coords[0], coords[1], 1800)
        });
    });
    
    //Grab the coords for cesium viewer and transfer to olmap.
    var handler = new Cesium.ScreenSpaceEventHandler(viewer.scene.canvas);
    handler.setInputAction(function(movement) {
        var pickedLocation = viewer.scene.pick(movement.position);
        if (!Cesium.defined(pickedLocation)) {
            var cartesian = viewer.scene.pickPosition(movement.position);
            if(Cesium.defined(cartesian)){
                var cartographic = Cesium.Cartographic.fromCartesian(cartesian);
                var longitude = Cesium.Math.toDegrees(cartographic.longitude);
                var latitude = Cesium.Math.toDegrees(cartographic.latitude);
                
                openlayersView.animate({center: ol.proj.fromLonLat([longitude, latitude])});
                viewer.camera.flyTo({
                    destination: Cesium.Cartesian3.fromDegrees(longitude, latitude, 1800)
                });
            }
        }
    }, Cesium.ScreenSpaceEventType.LEFT_CLICK);
    

    const viewer = new Cesium.Viewer('cesiumContainer', {
        terrainProvider: Cesium.createWorldTerrain(),
        animation: false,            // Disable animation widget
        baseLayerPicker: false,      // Disable the base layer picker widget
        fullscreenButton: false,     // Disable the full screen button
        geocoder: false,             // Disable the geocoder widget
        homeButton: false,           // Disable the home button
        infoBox: false,              // Disable the info box widget
        sceneModePicker: false,      // Disable the scene mode picker widget
        selectionIndicator: false,   // Disable selection indicator
        timeline: false,             // Disable timeline widget
        navigationHelpButton: false, // Disable navigation help button
        navigationInstructionsInitiallyVisible: false,  // Don't show navigation instructions
        scene3DOnly: true,           // Render only in 3D to optimize performance
        skyAtmosphere: false,        // Disable sky and atmosphere model
        shouldAnimate: true,         // Keep animation loop running for smooth experience
      });    


}