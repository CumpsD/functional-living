"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/light-hub").build();

connection.on("ReceiveLightTurnedOnMessage", function (lightId) {
  var lightDiv = document.getElementById("light-" + lightId);
  setLightStatus(lightDiv, "on");
});

connection.on("ReceiveLightTurnedOffMessage", function (lightId) {
  var lightDiv = document.getElementById("light-" + lightId);
  setLightStatus(lightDiv, "off");
});

connection.start().catch(function (err) {
  logMessage("Failed to connect.");
  return console.error(err.toString());
});

function getLights() {
  fetch("/v1/lights")
    .then(function(response) {
      if (response.status !== 200) {
        console.log("Looks like there was a problem. Status Code: " + response.status);
        return;
      }

      response.json().then(function(data) {
        data.lights.forEach(light => addLight(light.id, light.description, light.status));
      });
    })
    .catch(function(err) {
      console.log("Failed to fetch lights", err);
    });
}

function addLight(lightId, description, status) {
  description = description.replace("Verlichting - ", "");
  description = description.replace(" - Aan/Uit", "");

  var lightDiv = document.createElement("div");
  lightDiv.id = "light-" + lightId;
  lightDiv.dataset.lightId = lightId;
  lightDiv.textContent = description;
  lightDiv.tabIndex = -1;

  setLightStatus(lightDiv, status);

  lightDiv.addEventListener("click", clickLight, false);

  lightDiv.onmouseout = function (e) {
    var d = e.target;
    var isFocused = (document.activeElement === d);

    if (isFocused) {
      d.blur();
    }
  }

  document.getElementById("lights").appendChild(lightDiv);
}

function setLightStatus(lightDiv, status) {
  lightDiv.className = "light light-" + status;
  lightDiv.dataset.status = status;
}

function clickLight(e) {
  var lightElement = e.target;
  var lightId = lightElement.dataset.lightId;
  var status = lightElement.dataset.status;
  var desiredStatus = status == "on" ? "off" : "on";

  fetch("/v1/lights/" + lightId + "/" + desiredStatus)
    .then(function(response) {
      if (response.status !== 200) {
        console.log("Looks like there was a problem. Status Code: " + response.status);
        return;
      }

      response.json().then(function(data) {
        data.lights.forEach(light => addLight(light.id, light.description, light.status));
      });
    })
    .catch(function(err) {
      console.log("Failed to fetch lights", err);
    });
}

getLights();
