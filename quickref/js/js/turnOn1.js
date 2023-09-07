// Make sure the object exists, which can contain various scripts for the Quick Reference
// This makes sure that if other scripts also create it, they won't interfere with each other
window.quickReference = window.quickReference || {};

window.quickReference.turnOn1Message = function () {
  // get div in html with id 'turnOn1-message' and show "Hello World!"
  const tag = document.getElementById('turnOn1-message')
  tag.innerHTML = 'Hello from turnOn1.js! This was activated using turnOn';
}

window.quickReference.turnOn2Message = function (message) {
  // get div in html with id 'turnOn1-message' and show "Hello World!"
  const tag = document.getElementById('turnOn2-message');
  tag.innerHTML = message;
}

window.quickReference.turnOn3Message = function (config) {
  // get div in html with id 'turnOn1-message' and show "Hello World!"
  const tag = document.getElementById(config.domId);
  tag.innerHTML = config.message + 
    '; PageId: ' + config.pageId + 
    "; Url: " + config.pageUrl;
}