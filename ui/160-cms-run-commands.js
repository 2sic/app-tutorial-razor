function openAndCancelRefreshAfterwards(tag, event, action, params) {
  // Prevent the click from propagating, so the link won't execute href
  event.preventDefault();

  // This workflow step will run on every action, just to log what's happening
  const workflowToLog = {
    command: 'all',   // Run on every command/action
    phase: 'all',     // Run before and after
    code: (wfArgs) => {
      console.log("Toolbar asked to to something - here are the details.", wfArgs);
    }
  }

  // This is the workflow step we will register to stop page refresh
  const workflowToDisableRefresh = {
    command: 'refresh',   // The command name it's for
    phase: 'before',      // The workflow-step should run before the command is executed
    code: (wfArgs) => {   // The code which should be run
      console.log('Toolbar asked to refresh, will return false to stop it. These are the arguments we got.', wfArgs);
      return false;       // Return false to stop this command from happening
    }
  };

  $2sxc(tag).cms.run({ action: action, params: params, workflows: [workflowToLog, workflowToDisableRefresh]})
    .then(function(data) {
      console.log("after run", data);
      return false;
    });
  
}