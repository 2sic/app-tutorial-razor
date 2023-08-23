// Make sure the object exists, which can contain various scripts for the Quick Reference
// This makes sure that if other scripts also create it, they won't interfere with each other
window.quickReference = window.quickReference || {};

// Bind a click action to the button with the unique id
window.quickReference.demoUniqueQuey = function (ids) {
  document.getElementById(ids.buttonId)
    .addEventListener("click", function () {
      const msg = 'The button was pressed. ' +
      'This was only possible thanks to the unique id. ' + 
      'The ID this time was: ' + ids.buttonId + '.';

      // alert(msg);
      
      document.getElementById(ids.messageId).innerHTML = msg;
  });
}