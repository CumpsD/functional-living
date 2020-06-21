"use strict";

var retryMatrix = [0, 350, 1000, 5000];

function buildConnection(logMessage, hubName) {
  var connection = new signalR.HubConnectionBuilder()
    .withUrl("/" + hubName)
    .withAutomaticReconnect(retryMatrix)
    .configureLogging(signalR.LogLevel.Information)
    .build();

  connection.onreconnecting(() => {
    logMessage("Reconnecting: " + apiName);
    setBackground("lost-connection");
  });

  connection.onreconnected(() => {
    logMessage("Reconnected: " + apiName);
    setBackground("connected");
  });

  connection.onclose(() => {
    logMessage("Lost connection: " + apiName);
    setBackground("lost-connection");
    start(logMessage);
  });

  return connection;
}

function start(logMessage, attempt = 0) {
  logMessage("Trying to connect to: " + apiName);

  connection
    .start()
    .then(function () {
      logMessage("Connected to: " + apiName);
      setBackground("connected");
    }).catch(function () {
      logMessage("Failed to connect to: " + apiName);
      setBackground("lost-connection");
      setTimeout(() => start(logMessage, attempt + 1), getDelay(attempt + 1));
    });
};

function getDelay(attempt) {
  var retryIndex = attempt - 1;
  if (retryIndex > 3) retryIndex = 3;

  return retryMatrix[retryIndex];
}

function setBackground(backgroundClass) {
  document.body.className = backgroundClass;
}

function logMessage(message, args) {
  var now = new Date();
  if (args) {
    window.console.log("[" + now.toISOString() + "] " + message, args);
  } else {
    window.console.log("[" + now.toISOString() + "] " + message);
  }
}
