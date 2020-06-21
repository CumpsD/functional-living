"use strict";

var apiName = "Functional Living Knx Api";

function logMessage(message) {
  var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  var now = new Date();

  var p = document.createElement("p");
  p.textContent = "[" + now.toISOString() + "] " + msg;

  document.getElementById("log").appendChild(p);
  updateScroll();
}

var connection = buildConnection(logMessage, "knx-hub");

connection.on("ReceiveKnxMessage", function (message) {
  logMessage(message);
});

function updateScroll() {
  var element = document.getElementById("log");
  element.scrollTop = element.scrollHeight;
}

window.onload = function () {
  start(logMessage);
};
