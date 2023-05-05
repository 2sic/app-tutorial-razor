
window.toolbarWorkflow = {
  // This workflow definition will run on every action, just to log what's happening
  workflowToLog: {
    command: 'all',   // Run on every command/action
    phase: 'all',     // Run before and after
    code: (wfArgs) => {
      console.log("Toolbar asked to to something - here are the details.", wfArgs);
    }
  },

  // This is the workflow definition we will register to stop page refresh
  workflowToDisableRefresh: {
    command: 'refresh',   // The command name it's for
    phase: 'before',      // The workflow-step should run before the command is executed
    code: (wfArgs) => {   // The code which should be run
        console.log('Toolbar asked to refresh, will return false to stop it. These are the arguments we got.', wfArgs);
        return false;       // Return false to stop this command from happening
    }
  },

  // Attach event-listener to the TagToolbar parent, so we can register the workflow when the toolbar is created
  initOnTagToolbar: function(name) {
    console.log('initInlineToolbar(' + name + ')');
    var parent = document.getElementById(name);
    console.log('parent:', parent);
    parent.addEventListener('toolbar-init', (initEvent) => {
      console.log("Workflow Demo: Tag Toolbar was initialized - event kicked in - will now register");
      const workflow = initEvent.detail.workflow;

      workflow.add(window.toolbarWorkflow.workflowToLog);
      workflow.add(window.toolbarWorkflow.workflowToDisableRefresh);

      // Stop the event here, otherwise parent elements which have an event listener would get triggered as well
      initEvent.stopPropagation();
    });
  },

  // Attach event-listener to the parent of the inline-toolbar, so we can register the workflow when the toolbar is created
  initInlineToolbar: function(name) {
    console.log('initInlineToolbar(' + name + ')');
    var parent = document.getElementById(name);
    console.log('parent:', parent);
    parent.addEventListener('toolbar-init', (initEvent) => {
      console.log("Workflow Demo: Inline Toolbar was initialized - event kicked in - will now register");
      const workflow = initEvent.detail.workflow;

      workflow.add(window.toolbarWorkflow.workflowToLog);
      workflow.add(window.toolbarWorkflow.workflowToDisableRefresh);

      // Stop the event here, otherwise parent elements which have an event listener would get triggered as well
      initEvent.stopPropagation();
    }); 
  }

}