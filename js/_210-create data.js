// TODO: @2mh - use IFFE - see sample 240

var poetsToEdit = {
  poetsSvc: null,

  
}
let poetsSvc;

// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function init({ moduleId }) {
  // Create a $2sxc object using the current Module Id
  const sxc = $2sxc(moduleId);

  // Get the data Service - type PoetsToEdit has public create/delete permissions
  poetsSvc = sxc.data('PoetsToEdit');
}

function addPoet() {
  const newPoet = {
    name: document.querySelector('#name').value,
    birthdate: document.querySelector('#birthdate').value,
    poems: document.querySelector('#poems').value
  };

  // Create data in the backend with .create(object) and reload page after
  poetsSvc.create(newPoet).then(() => { alert('created poet, will reload'); location.reload(); });
}

function deletePoet(id) {
  // Delete data in the backend with .delete()
  poetsSvc.delete(id).then(() => { alert('deleted poet, will reload'); location.reload(); });
}


function updateCount(id) {
  // NOTE: Updated object doesn't need to contain all properties 
  const updatedPoet = {
    Poems: Math.floor(Math.random() * 100).toString()
  };

  // Update data in the backend with .update()
  poetsSvc.update(id, updatedPoet)
    // After backend update, show the new number which the backend returned
    .then(res => {
      document.querySelector(`[data-poet='${id}']`).innerText = res.Poems
    });
}

// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

const sDT = window.sxcDataTutorial210 = window.sxcDataTutorial210 || {};
sDT.init = sDT.init || init;
sDT.add = sDT.add || addPoet;
sDT.delete = sDT.delete || deletePoet;
sDT.updateCount = sDT.updateCount || updateCount;