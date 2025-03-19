// This tutorial uses turnOn, see turnOn tutorials
// As soon as this variable exists, the page will start the code thanks to turnOn
window.editPoets = {
  poetsSvc: null,

  init: function({ moduleId }) {
    // Create a $2sxc object for the current Module Id
    // ...then get the data Service - type PoetsToEdit has public create/delete permissions
    const sxc = $2sxc(moduleId);
    this.poetsSvc = sxc.data('PoetsToEdit');
  },

  add: function() {
    const newPoet = {
      name: document.querySelector('#name').value,
      birthdate: document.querySelector('#birthdate').value,
      poems: document.querySelector('#poems').value
    };

    // Create data in the backend with .create(object) and reload page after
    this.poetsSvc.create(newPoet).then(() => this.infoAndReload('added poet'));
  },

  // Delete data in the backend with .delete()
  delete: function(id) {
    this.poetsSvc.delete(id).then(() => this.infoAndReload('deleted poet'));
  },


  updateCount: function(id) {
    // NOTE: Updated object doesn't need to contain all properties 
    const updatedPoet = {
      Poems: Math.floor(Math.random() * 100).toString()
    };

    // Update data in the backend with .update()
    this.poetsSvc.update(id, updatedPoet)
      // After backend update, show the new number which the backend returned
      .then(res => {
        document.querySelector(`[data-poet='${id}']`).innerText = res.Poems
      });
  },

  infoAndReload(message) {
    alert(`${message} - will reload`);

    // Special problem: the live site has a cache
    // to protect against AI crawlers overloading the system
    // so we need to add a parameter to the URL to force a reload
    const urlParams = new URLSearchParams(location.search);
    const update = urlParams.get('update');
    // if we have an update in the url, increase it by 1, otherwise add it with 1
    urlParams.set('update', update ? parseInt(update) + 1 : 1);
    // reload the page with the new url parameter
    location.search = urlParams.toString();
  }
}
