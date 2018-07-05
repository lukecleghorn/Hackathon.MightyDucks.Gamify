// ==UserScript==
// @name     Mighty Ducks - Points Sender
// @version  1
// @grant    none
// @require http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js
// ==/UserScript==
$(document).ready(function() {
    $('a[href*="/solar-renewables/solar-energy"]').click(function(e) {
        e.preventDefault();

        console.log("clicked");
        sendData();
        return false;
    });
});
function sendData() {
    console.log("I am working");
    $.get(
        "https://mightyduckshackatongamify.azurewebsites.net/api/points/send",
        { id: "1", points: 500 });
}
