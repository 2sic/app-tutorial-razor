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
  var editor: Monaco.editor.IStandaloneCodeEditor;
  
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

  window.monaco.editor.create(document.getElementById(options.sourceCodeId).parentElement, {
    value: document.getElementById(options.sourceCodeId).innerText,
    language: options.language, theme: 'razorTutorialTheme', scrollBeyondLastLine: false
  });

  // document.getElementById(options.sourceCodeId).classList.add("hidden");

  // enable collapse
  let codeBlockHeader = document.querySelector(`[${domAttribute}] .header:not(.active)`);
  if (codeBlockHeader == null) return;

  let codeBlock = codeBlockHeader.parentElement;
  codeBlockHeader.classList.add("active");

  codeBlockHeader.addEventListener("click", () => {
    toggle((codeBlock.querySelector(".source-code") as HTMLElement), {});
    codeBlock.classList.toggle("is-expanded");
    if(codeBlock.classList.contains("is-expanded")) {
      console.log("SDV- is expanded");
      // editor = window.monaco.editor.create((codeBlock.querySelector(".source-code") as HTMLElement), {
      //   value: (codeBlock.querySelector(".source-code") as HTMLElement).innerText,
      //   language: options.language, theme: 'razorTutorialTheme', scrollBeyondLastLine: false
      // });
      // window.monaco.editor.getModels().forEach(x => {
      //   // console.log(x.getValue());
      //   x.setValue(x.getValue());
      //   // x.
      // });
    } else {
      console.log("SDV- is not expanded");
      // window.monaco.editor.getModels().find(x => x.id === editor.getId()).dispose();
    }
  })
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