// Simple init with Data object
function init(data) {
  // example element gets found in the DOM
  const exampleElement = document.querySelector("#turn-on-example")
  // parameters get displayed in the DOM
  exampleElement.innerText = `turnOn has passed the parameters: ${data.word1} ${data.word2}!`;
}

// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function demoDeconstruct({word1, word2}) {
  // example element gets found in the DOM
  const exampleElement = document.querySelector("#turn-on-example")
  // parameters get displayed in the DOM
  exampleElement.innerText = `turnOn has passed the parameters: ${word1} ${word2}!`;
}

const rt = window.razorTutorial = window.razorTutorial || {};
rt.init = rt.init || init;
rt.deconstructed = rt.deconstructed || demoDeconstruct;