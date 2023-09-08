/*  Openlayers haritası ve Cesium 3B harita görüntüleyicisini birlikte gösteren bir proje yapmanızı istiyoruz, tek bir html sayfası. 
    Openlayers haritası ekranın solunda olacak ve Cesium görüntüleyicisi de sağda. Boydan %50-50 böl ekranı. 

-   Tamam.
    ---------------------

    Açılışta soldaki haritanın gösterdiği alan aynı zamanda Cesium görüntüleyicisinde de görünecek. 
    Her iki harita da başta açık olacak yani.

-   Tamam, ilk koordinat noktaları belirlendi, Kızılay'a kitlendi.
    ---------------------

    Cesium haritasında arazi modelinin de görüntülenmesini istiyoruz. 
    Bunun için yapman gereken iş basit, sadece bilmiyorsan bir küçük araştırma yapman lazım.

-   Tamam, cesium haritasında arazi modeli gösterildi.
    ---------------------

    Haritalar açılığında Ankara Kızılay’ı merkez alacak şekilde bir zoom seviyesinde (ve 3D harita için gerekli görüş açısında) gelsinler lütfen. 
    Bundan sonra, soldaki 2D haritaya her tıklayışta sağdaki Cesium haritası da tıklanılan yerin koordinatlarına animasyonla gitsin ve 
    bu coğrafi koordinatlar konsola loglansın. Bu sırada Cesium haritası hiç bir zaman yer altına inmemeli ve 
    soldaki haritada tıklanılan yer, sağdaki haritada viewpoint’in dışında kalmamalı.

-   OL ve cesium maplar eventlistenerlarla birbirlerini dinleyecek şekilde ayarlandı.
-   Sol tuşla tıklama çok iyi çalışmıyor ama kaydırarak gitme gayet iyi.
-   Yükseklik 1800 olarak ayarlandı. Bazı koşullarda yer altına inebilir, mevcut elevationın üstüne offset eklenebilir mi ?
    ---------------------

    Sağdaki (Cesium) haritasında herhangi bir yere tıkladığınızda ise bu sefer soldaki haritanın merkezi o noktaya kaydırılsın ve 
    bu coğrafi koordinatlar konsola loglansın.

-   Cesium haritasında herhangi bir yere tıklandığında veya taşındığında olmapsın merkezi o noktaya kaydırılıyor.
-   Animasyon süreleri yüzünden çift renderlara sorunu düzeltildi, isAnimating flagi eklendi.
-   Sol tuş tıklama çok iyi çalışmıyor. Bakılacak.
    ---------------------

    Her iki harita tıklamaları da bir yandan sunucu tarafında bir uygulamaya gönderilsin ve tek bir text dosyasına loglansın. 
    Bunu nasıl yapacağın önemli değil, sadece bizim nasıl çalıştıracağımızı tarif etmeni bekleriz. 
    Bu uygulamanın çok kullanıcılı olduğunu ve aynı anda çok fazla loglama isteğinin sunucuya geleceğini ve 
    her birinin text dosyaya loglanması gerektiğini varsayalım.
-   Ajax ve homecontrollera eklediğim post işlemi ile tüm veriler txt filelarda loglanıyor.

-   Animasyon süreleri yüzünden çift renderlara sorunu düzeltildi, isAnimating flagi eklendi.
-   Sol tuş tıklama çok iyi çalışmıyor. Bakılacak.
    ---------------------
*/

/*

cesium viewer boolean traits:

        animation:            // Animation widget
        baseLayerPicker:      // The base layer picker widget
        fullscreenButton:     // Full screen button
        geocoder:             // The geocoder widget
        homeButton:           // Home button
        infoBox:              // Info box widget
        sceneModePicker:      // The scene mode picker widget
        selectionIndicator:   // Selection indicator
        timeline:             // Timeline widget
        navigationHelpButton: // Navigation help button
        navigationInstructionsInitiallyVisible:  // Navigation instructions
        scene3DOnly:          // Render only in 3D to optimize performance
        skyAtmosphere:        // Sky and atmosphere model
        shouldAnimate:        // Keep animation loop running for smooth experience
        pickTranslucentDepth: 

*/
