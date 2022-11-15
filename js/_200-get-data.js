// Use an IFFE to ensure the variables are not exposed globaly
// See https://developer.mozilla.org/en-US/docs/Glossary/IIFE
(() => {
  // This is a more modern JS feature to deconstruct parameters
  // See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
  function init({ moduleId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);

    // Get the data source using .data('xy')
    const poetsSvc = sxc.data('Poets');

    // Read data from the backend data source with the .getAll() query
    poetsSvc.getAll().then((poets) => {
      // pass poets to displayPoets
      displayPoets(poets);

      // Get data of first poet with .getOne(id) query and log it in the console
      poetsSvc.getOne(poets[0].Id).then((poet) => console.log(`Queried poet using .getOne(): ${poet}`));
    });
  }

  // Display example data in table
  function displayPoets(poets) {
    Array.prototype.forEach.call(poets.reverse(), (poet, poetIndex) => {
      // Make sure only 3 elements are shown
      if (poetIndex >= 3) return
      
      let tr = document.createElement('tr')
      
      addField(tr, poet.Name);
      addField(tr, new Date(poet.BirthDate).toLocaleDateString());
      addField(tr, poet.Poems);

      document.querySelector('#example-content > tbody').appendChild(tr)
    });
  }

  function addField(tr, text) {
    let td = document.createElement('td')
    td.innerText = text
    tr.appendChild(td)
  }

  // This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

  const sDT = window.sxcDataTutorial200 = window.sxcDataTutorial200 || {};
  sDT.init = sDT.init || init;
})();