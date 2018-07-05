// ==UserScript==
// @name     Mighty Ducks - Points Sender
// @version  1
// @grant    none
// @require http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js
// ==/UserScript==
$(document).ready(function() {
    $('a[href*="/solar-renewables/solar-energy"]').click(function(e) {
        sendData();
    });
    if ($("span[class='menu-item__label']:contains('Manage Account')")) {
        console.log("found");
    }
    $("span[class='menu-item__label']:contains('Manage Account')").click(function(e) {
        e.preventDefault();
        sendData();
        return false;
    });
});

function sendData() {
    $.get(
        "https://mightyduckshackatongamify.azurewebsites.net/api/points/send",
        { id: "1", points: 500 });
}
