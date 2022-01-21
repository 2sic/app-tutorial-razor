import { toggle } from 'slidetoggle';
import { initAccordion } from './lib-2sxc-accordion';
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

  let codeBlock = (codeBlockHeader.parentNode as HTMLElement);
  codeBlockHeader.classList.add("active");

  codeBlockHeader.addEventListener("click", () => {
    toggle((codeBlock.querySelector(".source-code") as HTMLElement), {});
    codeBlock.classList.toggle("is-expanded");
  })
}

var winAny = window as any;
winAny.razorTutorial ??= {};
winAny.razorTutorial.initAccordion ??= initAccordion;
winAny.razorTutorial.initSourceCode ??= initSourceCode;