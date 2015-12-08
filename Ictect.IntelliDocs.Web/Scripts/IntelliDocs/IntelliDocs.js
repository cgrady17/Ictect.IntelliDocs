/*! IntelliDocs JavaScript Framework
 *  Globally-accessible object for IntelliDocs-related
 *  functions and properties.
 * 
 *  Requires: jQuery
 */
"use strict";

(function () {
    /* Appends the Splash Page to the page and then removes it after 2 seconds */
    var initSplash = function (timeout) {
        $.get(IntelliDocs.baseUrl + "Home/SplashPage", function (data) {
            $("body").append(data).addClass("no-scroll");
            setTimeout(function () {
                $("body").removeClass("no-scroll");
                $("#splash-container").remove();
            }, timeout || 2000);
        });
    };

    // Instantiate the IntelliDocs Object
    var initIntelliDocs = function () {
        var intelliDocsObj = {
            showSplash: function (timeout) {
                return initSplash(timeout);
            },
            baseUrl: "http://localhost/IntelliDocs/",
            uploadDoc: function () {
                return uploadDocument();
            },
            newFolder: function (dirName) {
                return newFolder(dirName);
            },
            loadLibrary: function (dirId) {
                // Load library at specified directory
                var dialogId = this.library.showLoader("Loading Directory...", "We appreciate your patience while we load the request Directory and its contents.", false);
                $.post(this.baseUrl + "Library/LoadDirectory", { dirId: dirId, libraryId: this.library.libraryId, parentDirId: this.library.workingDirId }, function (result) {
                    $("#library-content").html(result);
                    $(".library-folder[data-dirid], [data-action='loadDirectory']").click(function (e) {
                        return IntelliDocs.loadLibrary($(this).data("dirid"));
                    });
                });
                this.library.hideLoader(dialogId);
            },
            library: {
                workingDirId: 0,
                parentDirId: 0,
                libraryId: 0,
                showLoader: function (header, body, showClose) {
                    return showLibraryLoader(header, body, showClose);
                },
                hideLoader: function (dialogId) {
                    return hideLibraryLoader(dialogId);
                }
            },
            setLibrary: function (workingDirId, parentDirId, libraryId) {
                this.library.workingDirId = workingDirId;
                this.library.parentDirId = parentDirId;
                this.library.libraryId = libraryId;
            },
            viewShares: function () {
                $.get(this.baseUrl + "Shares", function (data) {
                    $("#content-container").html(data);
                });
            },
            viewLibrary: function () {
                window.location = this.baseUrl;
            }
        };

        window.IntelliDocs = intelliDocsObj;
    };
    // Call initIntelliDocs to instantiate and add IntelliDocs to window
    initIntelliDocs();

    var uploadDocument = function () {
        alert("Coming Soon");
    };

    var newFolder = function (dirName, input) {
        IntelliDocs.library.showLoader("Creating your new Directory...", "We appreciate your patience while we create your new Directory, <strong>" + dirName + "</strong>.", false);
        $.post(IntelliDocs.baseUrl + "Library/CreateFolder", { parentDirId: IntelliDocs.library.parentDirId, dirName: dirName, libraryId: IntelliDocs.library.libraryId }, function (result) {
            IntelliDocs.library.hideLoader();
            if (result.status === "success") {
                if (input !== undefined) {
                    input.attr("readonly", true);
                    input.parent().find(".glyphicon-plus").remove();
                }
                alert("Success! New dir ID: " + result.message);
            } else {
                alert("Failure! Message: " + result.message);
            }
        });
    };

    var showLibraryLoader = function (header, body, showClose) {
        var dialogId = Math.floor(Math.random() * 300);
        $("#content-container").append("<div id='dialog-" + dialogId + "' class='dialog-overlay'><div class='dialog-container'><div class='dialog-body'><h2>" + header + "</h2><p>" + body + "</p>" + (showClose === true ? "<button type='button' class='btn btn-primary' onclick='IntelliDocs.library.hideLoader(" + dialogId + ")'>Close</button>" : "") + "</div></div></div>");
        $("#dialog-" + dialogId).show(400);
        return dialogId;
    };

    var hideLibraryLoader = function (dialogId) {
        $("#dialog-" + dialogId).hide(400).remove();
    };
})(window);

/* jQuery-dependent */
$(function () {
    $("[data-action='view-shares'").click(function (e) {
        IntelliDocs.viewShares();
        $(this).parent().parent().find("a").removeClass("active");
        $(this).addClass("active");
    });

    $("[data-action='view-library']").click(function (e) {
        IntelliDocs.viewLibrary();
        $(this).parent().parent().find("a").removeClass("active");
        $(this).addClass("active");
    });

    $("[data-action='upload']").click(function (e) {
        return IntelliDocs.uploadDoc();
    });

    $("[data-action='new-folder']").click(function (e) {
        $(this).find("input").focus();
    });

    $("input#new-folder-name").bind("enterKey", function (e) {
        // Create Folder
        return IntelliDocs.newFolder($(this).val());
    });
    $("input#new-folder-name").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).trigger("enterKey");
        }
    });

    $(".library-folder[data-dirid], [data-action='loadDirectory']").click(function (e) {
        return IntelliDocs.loadLibrary($(this).data("dirid"));
    });
});