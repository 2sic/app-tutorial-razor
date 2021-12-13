// The init function which should be called
function init({ domAttribute }) {
  // Example element gets found in the DOM and bound to Fancybox
  Fancybox.bind(`[${domAttribute}]`);
}

const tt = window.turnOnTutorial202 = window.turnOnTutorial202 || {};
tt.init = tt.init || init;