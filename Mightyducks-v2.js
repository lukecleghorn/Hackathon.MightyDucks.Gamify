// ==UserScript==
// @name     mightducks v2
// @version  1
// @grant    none
// @require  https://gist.github.com/raw/2625891/waitForKeyElements.js
// @require http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js
// ==/UserScript==

waitForKeyElements(
    ".loaded",
    init
);

function init() { }

waitForKeyElements(
    "html body agl-apps.loaded agl-account-app.ng-star-inserted agl-app-header header agl-desktop-menu.ng-tns-c11-1 div.agl-desktop-wrapper.desktop div.agl-menu.agl-desktop-header div.agl-desktop-header__container div.agl-desktop-header-menu ul.agl-desktop-header-menu__tabs li#agl-desktop-header-menu-bills.ng-tns-c11-1.ng-star-inserted a.ng-tns-c11-1 span.agl-desktop-header-menu__menu-item-text agl-menu-item.ng-tns-c11-1",
    init2
)

function init2() {
    $('#agl-desktop-header-menu-rewards').click(function () {
        console.log("clicked");
        sendData();
    });
}

function sendData() {
    $.get(
        "https://mightyduckshackatongamify.azurewebsites.net/api/points/send",
        { id: "1", points: 500 });
}