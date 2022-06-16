import { show, toggle } from 'slidetoggle';
import { initAccordion } from './lib-2sxc-accordion';
import { AccordionOptions } from './lib-2sxc-accordion-options';
declare let ace: any;

function initSourceCode({ domAttribute, aceOptions  }: { domAttribute: string, aceOptions: { sourceCodeId: string, language: string, wrap: boolean } }) {  
  // set theme
  let editor = ace.edit(aceOptions.sourceCodeId, { 
    useWrapMode: true,
    maxLines: 30
  });

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