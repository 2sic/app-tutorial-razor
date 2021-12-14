let deadPoetMembersSvc;

// This is a more modern JS feature to deconstruct parameters
// See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Destructuring_assignment
function init({ moduleId }) {
  // Create a $2sxc object using the current Module Id
  const sxc = $2sxc(moduleId);

  // Get the data source using .data('xy')
  deadPoetMembersSvc = sxc.data('DeadPoetSocietyMembership');
}

function addMembership(poetGuid) {
  const membershipMetadata = { 
    membershipNumber: Math.floor(Math.random() * 999999).toString()
  };

  const target = { 
    TargetType: 4, // for Entity, see: https://docs.2sxc.org/basics/metadata/target-types.html
    Guid: poetGuid
  };

  // Create data in the backend with .create(object, target) and reload page after
  deadPoetMembersSvc.create(membershipMetadata, target).then(() => location.reload());
}

// This tutorial uses turnOn, see https://app-dev.2sxc.org/tutorial-razor/en-bs4/Home/turn-on/home

const sDT = window.sxcDataTutorial240 = window.sxcDataTutorial240 || {};
sDT.init = sDT.init || init;