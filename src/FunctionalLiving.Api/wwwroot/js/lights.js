"use strict";

var apiName = "Functional Living Lights Api";
var retryMatrix = [0, 200, 200, 5000];
var itemsPerRow = 4;

var connection = buildConnection(logMessage, "light-hub");

connection.on("ReceiveLightTurnedUnknownMessage", function (lightId) {
  var lightDiv = document.getElementById("light-" + lightId);
  setLightStatus(lightDiv, "unknown");
});

connection.on("ReceiveLightTurnedOnMessage", function (lightId) {
  var lightDiv = document.getElementById("light-" + lightId);
  setLightStatus(lightDiv, "on");
});

connection.on("ReceiveLightTurnedOffMessage", function (lightId) {
  var lightDiv = document.getElementById("light-" + lightId);
  setLightStatus(lightDiv, "off");
});

function getLights() {
  logMessage("Getting Lights from /v1/lights");

  fetch("/v1/lights")
    .then(function(response) {
      if (response.status !== 200) {
        logMessage("Looks like there was a problem. Status Code: " + response.status);
        return;
      }

      response.json().then(function(data) {
        data
          .lights
          .sort(function(a, b) {
            var nameA = a.description.toUpperCase();
            var nameB = b.description.toUpperCase();

            if (nameA < nameB) return -1;
            if (nameA > nameB) return 1;
            return 0;
          })
          .forEach(light => addLight(light.id, light.description, light.status));

        var numberOfLights = data.lights.length;
        var fillerSlots = itemsPerRow - (numberOfLights % itemsPerRow);
        if (fillerSlots === itemsPerRow) fillerSlots = 0;
        for (var i = 0; i < fillerSlots; i++) {
          addFiller();
        }
      });
    })
    .catch(function(err) {
      logMessage("Failed to fetch lights", err);
    });
}

function addFiller() {
  var fillerDiv = document.createElement("div");
  fillerDiv.className = "filler";
  document.getElementById("lights").appendChild(fillerDiv);
}

function addLight(lightId, description, status) {
  description = description.replace("Verlichting - ", "");
  description = description.replace(" - Aan/Uit", "");

  var lightDiv = document.createElement("div");
  lightDiv.id = "light-" + lightId;
  lightDiv.dataset.lightId = lightId;
  lightDiv.tabIndex = -1;

  var labelDiv = document.createElement("div");
  labelDiv.textContent = description;
  lightDiv.append(labelDiv);

  setLightStatus(lightDiv, status);

  lightDiv.addEventListener("onmouseout", resetHover, false);
  lightDiv.addEventListener("click", clickLight, false);
  document.getElementById("lights").appendChild(lightDiv);
}

function setLightStatus(lightDiv, status) {
  logMessage("Updating Light '" + lightDiv.dataset.lightId + "' Status: " + status);
  lightDiv.className = "light light-" + status;
  lightDiv.dataset.status = status;
}

function resetHover(e) {
  var d = e.currentTarget;
  var isFocused = (document.activeElement === d);

  if (isFocused) {
    d.blur();
  }
}

function clickLight(e) {
  e.stopPropagation();

  var lightElement = e.currentTarget;
  var lightId = lightElement.dataset.lightId;
  var status = lightElement.dataset.status;
  var desiredStatus = status === "on" ? "off" : "on";

  logMessage("Updating Light '" + lightId + "' Status: " + desiredStatus);

  fetch("/v1/lights/" + lightId + "/" + desiredStatus)
    .then(function(response) {
      if (response.status !== 202) {
        logMessage("Looks like there was a problem. Status Code: " + response.status);
        return;
      }

      logMessage("Updated Light '" + lightId + "' Status: " + desiredStatus);
    })
    .catch(function (err) {
      logMessage("Failed to update light", err);
    });
}

window.onload = function () {
  start(logMessage);
  getLights();
};
