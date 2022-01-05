window.queryParameters = {
  querySvc: null,
  // Object with query parameters
  poetQueryParameters: {
    Name: '',
    Poems: -1
  },
  // This is a more modern JS feature to deconstruct parameters
  // See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
  init: function({ moduleId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);

    // Get the data source using .data('xy')
    this.querySvc = sxc.data('Poets');

    // Read data from the backend data source with the .getAll() query
    this.querySvc.getAll().then((poets) => {
      // pass poets to displayPoets
      this.displayPoets(poets);
    });
  },

  // Display example data in table
  displayPoets: function(poets) {
    document.querySelector('#example-content > tbody').innerHTML = "";
    Array.prototype.forEach.call(poets.reverse(), (poet, poetIndex) => {
      // Make sure only 3 elements are shown
      if (poetIndex >= 3) return
      
      let tr = document.createElement('tr')
      
      this.addField(tr, poet.Name);
      this.addField(tr, new Date(poet.BirthDate).toLocaleDateString());
      this.addField(tr, poet.Poems);

      document.querySelector('#example-content > tbody').appendChild(tr)
    });
  },

  queryByParameters: function() {
    // Retrieve set Query Parameters from DOM
    this.poetQueryParameters.Name = document.getElementById("param-name").value;
    this.poetQueryParameters.Poems = document.getElementById("param-poems").value;

    // Build Query string
    const queryString = new URLSearchParams(this.poetQueryParameters).toString();

    const isQueryString = document.getElementById("is-querystring").value
    if (isQueryString) console.log(`Query string: ${queryString}`)

    // TODO 2mh: ask 2dm how query using parameters works
    // Read data from the backend data source with the .getAll() query and additional parameters
    // Note: Parameters can be passed as a string or object
    this.querySvc.getAll(isQueryString ? queryString : this.poetQueryParameters).then((poets) => {
      // Pass poets to displayPoets
      this.displayPoets(poets);
    });
  },

  addField: function(tr, text) {
    let td = document.createElement('td')
    td.innerText = text
    tr.appendChild(td)
  }
}