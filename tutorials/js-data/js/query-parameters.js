
window.tutQueryParameters = {
  querySvc: null,
  demoAuthorId: 0,
  init: function({ moduleId, demoAuthorId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);
    this.querySvc = sxc.query('AuthorsWithBooks');
    this.demoAuthorId = demoAuthorId;

    // Attach click-handlers to button
    document.getElementById(`mod-${moduleId}-object`).onclick = () => this.queryWithObjectParams();
    document.getElementById(`mod-${moduleId}-string`).onclick = () => this.queryWithStringParams();
    document.getElementById(`mod-${moduleId}-stream-params`).onclick = () => this.queryStreamWithParams();
    document.getElementById(`mod-${moduleId}-streams-params`).onclick = () => this.queryStreamsWithParams();
  },

  queryWithObjectParams: function() {
    const queryParameters = {
      authorId: this.demoAuthorId,
      authorPageResults: 2,
      currentAuthorPage: 1
    };

    this.querySvc.getAll(queryParameters).then(data => {
      console.log("Get query with passing url parameters as object", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },

  queryWithStringParams: function() {
    // For passing parameters as a string it's important to use the default query string structure (key=value seperated by a &)
    // See: https://en.wikipedia.org/wiki/Query_string#Structure
    const stringWithParameters = `authorId=${this.demoAuthorId}&authorPageResults=2&currentAuthorPage=1`;

    this.querySvc.getAll(stringWithParameters).then(data => {
      console.log("Get query with passing url parameters as string", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },

  queryStreamWithParams: function() {
    const queryParameters = {
      authorId: this.demoAuthorId
    };

    this.querySvc.getStream('Current', queryParameters).then(data => {
      console.log("Get Query stream with passing url parameter", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  },

  queryStreamsWithParams: function() {
    const queryParameters = {
      authorId: this.demoAuthorId,
      authorPageResults: 2,
      currentAuthorPage: 1
    };

    this.querySvc.getStreams('Current,AuthorsByPage', queryParameters).then(data => {
      console.log("Get query streams with passing url parameter", data);
      alert('Got all - see console for details. \n \n' + JSON.stringify(data, null, 2));
    });
  }
}