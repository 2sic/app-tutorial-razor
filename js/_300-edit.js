// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home
// As soon as this variable exists, the page will start the code thanks to turnOn
window.editPoets = {
  poetsSvc: null,

  init: function({ moduleId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);

    // Get the data Service - type PoetsToEdit has public create/delete permissions
    poetsSvc = sxc.data('PoetsToEdit');
  },

  add: function() {
    const newPoet = {
      name: document.querySelector('#name').value,
      birthdate: document.querySelector('#birthdate').value,
      poems: document.querySelector('#poems').value
    };

    // Create data in the backend with .create(object) and reload page after
    poetsSvc.create(newPoet).then(() => { alert('created poet, will reload'); location.reload(); });
  },

  delete: function(id) {
    // Delete data in the backend with .delete()
    poetsSvc.delete(id).then(() => { alert('deleted poet, will reload'); location.reload(); });
  },


  updateCount: function(id) {
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
}
