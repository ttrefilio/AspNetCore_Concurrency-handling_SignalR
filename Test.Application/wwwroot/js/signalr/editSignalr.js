"use strict";

(function () {

    // #region SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/eventHub")
        .withAutomaticReconnect()
        .build();


    var id = document.getElementById("eventId").value;
    var groupName = "Event-" + id;
    var entityName = "Event";


    connection.on("logInfo", function (message) {
        console.log(message);
    });

    connection.on("informGroupLockReleased", function (result) {
        if (confirm(result.message)) {
            location.reload();
        }
    });

    connection.start().then(function () {

        connection.invoke("AddToGroup", id, entityName, groupName);

    });


    // #endregion



    //// #region Timeout

    //const timeout = function () {
    //    var time;
    //    var stopTimeout = false;
    //    const timeoutDiv = document.getElementById("divTimeout");


    //    (function () {

    //        window.onload = resetTimer;
    //        document.onmousemove = resetTimer;
    //        document.onkeypress = resetTimer;

    //    })()

    //    function kickOut() {
    //        connection.invoke("RemoveLockPageEvent", id, entityName, groupName);
    //        stopTimeout = true;
    //        disableForm();
    //        timeoutDiv.innerText = "You have been disconnected due to a timeout.";
    //        timeoutDiv.hidden = false;            
            
    //    }

    //    function resetTimer() {
    //        if (!stopTimeout) {
    //            clearTimeout(time);
    //            time = setTimeout(kickOut, 25000);
    //            console.log("Timeout reset");
    //        }            
    //    }

    //    resetTimer();
    //};

    //const ownsTheLock = document.getElementById("ownsTheLock").value;

    //if (ownsTheLock) {

    //    window.onload = function () {
    //        timeout();
    //    }

    //    window.onunload = function() {
    //        connection.invoke("RemoveLockPageEvent", id, entityName, groupName);
    //    }
    //}

    //// #endregion

})()






