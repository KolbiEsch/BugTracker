// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

let dropdown = document.querySelector(".dropdown");
let dropdownMenu = document.querySelector(".dropdown__menu");
let dropdownTitle = document.querySelector(".dropdown__title");
const navMenu = document.querySelector(".nav__menu");
const hamburger = document.querySelector(".hamburger");

function doMenuOpen() {
    navMenu.classList.add("active");
    document.addEventListener("keyup", function (e) {
        if (e.keyCode == 27) {
            doMenuClose();
        }
    }, false);
}

function doMenuClose() {
    navMenu.classList.remove("active");
    hamburger.focus();
}

hamburger.addEventListener("click", function () {
    if (navMenu.classList.contains("active")) {
        doMenuClose();
    }
    else {
        doMenuOpen();
    }
}, false);

dropdownTitle.addEventListener("click", () => {
    if (dropdownTitle.classList.contains("active")) {
        dropdownMenu.classList.remove("hover-effect");
    }
    dropdownTitle.classList.toggle("active");
});

dropdown.addEventListener("mouseover", () => {
    dropdownMenu.classList.add("hover-effect");
});

dropdown.addEventListener("mouseout", () => {
    if (dropdownTitle.classList.contains("active")) {
        return;
    }
    dropdownMenu.classList.remove("hover-effect");
});

dropdown.addEventListener("focus", focusFunction, true);
dropdown.addEventListener("focusout", function (e) {
    if (e.currentTarget.contains(e.relatedTarget)) {
        return;
    } else {
        focusOutFunction();
    }
});

function focusFunction() {
    dropdownMenu.classList.add("hover-effect");
}

function focusOutFunction() {
    dropdownMenu.classList.remove("hover-effect");
    dropdownTitle.classList.remove("active");
}



let inputs = document.querySelectorAll(".inputfile");
inputs.forEach(function (input) {

    let label = input.nextElementSibling;
    let labelVal = label.innerHTML;

    input.addEventListener("change", function (e) {
        let fileName = "";

        fileName = e.target.value.split("\\").pop();

        if (fileName) {
            label.querySelector("span").innerHTML = fileName;
        } else {
            label.innerHTML = labelVal;
        }
    });
});

function GetUsers(_projectId) {
    var procemessage = "<option value='0'> Please wait...</option>";
    $("#ddlusers").html(procemessage).show();
    var url = "/MyTickets/GetApplicationUserByProjectId/";

    $.ajax({
        url: url,
        data: { projectId: _projectId },
        cache: false,
        type: "POST",
        success: function (data) {
            var markup = "<option value='0'>Select User</option>";
            for (var x = 0; x < data.length; x++) {
                markup += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
            }
            $("#ddlusers").html(markup).show();
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}





