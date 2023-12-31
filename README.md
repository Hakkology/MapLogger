# MapLogger

Maplogger page is divided into two parts showing an OpenLayerMap and a Cesium 3B Map Display widget.
Openlayermap is on the left and Cesium 3B map is on the right.

Initial location is set to be Kizilay/Ankara.
OpenLayerMap is initialized focused on this position. A landscape model is shown on the cesium map, similarly focused on the same region.

Through event listeners, OpenLayerMap is connected to Cesium viewer for both left clicking and dragging the mouse.
The same connection is also introduced to Cesium Viewer, any left click or dragging operation will also be listened by the openlayermap.
Once a location is finalized, that coordinate is shown by animation. To disable to rerendering issue, a flag is also implemented.

Height map is currently set to 1.800 meters and im unable to request actual elevation levels for cesium map. 
This occasionally causes problems where the camera is under the terrain level. Will look into this later.

- This problem is solved by addition of terrain data to javascript side. The camera is automatically set to 1000 meters above the terrain of the requested coordinate.

Both map clicks are also logged in a text file seperately made for each map, openlayermap and cesium viewer.
Everytime user clicks on a coordinate, this coordinate is displayed on the console log.
Additionally, timestamp, longitude and latitude information is logged on the text file for that type of map.

A model is created for coordinates having Id, timestamp, longitude and latitude.
This model is reflected onto a postgresql database instance made with elephantsql, through efcore and code first approach.

If the timestamp of an entry is higher than the previously signed timestamp (basically highest timestamp value for that moment), that entry is logged on the database once every 5 minutes.
By doing so, every log information is collected and saved within specific time intervals. At the same time, we keep saving logs on the text file for both maps.

- This part is now obsolete, data is being saved to db simultaneously with clicked coordinates and logged to txt file.

The purpose of this method is to implement a logging mechanism with the database and reduce the connection load on the db. 
Such a feature would be necessary in a case where many users shall be utilizing this program at the same time, if need be.

-----------------------------------------

Made adjustments to logging model for db saves. Similarly, logging mechanism is squeezed onto one txt file.
Utilized RabbitMQ for cases where there are too many Users to deal with. With rabbitmq, it is possible to find solutions to problems and spot if there is any congestion or overloading due to requests.
Coordinates are now saved on db in an asynchronous manner. Logs are only written if db save was successful.
Divided services for logging, db add and rabbitmq add/consume utilities.

