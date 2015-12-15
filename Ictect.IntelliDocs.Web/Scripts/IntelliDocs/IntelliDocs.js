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

    var uploadDocument = function () {
        $.get(IntelliDocs.baseUrl + "File/Upload/" + IntelliDocs.library.workingDirId, function (data) {
            $("#ajax-modal .modal-content").html(data);
        });
    };

    var deleteDocument = function(docId) {
        $.get(IntelliDocs.baseUrl = "File/Delete/" + docId, function (data) {
            $("#doc-actions-result").html(data.message);
            if (data.status === "success") {
                IntelliDocs.reloadLibrary();
                $("#doc-actions").hide(400);
                $("#doc-actions-result").addClass("success");
            } else {
                $("#doc-actions-result").addClass("error");
            }
        });
    }

    var newFolder = function (dirName, input) {
        var dialogId = IntelliDocs.library.showLoader("Creating your new Directory...", "We appreciate your patience while we create your new Directory, <strong>" + dirName + "</strong>.", false);
        $.post(IntelliDocs.baseUrl + "Library/CreateFolder", { parentDirId: IntelliDocs.library.parentDirId, dirName: dirName, libraryId: IntelliDocs.library.libraryId }, function (result) {
            IntelliDocs.library.hideLoader(dialogId);
            if (result.status === "success") {
                IntelliDocs.reloadLibrary();
                IntelliDocs.showAlert(result.message, "alert-success");
            } else {
                IntelliDocs.showAlert(result.message, "alert-danger");
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
                    $("[data-toggle='modal'][data-url]").click(function (e) {
                        var target = $(this).data("target");
                        $.get($(this).data("url"), function (data) {
                            $(target).find(".modal-content").html(data);
                        });
                    });
                });
                this.library.hideLoader(dialogId);
                //$("[data-action='load-file'][data-docid]").click(function (e) {
                //    return IntelliDocs.loadFile($(this).data("docid"));
                //});
            },
            reloadLibrary: function () {
                return IntelliDocs.loadLibrary(IntelliDocs.library.workingDirId);
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
                },
                dirName: "",
                fullPath: ""
            },
            setLibrary: function (workingDirId, parentDirId, libraryId, dirName, fullPath) {
                this.library.workingDirId = workingDirId;
                this.library.parentDirId = parentDirId;
                this.library.libraryId = libraryId;
                this.library.dirName = dirName;
                this.library.fullPath = fullPath;
                //window.location.href = fullPath;
            },
            viewShares: function () {
                $.get(this.baseUrl + "Shares", function (data) {
                    $("#content-container").html(data);
                });
            },
            viewDocShares: function(docId) {
                $.get(this.baseUrl + "Shares/ShareDocument/" + docId, function(data) {
                    $("#content-container").html(data);
                });
            },
            viewLibrary: function () {
                window.location = this.baseUrl;
            },
            loadFile: function (docId) {
                window.open(IntelliDocs.baseUrl + "File/Get/" + docId, "_blank");
            },
            showAlert: function(alertText, alertClass) {
                // set the message to display: none to fade it in later.
                if (alertClass === undefined) {
                    alertClass = "alert-danger";
                }
                var message = $("<div class=\"alert " + alertClass + " error-message\" style=\"display: none;\">");
                // a close button
                var close = $("<button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times</button>");
                message.append(close); // adding the close button to the message
                message.append(alertText); // adding the error response to the message
                // add the message element to the body, fadein, wait 3secs, fadeout
                message.appendTo($("body")).fadeIn(300).delay(3000).fadeOut(500);
            },
            deleteFile: function(docId) {
                return deleteDocument(docId);
            },
            viewDirShares: function(dirId) {
                $.get(this.baseUrl + "Shares/ShareDirectory/" + dirId, function(data) {
                    $("#content-container").html(data);
});
            },
            confirmDirDelete: function(dirId) {
                $.get(IntelliDocs.baseUrl + "Library/DeleteDirectoryConfirm/" + dirId, function (data) {
                    $("#ajax-modal .modal-content").html(data);
                });
            },
            deleteDirectory: function(dirId) {
                $.get(IntelliDocs.baseUrl + "Library/DeleteDirectory/" + dirId, function(data) {
                    $("#dir-actions-result").html(data.message);
                    if (data.status === "success") {
                        IntelliDocs.reloadLibrary();
                        $("#dir-actions").hide(400);
                        $("#dir-actions-result").addClass("success");
                    } else {
                        $("#dir-actions-result").addClass("error");
                    }
                });
            },
            createDirectoryModal: function() {
                $.get(IntelliDocs.baseUrl + "Library/NewFolder", function(data) {
                    $("#ajax-modal .modal-content").html(data);
                });
            }
        };

        window.IntelliDocs = intelliDocsObj;
    };
    // Call initIntelliDocs to instantiate and add IntelliDocs to window
    initIntelliDocs();
})(window);

/* jQuery-dependent */
$(function () {
    $("#header-actions [data-action='new-folder']").click(function() {
        IntelliDocs.createDirectoryModal();
    });

    $("[data-action='delete-folder']").click(function() {
        IntelliDocs.confirmDirDelete(IntelliDocs.library.workingDirId);
    });

    $("[data-action='share-folder']").click(function() {
        IntelliDocs.viewDirShares(IntelliDocs.library.workingDirId);
    });

    $("[data-action='view-doc-shares'][data-docid]").click(function() {
        IntelliDocs.viewDocShares($(this).data("docid"));
    });

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
        if (e.keyCode === 13) {
            $(this).trigger("enterKey");
        }
    });

    $(".library-folder[data-dirid], [data-action='loadDirectory']").click(function (e) {
        return IntelliDocs.loadLibrary($(this).data("dirid"));
    });

    //$("[data-action='load-file'][data-docid]").click(function (e) {
    //    return IntelliDocs.loadFile($(this).data("docid"));
    //});

    $("[data-toggle='modal'][data-url]").click(function (e) {
        var target = $(this).data("target");
        $.get($(this).data("url"), function(data) {         
            $(target).find(".modal-content").html(data);
        });
    });
});