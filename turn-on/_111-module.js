// Use an IFFE to ensure the variables are not exposed globaly
// See https://developer.mozilla.org/en-US/docs/Glossary/IIFE
(() => {
  // Data gets passed as a single object, so we need to deconstruct it.
  function init({domAttribute}) {
    // The element gets found in the DOM by a querySelector with the passed attribute
    const foundElement = document.querySelector(`[${domAttribute}]`)
    // Parameters get displayed in the DOM
    foundElement.innerText = `turnOn has passed the domAttribute: ${domAttribute}`;
  }
  
  const tt = window.turnOnTutorial111 = window.turnOnTutorial111 || {};
  tt.init = tt.init || init;
})();