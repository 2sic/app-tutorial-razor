import { show, toggle } from 'slidetoggle';
import { initAccordion } from './lib-2sxc-accordion';
import { AccordionOptions } from './lib-2sxc-accordion-options';
import { initRevealers } from './snippets';
import { initSplit } from './split';
import { createImgDemo } from './imgDemo';
import { initToggleAll } from './accordions';
declare let ace: any;

function initSourceCode({ domAttribute, aceOptions  }: { domAttribute: string, aceOptions: { sourceCodeId: string, language: string, wrap: boolean } }) {  
  // debug
  // console.log('args', arguments);

  // set base path - in case it's loaded as a module or similar and doesn't know its path
  // you can always debug this in the browser using commands such as:
  // ace.config.get('themePath')
  const acePath = "https://cdn.jsdelivr.net/npm/ace-builds@1.35.2/src-noconflict";
  ace.config.set('basePath', acePath);
  ace.config.set('themePath', acePath);
  ace.config.set('modePath', acePath);
  // console.log('ace.config', ace.config);

  // set theme
  let editor = ace.edit(aceOptions.sourceCodeId, { 
    wrap: true,
    maxLines: 30
  });

  // console.log('editor', editor, editor.getOptions());

  editor.setTheme("ace/theme/sqlserver");
  editor.session.setMode(aceOptions.language);
  editor.session.setOptions({
    wrap: aceOptions.wrap,
  });

  // enable collapse
  let codeBlockHeader = document.querySelector(`[${domAttribute}] .header:not(.active)`);
  if (codeBlockHeader == null) return;

  let codeBlock = codeBlockHeader.parentElement;
  codeBlockHeader.classList.add("active");

  codeBlockHeader.addEventListener("click", () => {
    toggle((codeBlock.querySelector(".source-code") as HTMLElement), {});
    codeBlock.classList.toggle("is-expanded");
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
winAny.razorTutorial.initSourceCode ??= initSourceCode;

// Get snippets ready
winAny.snippets ??= {};
winAny.snippets.initRevealers ??= initRevealers;

// Get splitter ready
winAny.splitter ??= {};
winAny.splitter.init ??= initSplit;

// Get the imgDemo ready
createImgDemo();

winAny.tutAccordions ??= {};
winAny.tutAccordions.init ??= initToggleAll;

