let stocksSvc;

// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function init({ moduleId }) {
  // Create a $2sxc object using the current Module Id
  const sxc = $2sxc(moduleId);

  // Get the data source using .data('xy')
  poetSvc = sxc.data('Poets');
}

function addPoet() {
  const newPoet = {
    name: document.querySelector('#name').value,
    birthdate: document.querySelector('#birthdate').value,
    poems: document.querySelector('#poems').value
  };

  // Create data in the backend with .create(object) and reload page after
  poetSvc.create(newPoet).then(() => location.reload());
}


// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

const sDT = window.sxcDataTutorial210 = window.sxcDataTutorial210 || {};
sDT.init = sDT.init || init;