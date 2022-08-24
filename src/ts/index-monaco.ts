import { show, toggle } from 'slidetoggle';
import { initAccordion } from './lib-2sxc-accordion';
import { AccordionOptions } from './lib-2sxc-accordion-options';
import type * as Monaco from 'monaco-editor';


function beforeInitMonacoSourceCode() {
  window.require.config({
    paths: {
      vs: ['https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.33.0/min/vs'],
    },
  });

  window.require(['vs/editor/editor.main'], (monaco: typeof Monaco) => {
    window.monaco = monaco;
  });
}

function initMonacoSourceCode({ domAttribute, options, appPath }: { domAttribute: string, options: { sourceCodeId: string, language: string}, appPath: string }) {

  // @ts-ignore
  self.MonacoEnvironment = {
    getWorkerUrl: function (moduleId: any, label: string) {
      if (label === 'json') {
        return appPath + '/dist/json.worker.min.js';
      }
      if (label === 'css' || label === 'scss' || label === 'less') {
        return appPath + '/dist/css.worker.min.js';
      }
      if (label === 'html' || label === 'handlebars' || label === 'razor') {
        return appPath + '/dist/html.worker.min.js';
      }
      if (label === 'typescript' || label === 'javascript') {
        return appPath + '/dist/ts.worker.min.js';
      }
      return appPath + '/dist/editor.worker.min.js';
    }
  };

  window.monaco.editor.defineTheme('razorTutorialTheme', {
    base: 'vs',
    inherit: true,
    rules: [ ],
    colors: {
      'editor.background': '#f5f5f5',
    }
  });
  

  // enable collapse
  let codeBlockHeader = document.querySelector(`[${domAttribute}] .header:not(.active)`);
  if (codeBlockHeader == null || codeBlockHeader.parentElement.querySelector('#' + options.sourceCodeId) == null) 
  {
    //for codeblocks that are not collapsable
    createMonaco(document.getElementById(options.sourceCodeId).parentElement, 
      document.getElementById(options.sourceCodeId).innerText, 
      options.language, 'razorTutorialTheme', false, options.sourceCodeId);
    return;
  }

  let codeBlock = codeBlockHeader.parentElement;
  codeBlockHeader.classList.add("active");

  if(codeBlock.classList.contains("is-expanded")) {
    //for codeblocks that are collapsable and by default not collapsed
    createMonaco(document.getElementById(options.sourceCodeId).parentElement, 
      document.getElementById(options.sourceCodeId).innerText, 
      options.language, 'razorTutorialTheme', false, options.sourceCodeId);
  } else {
    //for codeblocks that are collapsable and by default collapsed
    codeBlock.classList.toggle("is-expanded");
    createMonaco(document.getElementById(options.sourceCodeId).parentElement, 
      document.getElementById(options.sourceCodeId).innerText, 
      options.language, 'razorTutorialTheme', false, options.sourceCodeId);
    codeBlock.classList.toggle("is-expanded");
  }

  codeBlockHeader.addEventListener("click", () => {
    toggle((codeBlock.querySelector(".source-code") as HTMLElement), {});
    codeBlock.classList.toggle("is-expanded");
  })
}

function createMonaco(element: HTMLElement, value: string, language: string, theme: string, scrollBeyondLastLine: boolean, sourceCodeId: string) {
  window.monaco.editor.create(element, {
    value: value, language: language, theme: theme, scrollBeyondLastLine: scrollBeyondLastLine
  });
  document.getElementById(sourceCodeId).classList.add("hidden");
}

export function showParentSections(targetOpenElem: HTMLElement, options: AccordionOptions) {
  let parentSection = targetOpenElem.parentElement.closest(`[${options.attrChild}]`) as HTMLElement;
  if (parentSection == null) return
  parentSection.parentElement.classList.add(`${options.classIsExpanded}`);

  show(parentSection, {})
}

var winAny = window as any;
winAny.razorTutorial ??= {};
winAny.razorTutorial.initAccordion ??= initAccordion;
winAny.razorTutorial.initMonacoSourceCode ??= initMonacoSourceCode;
winAny.razorTutorial.beforeInitMonacoSourceCode ??= beforeInitMonacoSourceCode;