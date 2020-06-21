"use strict";

var apiName = "Functional Living Knx Api";

var connection = new signalR.HubConnectionBuilder()
  .withUrl("/knx-hub")
  .configureLogging(signalR.LogLevel.Information)
  .build();

connection.on("ReceiveKnxMessage", function (message) {
  logMessage(message);
});

function start() {
  logMessage("Trying to connect to: " + apiName);

  connection
    .start()
    .then(function () {
      logMessage("Connected to: " + apiName);
      setBackground("connected");
    }).catch(function () {
      logMessage("Failed to connect to: " + apiName);
      setBackground("lost-connection");
      setTimeout(() => start(), 5000);
    });
};

connection.onclose(() => {
  logMessage("Lost connection: " + apiName);
  setBackground("lost-connection");
  start();
});

function setBackground(backgroundClass) {
  document.body.className = backgroundClass;
}

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

window.onload = function () {
  start();
};
