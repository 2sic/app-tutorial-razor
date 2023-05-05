// Use an IFFE to ensure the variables are not exposed globaly
// See https://developer.mozilla.org/en-US/docs/Glossary/IIFE
(() => {
  // Simple init with Data object
  function init(data) {
    // Example element gets found in the DOM
    const exampleElement = document.querySelector("#turn-on-example")
    // Parameters get displayed in the DOM
    exampleElement.innerText = `turnOn has passed the parameters: ${data.greeting} ${data.name}!`;
  }
  
  // This is a more modern JS feature to deconstruct parameters
  // See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
  function demoDeconstruct({greeting, name}) {
    // Example element gets found in the DOM
    const exampleElement = document.querySelector("#turn-on-example-deconstruct")
    // Parameters get displayed in the DOM
    exampleElement.innerText = `turnOn has passed the parameters: ${greeting} ${name}!`;
  }
  
  const tt = window.turnOnTutorial110 = window.turnOnTutorial110 || {};
  tt.init = tt.init || init;
  tt.demoDeconstruct = tt.demoDeconstruct || demoDeconstruct;
})();