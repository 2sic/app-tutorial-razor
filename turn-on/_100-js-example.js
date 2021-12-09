function init() {
  // example element gets found in the DOM
  const exampleElement = document.querySelector("#turn-on-example")
  // success text gets displayed in the DOM
  exampleElement.innerText = "turnOn has been executed. 😉";
}

window.razorTutorial = window.razorTutorial || {};
window.razorTutorial.init = init;