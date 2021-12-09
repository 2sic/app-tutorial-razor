// data gets passed as a single object, so we need to deconstruct it.
function init({domAttribute}) {
  // the element gets found in the DOM by a querySelector with the passed attribute
  const foundElement = document.querySelector(`[${domAttribute}]`)
  // parameters get displayed in the DOM
  foundElement.innerText = `turnOn has passed the domAttribute: ${domAttribute}`;
}

window.razorTutorial = window.razorTutorial || {};
window.razorTutorial.init = init;