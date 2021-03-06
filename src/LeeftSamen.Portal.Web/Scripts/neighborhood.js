﻿(function($) {
    $.fn.neighborhood = function() {
        $(this).each(function() {
            var control = $(this),
                postalCodeField = control.find(".postalcode"),
                houseNumberField = control.find(".housenumber"),
                streetField = control.find(".street"),
                cityField = control.find(".city"),
                radiusSlider = control.find(".radius-slider"),
                radiusLabel = control.find(".radius-label"),
                mapControl = control.find(".map"),
                radiusField = control.find(".radius-value"),
                latitudeField = control.find(".latitude"),
                longitudeField = control.find(".longitude"),
                notFoundAlertElement = control.find("#NotFoundAlert"),
                loaderImage = control.find("#Loader");

            var dotSize = 0.04;
            var map = {
                map: null,
                circle: null,
                dot: null
            };

            setupRadiusSlider();
            setupAddressChangeHandler();
            setupMap();
            updateMap();

            function setupRadiusSlider() {
                radiusSlider.slider({
                    "min": radiusSlider.data('min'),
                    "max": radiusSlider.data('max'),
                    "step": radiusSlider.data('step'),
                    value: radiusField.val(),
                    slide: function(event, ui) {
                        radiusLabel.html(ui.value);
                        radiusField.val(ui.value);
                        updateMap();
                    }
                });

                radiusLabel.html(radiusField.val());
            }

            function setupAddressChangeHandler() {
                var addressTypingDelayTimer = null;

                postalCodeField.add(houseNumberField).keyup(function() {
                    if (addressTypingDelayTimer != undefined) {
                        window.clearTimeout(addressTypingDelayTimer);
                        addressTypingDelayTimer = null;
                    }
                    addressTypingDelayTimer = window.setTimeout(updateAddress, 800);
                });
                postalCodeField.add(houseNumberField).blur(function () {
                    if (addressTypingDelayTimer != undefined) {
                        window.clearTimeout(addressTypingDelayTimer);
                        addressTypingDelayTimer = null;
                    }
                    addressTypingDelayTimer = window.setTimeout(updateAddress, 800);
                });
            }

            function setupMap() {
                var defaultLatLng = [52.2129919, 5.2793703];
                map.map = L.map(mapControl.get()[0], {
                    zoomControl: false
                }).setView(defaultLatLng, 7);

                // Disable drag and zoom handlers.
                if (map.map.dragging) map.map.dragging.disable();
                if (map.map.touchZoom) map.map.touchZoom.disable();
                if (map.map.doubleClickZoom) map.map.doubleClickZoom.disable();
                if (map.map.scrollWheelZoom) map.map.scrollWheelZoom.disable();
                if (map.map.keyboard) map.map.keyboard.disable();
                if (map.map.boxZoom) map.map.boxZoom.disable();
                if (map.map.tap) map.map.tap.disable();

                L.tileLayer('/ResourceProxy/Proxy.ashx?http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                }).addTo(map.map);

                map.circle = L.circle(defaultLatLng, radiusField.val(), {
                    color: '#2ec5c2',
                    opacity: 0,
                    weight: 4,
                    dashArray: '5,10',
                    fillColor: '#2ec5c2',
                    fillOpacity: 0
                }).addTo(map.map);

                map.dot = L.circle(defaultLatLng, radiusField.val() * dotSize, {
                    color: '#2ec5c2',
                    opacity: 0,
                    weight: 0,
                    fillColor: '#2ec5c2',
                    fillOpacity: 0
                }).addTo(map.map);
            }

            function getLatLng() {
                var latitude = parseFloat(latitudeField.val());
                var longitude = parseFloat(longitudeField.val());
                if (isNaN(latitude) || isNaN(longitude) || !latitude || !longitude) {
                    return null;
                }

                return [latitude, longitude];
            }

            function updateMap() {
                var latlng = getLatLng(),
                    radius = radiusField.val();

                if (latlng === null) {
                    return;
                }

                map.circle.setRadius(radiusField.val());
                map.circle.setLatLng(latlng);
                map.circle.setStyle({ opacity: 1, fillOpacity: 0.2 });
                map.dot.setRadius(radius * dotSize);
                map.dot.setLatLng(latlng);
                map.dot.setStyle({ opacity: 0, fillOpacity: 1 });

                // Add slight delay to allow getBounds to catch up
                window.setTimeout(function() {
                    map.map.fitBounds(map.circle.getBounds());
                }, 500);
            }

            function updateAddress() {
                var postalCode = postalCodeField.val(),
                    houseNumber = houseNumberField.val();

                if (postalCode.length < 6 || houseNumber == '') {
                    return;
                }

                loaderImage.removeClass('hidden');
                streetField.val("");
                cityField.val("");
                latitudeField.val("");
                longitudeField.val("");

                postalCodeCheck(postalCode, houseNumber).done(function (data) {
                    loaderImage.addClass('hidden');
                    if (typeof data.results != 'undefined'
                        && data.results.length > 0
                        && typeof data.results[0].lng != 'undefined'
                        && typeof data.results[0].lat != 'undefined') {

                        streetField.val(data.results[0].street);
                        cityField.val(data.results[0].city);
                        latitudeField.val(data.results[0].lat);
                        longitudeField.val(data.results[0].lng);
                        notFoundAlertElement.addClass('hidden');

                        updateMap();
                    } else {
                        notFoundAlertElement.removeClass('hidden');
                    }
                });
            }
        });
    };

    $.fn.neighborhoodRadius = function () {
        $(this).each(function () {
            var control = $(this),
                radiusSlider = control.find("#radius-slider"),
                radiusMenu = control.find("#radius-menu"),
                radiusLabel = control.find("#radius-label"),
                mapControl = control.find("#map"),
                radiusField = control.find("#radius-value"),
                latitudeField = control.find("#Latitude"),
                longitudeField = control.find("#Longitude");

            var dotSize = 0.04;
            var map = {
                map: null,
                circle: null,
                dot: null
            };

            setupRadiusSlider();
            setupMap();
            updateMap();
            updateRadiusMenu();
            setupRadiusMenu();

            function setupRadiusSlider() {
                var slideDelayTimer = null;

                radiusSlider.slider({
                    "min": radiusSlider.data('min'),
                    "max": radiusSlider.data('max'),
                    "step": radiusSlider.data('step'),
                    value: radiusField.val(),
                    slide: function (event, ui) {
                        radiusLabel.html(ui.value);
                        radiusField.val(ui.value);
                        updateMap();
                        if (slideDelayTimer != undefined) {
                            window.clearTimeout(slideDelayTimer);
                            slideDelayTimer = null;
                        }
                        slideDelayTimer = window.setTimeout(updateRadiusMenu, 1000);
                    },
                    change: function (event, ui) {
                        radiusLabel.html(ui.value);
                        radiusField.val(ui.value);
                        updateMap();
                    }
                });

                radiusLabel.html(radiusField.val());
            }

            function setupRadiusMenu() {
                radiusMenu.change(function() {
                    radiusSlider.slider('value', this.value);
                });
            }

            function updateRadiusMenu() {
                $('option', radiusMenu).each(function (i) {
                    if (parseInt(this.value) >= parseInt(radiusField.val())) {
                        radiusMenu.val(this.value);
                        return false;
                    }
                });
            }

            function setupMap() {
                var defaultLatLng = [52.2129919, 5.2793703];
                map.map = L.map(mapControl.get()[0], {
                    zoomControl: false
                }).setView(defaultLatLng, 7);

                // Disable drag and zoom handlers.
                if (map.map.dragging) map.map.dragging.disable();
                if (map.map.touchZoom) map.map.touchZoom.disable();
                if (map.map.doubleClickZoom) map.map.doubleClickZoom.disable();
                if (map.map.scrollWheelZoom) map.map.scrollWheelZoom.disable();
                if (map.map.keyboard) map.map.keyboard.disable();
                if (map.map.boxZoom) map.map.boxZoom.disable();
                if (map.map.tap) map.map.tap.disable();

                L.tileLayer('/ResourceProxy/Proxy.ashx?http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
                }).addTo(map.map);

                map.circle = L.circle(defaultLatLng, radiusField.val(), {
                    color: '#2ec5c2',
                    opacity: 0,
                    weight: 4,
                    dashArray: '5,10',
                    fillColor: '#2ec5c2',
                    fillOpacity: 0
                }).addTo(map.map);

                map.dot = L.circle(defaultLatLng, radiusField.val() * dotSize, {
                    color: '#2ec5c2',
                    opacity: 0,
                    weight: 0,
                    fillColor: '#2ec5c2',
                    fillOpacity: 0
                }).addTo(map.map);
            }

            function getLatLng() {
                var latitude = parseFloat(latitudeField.val());
                var longitude = parseFloat(longitudeField.val());
                if (isNaN(latitude) || isNaN(longitude) || !latitude || !longitude) {
                    return null;
                }

                return [latitude, longitude];
            }

            function updateMap() {
                var latlng = getLatLng(),
                    radius = radiusField.val();

                if (latlng === null) {
                    return;
                }

                map.circle.setRadius(radiusField.val());
                map.circle.setLatLng(latlng);
                map.circle.setStyle({ opacity: 1, fillOpacity: 0.2 });
                map.dot.setRadius(radius * dotSize);
                map.dot.setLatLng(latlng);
                map.dot.setStyle({ opacity: 0, fillOpacity: 1 });

                // Add slight delay to allow getBounds to catch up
                window.setTimeout(function () {
                    map.map.fitBounds(map.circle.getBounds());
                }, 500);
            }

        });
    };

    $.fn.address = function () {
        $(this).each(function () {
            var control = $(this),
                postalCodeField = control.find("#PostalCode"),
                houseNumberField = control.find("#HouseNumber"),
                streetField = control.find("#Street"),
                cityField = control.find("#City"),
                latitudeField = control.find("#Latitude"),
                longitudeField = control.find("#Longitude"),
                notFoundAlertElement = control.find('#NotFoundAlert'),
                loaderImage = control.find('#Loader');

            setupAddressChangeHandler();

            function setupAddressChangeHandler() {
                var addressTypingDelayTimer = null;

                postalCodeField.add(houseNumberField).keyup(function () {
                    if (addressTypingDelayTimer != undefined) {
                        window.clearTimeout(addressTypingDelayTimer);
                        addressTypingDelayTimer = null;
                    }
                    addressTypingDelayTimer = window.setTimeout(updateAddress, 800);
                });
                postalCodeField.add(houseNumberField).blur(function () {
                    if (addressTypingDelayTimer != undefined) {
                        window.clearTimeout(addressTypingDelayTimer);
                        addressTypingDelayTimer = null;
                    }
                    addressTypingDelayTimer = window.setTimeout(updateAddress, 800);
                });
            }

            function updateAddress() {
                var postalCode = postalCodeField.val(),
                    houseNumber = houseNumberField.val();

                if (postalCode.length < 4 || houseNumber == '') {
                    return;
                }

                loaderImage.removeClass('hidden');
                streetField.val("");
                cityField.val("");
                latitudeField.val("");
                longitudeField.val("");

                postalCodeCheck(postalCode, houseNumber).done(function (data) {
                    loaderImage.addClass('hidden');
                    if (typeof data.results != 'undefined'
                        && data.results.length > 0
                        && typeof data.results[0].lng != 'undefined'
                        && typeof data.results[0].lat != 'undefined') {

                        streetField.val(data.results[0].street);
                        cityField.val(data.results[0].city);
                        latitudeField.val(data.results[0].lat);
                        longitudeField.val(data.results[0].lng);
                        notFoundAlertElement.addClass('hidden');
                    } else {
                        notFoundAlertElement.removeClass('hidden');
                        streetField.attr("readonly", false);
                        streetField.attr("placeholder", "Voer je straatnaam in...");
                        cityField.attr("readonly", false);
                        cityField.attr("placeholder", "Voer je woonplaats in...");
                    }
                });
            }
        });
    };
})(jQuery);