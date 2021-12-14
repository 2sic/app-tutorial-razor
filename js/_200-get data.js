
// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function init({ moduleId }) {
  // Create a $2sxc object using the current Module Id
  const sxc = $2sxc(moduleId);

  // Get the data source using .data('xy')
  const poetsSvc = sxc.data('Poets');
  
  // Read data from the backend data source with the .getAll() query and pass it to displayPoets
  poetsSvc.getAll().then((poets) => displayPoets(poets));
}

// display example data in table
function displayPoets(poets) {
  Array.prototype.forEach.call(poets.reverse(), (stock, poetIndex) => {
    // make sure only 3 elements are shown
    if (poetIndex >= 3) return
    
    let tr = document.createElement('tr')
    
    let name = document.createElement('td')
    name.innerText = stock.Name
    tr.appendChild(name)
    
    let birthdate = document.createElement('td')
    birthdate.innerText = new Date(stock.BirthDate).toLocaleDateString()
    tr.appendChild(birthdate)
    
    let poems = document.createElement('td')
    poems.innerText = stock.Poems
    tr.appendChild(poems)

    document.querySelector('#example-content > tbody').appendChild(tr)
  });
}

// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

const sDT = window.sxcDataTutorial200 = window.sxcDataTutorial200 || {};
sDT.init = sDT.init || init;