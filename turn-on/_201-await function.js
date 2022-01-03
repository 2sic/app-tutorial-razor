// Use an IFFE to ensure the variables are not exposed globaly
// See https://developer.mozilla.org/en-US/docs/Glossary/IIFE
(() => {
  // data gets passed as a single object, so we need to deconstruct it.
  function init({domAttribute}) {
    // the element gets found in the DOM by a querySelector with the passed attribute
    const foundElement = document.querySelector(`[${domAttribute}]`)
    foundElement.style.width = '100%';
    foundElement.style.height = '250px';
    
    // thirdparty code gets executed
    animateSpaceship(foundElement)
  }

  function ready() {
    // checks if spritejs exists and returns either true or false
    if (spritejs) return true
    return false
  }

  const tt = window.turnOnTutorial201 = window.turnOnTutorial201 || {};
  tt.init = tt.init || init;
  tt.ready = tt.ready || ready;

  // example code that consumes thirdparty library
  function animateSpaceship(container) {
    const {Scene, Sprite} = spritejs;
    const scene = new Scene({container, width: 1000, height: 250, mode: 'stickyTop'});
    const layer = scene.layer();
    const spaceShip = new Sprite('https://images.squarespace-cdn.com/content/v1/528a31e5e4b00863f1646510/1562710379584-IOMK7TTBLH7H8GJ8QB1A/Screen+Shot+2019-07-09+at+5.12.37+PM.png');
    
    spaceShip.attr({
      anchor: [0, 0.3],
      pos: [0, 0],
    });
    
    spaceShip.animate([
      {pos: [0, 0]},
      {pos: [0, 75]},
      {pos: [600, 100]},
      {pos: [600, 50]},
    ], {
      duration: 4000,
      iterations: Infinity,
      direction: 'alternate',
    });
    
    layer.append(spaceShip);
  }
})();