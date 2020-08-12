"use strict";
(function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/eventHub")
        .withAutomaticReconnect()
        .build();


    connection.on("updateListLockAcquired", function (result) {
        document.querySelector(`tr[data-id='${result.id}'] button`).innerHTML = "Locked";
        document.querySelector(`tr[data-id='${result.id}'] button`).classList.add("btn-outline-danger");
        document.querySelector(`tr[data-id='${result.id}'] button`).classList.remove("btn-outline-success");


    });

    connection.on("updateListLockReleased", function (result) {
        document.querySelector(`tr[data-id='${result.id}'] button`).innerHTML = "Free";
        document.querySelector(`tr[data-id='${result.id}'] button`).classList.add("btn-outline-success");
        document.querySelector(`tr[data-id='${result.id}'] button`).classList.remove("btn-outline-danger");

    });



    connection.start();
})()
