
// This will simply show a message in the console and DOM when the script is loaded
// Note that it will automatically run when the script is loaded
// This is not a good practice, but it is useful for this demo

console.log('The script for the Quick Reference was loaded - relative to the the @App.Folder.Url.');

document.getElementById('loadjs-app-message').innerHTML = 
  'Hello from report-loaded-in-console-path-app.js! This was loaded using @App.Folder.Url.';