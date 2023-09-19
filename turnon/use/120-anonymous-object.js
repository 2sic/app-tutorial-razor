(() => {
  // Data gets passed as a single object, so we need to deconstruct it.
  function init({domId, style}) {
    // The element gets found in the DOM by a querySelector with the passed attribute
    const foundElement = document.querySelector(`#${domId}`)
  
    // apply passed settings on DOM element
    foundElement.style.backgroundColor = style.backgroundColor;
    foundElement.style.height = `${style.height}px`;
    foundElement.style.width = `${style.width}px`;
  
    // Parameters get displayed in the DOM
    foundElement.innerText = `turnOn has passed this data: ${JSON.stringify({domId, style})}`;
  }
  
  const tt = window.turnOnTutorial120 = window.turnOnTutorial120 || {};
  tt.init = tt.init || init;
})();
