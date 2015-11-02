/*! IntelliDocs JavaScript Framework
 *  Globally-accessible object for IntelliDocs-related
 *  functions and properties.
 * 
 *  Requires: jQuery
 */
"use strict";

/* Appends the Splash Page to the page and then removes it after 2 seconds */
var initSplash = function () {
    $.get(IntelliDocs.baseUrl + "Home/SplashPage", function(data) {
        $("body").append(data).addClass("no-scroll");
        setTimeout(function() {
            $("body").removeClass("no-scroll");
            $("#splash-container").remove();
        }, 2000);
    });
};

// Instantiate the IntelliDocs Object
var initIntelliDocs = function () {
    var intelliDocsObj = {
        showSplash: function () {
            return initSplash();
        },
        baseUrl: "http://localhost/IntelliDocs/"
    };

    window.IntelliDocs = intelliDocsObj;
};
// Call initIntelliDocs to instantiate and add IntelliDocs to window
initIntelliDocs();