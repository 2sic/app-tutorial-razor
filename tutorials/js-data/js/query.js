
window.tutQuery = {
  querySvc: null,
  init: function({ moduleId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);
    this.querySvc = sxc.query('RandomAuthorWithBooks');

    // Attach click-handlers to button
    document.getElementById(`mod-${moduleId}-load-all`).onclick = () => this.getAll();
    document.getElementById(`mod-${moduleId}-author`).onclick = () => this.getAuthor();
    document.getElementById(`mod-${moduleId}-streams`).onclick = () => this.getStreams();
  },

  getAll: function() {
    this.querySvc.getAll().then(data => {
      console.log("Get All", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },

  getAuthor: function() {
    this.querySvc.getStream('Author').then(data => {
      console.log("Get Stream 'Author' only", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },

  getStreams: function() {
    this.querySvc.getStreams('Author,Books').then(data => {
      console.log("Get Streams 'Author' and 'Books'", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },
}