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

    var cesiumViewer = new Cesium.Viewer('cesiumContainer', {
        imageryProvider: new Cesium.IonImageryProvider({ assetId: 3812 }),
        terrainProvider: Cesium.createWorldTerrain(),
        animation: false,
        baseLayerPicker: false,
        geocoder: false,
        homeButton: false,
        sceneModePicker: false,
        timeline: false,
        navigationHelpButton: false,
        vrButton: false,
        initialViewRectangle: Cesium.Rectangle.fromDegrees(32.8597 - 0.1, 39.9208 - 0.1, 32.8597 + 0.1, 39.9208 + 0.1)
    });

    olMap.on('click', function(evt) {
        var coords = ol.proj.toLonLat(evt.coordinate);
        cesiumViewer.camera.flyTo({destination: Cesium.Cartesian3.fromDegrees(coords[0], coords[1], 1500)});
        console.log(coords);
        // Burada sunucuya istek atıp koordinatları loglayabilirsiniz
    });

    var handler = new Cesium.ScreenSpaceEventHandler(cesiumViewer.scene.canvas);
    handler.setInputAction(function(movement) {
        var pickedFeature = cesiumViewer.scene.pick(movement.position);
        if (pickedFeature) {
            var cartesian = cesiumViewer.scene.pickPosition(movement.position);
            var cartographic = Cesium.Cartographic.fromCartesian(cartesian);
            var longitude = Cesium.Math.toDegrees(cartographic.longitude);
            var latitude = Cesium.Math.toDegrees(cartographic.latitude);
            
            openlayersView.animate({center: ol.proj.fromLonLat([longitude, latitude])});
            console.log([longitude, latitude]);
            // Rest of your code...
        }
    }, Cesium.ScreenSpaceEventType.LEFT_CLICK);
    
}