# MapLogger

This is an application created in order to respond to a case.

HTML page is divided into two parts showing an OpenLayerMap and a Cesium 3B Map Display widget.
Openlayermap is on the left and Cesium 3B map is on the right.

Initial location is set to be Kizilay/Ankara.
OpenLayerMap is initialized focused on this position. A landscape model is shown on the cesium map, similarly focused on the same region.

Through event listeners, OpenLayerMap is connected to Cesium viewer for both left clicking and dragging the mouse.
Once a location is finalized, that coordinate is shown by animation. 
To disable to rerendering issue, a flag is also implemented.

Height map is currently set to 1.800 meters and im unable to request actual elevation levels for cesium map. 
This occasionally causes problems where the camera is under the terrain level. Will look into this later.

Both map clicks are also logged in a text file seperately made for each map, openlayermap and cesium viewer.
Everytime user clicks on a coordinate, this coordinate is displayed on the console log.
Additionally, timestamp, longitude and latitude information is registered on the text file.

A model is created for coordinates having Id, timestamp, longitude and latitude.
This model is reflected onto a postgresql database through efcore and code first approach with elephantsql.

If the timestamp of an entry is higher than the previously signed timestamp (basically highest timestamp value for any given moment), that entry is logged on the database once every 5 minutes.
By doing so, every log information is collected and saved within specific time intervals.
At the same time, we keep saving logs on the text file for both maps.
The purpose of this method is to implement a logging mechanism with the database and reduce the connection load on the db.
Cleaning the logger txt files for previously logged entries might be necessary in order to bring more performance to db logging operations.

