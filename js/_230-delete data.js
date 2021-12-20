// TODO: @2mh - use IFFE - see sample 240

let poetsSvc;

// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function init({ moduleId }) {
  // Create a $2sxc object using the current Module Id
  const sxc = $2sxc(moduleId);

  // Get the data source using .data('xy')
  poetsSvc = sxc.data('Poets');
}

function deletePoet(id) {
  // Delete data in the backend with .delete()
  poetsSvc.delete(id).then(() => location.reload());
}

// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

const sDT = window.sxcDataTutorial230 = window.sxcDataTutorial230 || {};
sDT.init = sDT.init || init;