"use strict";

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
  lightDiv.className = "light light-" + status;
  lightDiv.textContent = description;
  lightDiv.dataset.lightId = lightId;
  lightDiv.dataset.status = status;

  lightDiv.addEventListener("click", clickLight, false);
  document.getElementById("lights").appendChild(lightDiv);
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
