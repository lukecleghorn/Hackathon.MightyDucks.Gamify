// ==UserScript==
// @name     Mighty Ducks - Points Sender
// @version  1
// @grant    none
// @require http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js
// ==/UserScript==
window.addEventListener('load', function() {
    console.log("I am working");
    $.get(
        "https://mightyduckshackatongamify.azurewebsites.net/api/points/send",
        {id : "1", points : 500});
}, false);