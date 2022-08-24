import { show, toggle } from 'slidetoggle';
import { initAccordion } from './lib-2sxc-accordion';
import { AccordionOptions } from './lib-2sxc-accordion-options';
declare let ace: any;

function initAceSourceCode({ domAttribute, options  }: { domAttribute: string, options: { sourceCodeId: string, language: string, wrap: boolean } }) {  
  // set theme
  let editor = ace.edit(options.sourceCodeId, { 
    useWrapMode: true,
    maxLines: 30
  });

  editor.setTheme("ace/theme/sqlserver");
  editor.session.setMode(options.language);
  editor.session.setOptions({
    wrap: options.wrap,
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

var winAny = window as any;
winAny.razorTutorial ??= {};
winAny.razorTutorial.initAccordion ??= initAccordion;
winAny.razorTutorial.initAceSourceCode ??= initAceSourceCode;