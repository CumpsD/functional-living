"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/knx-hub").build();

connection.on("ReceiveKnxMessage", function (message) {
  logMessage(message);
});

connection.start().then(function () {
  logMessage("Connected to: Functional Living Knx Api");
}).catch(function (err) {
  return console.error(err.toString());
});

function logMessage(message) {
  var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  var now = new Date();
  
  var p = document.createElement("p");
  p.textContent = "[" + now.toISOString() + "] " + msg;

  document.getElementById("log").appendChild(p);
  updateScroll();
}

function updateScroll() {
  var element = document.getElementById("log");
  element.scrollTop = element.scrollHeight;
}
