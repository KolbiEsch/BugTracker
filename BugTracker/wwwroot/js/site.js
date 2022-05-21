﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const tx = document.getElementsByTagName("textarea");
for (let i = 0; i < tx.length; i++) {
    tx[i].setAttribute("style", "height:" + (tx[i].scrollHeight) + "px;overflow-y:hidden;resize:none;");
    tx[i].addEventListener("input", OnInput, false);
}

function OnInput() {
    this.style.height = "auto";
    this.style.height = (this.scrollHeight) + "px";
}

$(window).resize(function () {
    drawChart();
})

const navEl = document.body.querySelector("#navigation");
const navHeight = navEl.offsetHeight;

function onLoad(navHeight) {
    const main = document.body.querySelector(".main");
    main.style.height = window.innerHeight - navHeight + "px";
}

window.onload = function () {
    onLoad(navHeight);
    
};

window.onresize = function () {
    onLoad(navHeight);
}



