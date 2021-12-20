// Use an IFFE to ensure the variables are not exposed globaly
// See https://developer.mozilla.org/en-US/docs/Glossary/IIFE
(() => {
  let deadPoetMembersSvc;

  // This is a more modern JS feature to deconstruct parameters
  // See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
  function init({ moduleId }) {
    // Create a $2sxc object using the current Module Id
    const sxc = $2sxc(moduleId);

    // Create the Service for the DeadPoetSocietyMembership Data
    deadPoetMembersSvc = sxc.data('DeadPoetSocietyMembership');
  }

  function add(poetGuid) {
    // Create a random membership number
    const randomMemberId = Math.floor(Math.random() * 999999).toString();

    // Create data in the backend with .create(object, target) and reload page after
    deadPoetMembersSvc.create({ MembershipNumber: randomMemberId }, poetGuid)
      .then(() => {
        alert('Just created new metadata for ' + poetGuid + `. We pretend he's member ${randomMemberId}. Will reload the page now.`);
        location.reload();
      });
  }

  function del(id) {
    if (confirm("Delete this membership?"))
      deadPoetMembersSvc.delete(id).then(() => {
        alert(`Just deleted ${id}. Will reload page now.`);
        location.reload();
      });
  }

  // 
  // This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home
  const sDT = window.sxcDataTutorial240 = window.sxcDataTutorial240 || {};
  sDT.init = sDT.init || init;
  sDT.add = sDT.add || add;
  sDT.del = sDT.del || del;
})();
