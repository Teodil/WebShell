// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let textarea = document.getElementById('textarea');
var lastLine;
let history=[];
var historyIndex = 0;

let xhttp = new XMLHttpRequest();

let currentSelect = document.getElementById('selector').options.selectedIndex;


window.onload = function () {
    textarea.value += "User:>";
    var text = textarea.value.split(/[\r\n]+/);
    lastLine = text[text.length - 1];
}

textarea.oninput = function () {
    var text = textarea.value.split(/[\r\n]+/);
    lastLine = text[text.length - 1];

}

//Перефокусировка курсора
textarea.onclick = function () {
    var pos = textarea.value.length;
    textarea.focus();
    textarea.setSelectionRange(pos, pos);
}

document.addEventListener('keydown', function (e) {
    //Отправка данных(Enter)
    if (e.keyCode == 13) {
        textarea.setAttribute('readonly', '');
        var text = textarea.value.split(/[\r\n]+/);
        console.log('Весь текст: ' + text);
        lastLine = text[text.length - 1];
        lastLine = lastLine.substring(lastLine.indexOf(">") + 1);
        console.log('Сообщение: ' + lastLine);
        history.push(lastLine);
        historyIndex = history.length;
        var shell = document.getElementById('selector').options[currentSelect].value;
        console.log('Shell: ' + shell);

        xhttp.open("GET", "Home/Request?request=" + lastLine + "&shell=" + shell, true);
        xhttp.send();
    }
    //Удаление строки(Backspace)
    if (e.keyCode == 8) {
        var CursorPos = textarea.selectionStart;
        if (textarea.value[CursorPos - 1] == '>') {
            console.log("Не должно удаляться");
            e.preventDefault();
        }
    }
    //Стрелка вверх
    if (e.keyCode == 38) {
        console.log("История "+history)
        e.preventDefault();
        if (history.length > 0) {
            if (historyIndex > 0) {
                historyIndex -= 1;
            }
            else {
                historyIndex = history.length - 1;
            }
            var text = textarea.value.split(/[\r\n]+/);
            lastLine = text[text.length - 1];
            lastLine = lastLine.substring(lastLine.indexOf(">") + 1);
            console.log("Замена " + lastLine + " На " + history[historyIndex]);
            var newValue = textarea.value.replace(new RegExp(lastLine+'$'), history[historyIndex]);
            textarea.value = newValue;
        }
    }
    //Стрелка вниз
    if (e.keyCode == 40) {
        e.preventDefault();
        if (history.length > 0) {
            if (historyIndex < history.length-1) {
                historyIndex += 1;
            }
            else {
                historyIndex = 0;
            }
            var text = textarea.value.split(/[\r\n]+/);
            lastLine = text[text.length - 1];
            lastLine = lastLine.substring(lastLine.indexOf(">") + 1);
            console.log("Замена " + lastLine + " На " + history[historyIndex]);
            var newValue = textarea.value.replace(new RegExp(lastLine + '$'), history[historyIndex]);
            textarea.value = newValue;
        }
    }
    //Стрелка влево
    if (e.keyCode == 37) {
        var CursorPos = textarea.selectionStart;
        if (textarea.value[CursorPos-1] == '>') {
            //console.log("Не должно двигаться");
            e.preventDefault();
        }
    }
    //Удаление по Delete
    if (e.keyCode == 46) {
        e.preventDefault();
    }
});


xhttp.onreadystatechange = function () {
    if (this.readyState == 4 & this.status == 200) {
        if (this.status == 200) {
            SetResponse(this.responseText);
        }
        else {
            SetResponse("Произошла ошибка");
        }
        textarea.removeAttribute('readonly');
    }
    else {
        console.log(this.readyState);
    }
}

function SetResponse(data) {
    var responce = "\nServer:>\n" + data + "\nUser:>";
    textarea.value += responce;
    var text = textarea.value.split(/[\r\n]+/);
    lastLine = text[text.length - 1];
    textarea.focus();
    textarea.setSelectionRange(textarea.value.length, textarea.value.length);
}

//-Селектор-//
document.getElementById('selector').onchange = function () {
    currentSelect = this.options.selectedIndex;
    console.log(currentSelect);
    textarea.value += "\nШел изменён на " + this.options[currentSelect].text;
    if (xhttp.readyState != 1) {
        textarea.value += "\nUser:>";
        var text = textarea.value.split(/[\r\n]+/);
        lastLine = text[text.length - 1];
    }
}
