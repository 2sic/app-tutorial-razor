// data gets passed as a single object, so we need to deconstruct it.
function init({word1, word2}) {
  // example element gets found in the DOM
  const exampleElement = document.querySelector("#turn-on-example")
  // parameters get displayed in the DOM
  exampleElement.innerText = `turnOn has passed the parameters: ${word1} ${word2}!`;
}

window.razorTutorial = window.razorTutorial || {};
window.razorTutorial.init = init;