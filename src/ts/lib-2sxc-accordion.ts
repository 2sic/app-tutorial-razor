import { hide, show, toggle } from 'slidetoggle';
import { showParentSections } from '.';
import { AccordionOptions } from './lib-2sxc-accordion-options';

/*
  This is a shared code used in various 2sxc apps. Make sure that they are in sync, so if you improve it, improve all 2sxc apps which use this. 
  ATM they are:
  - Accordion
  - FAQ
  - App School System

  The master with the newest / best version must always be the Accordion, the others should then get a fresh copy.
  Because this is shared, all parameters like DOM-IDs etc. must be provided in the Init-call that it can work across apps
*/ 

// THIS FILE HAS MINOR MODIFICATIONS
// - noHash because of Oqtane ca. line 38
// - SEE LINE ca. 59

export function initAccordion({ domId, options } : { domId: string, options: AccordionOptions }) {
  // get navHight for correct scrollposition
  let nav = (document.getElementsByTagName(options.tagStickyHeader)[0] as HTMLElement);
  let navHeight = (nav != null ? nav.offsetHeight : 0);

  // attach click to all accordions when loading
  let accordionOpener = document.querySelectorAll(`[${options.attrParent}]`);

  accordionOpener.forEach ((elem: HTMLElement, index) => {
    elem.addEventListener('click', (event) => {
      event.preventDefault();

      const currentElem = event.currentTarget as HTMLElement;
      const hash = currentElem.dataset.accordionParent;
      const parent = currentElem.parentElement;
      const targetOpenElem = document.querySelector(`[${options.attrChild}="${hash}"`) as HTMLElement;

      // add hash to url
      if (!options.noHash)
        location.hash = hash;

      // open / close mechanic for slide
      toggle(targetOpenElem, {});
      parent.classList.toggle(`${options.classIsExpanded}`);
    })
  });

  // get hash from url and open specific item
  if (window.location.hash) {
    const hash = window.location.hash.replace('#', '');
    const targetHashElem = document.querySelector(`[${options.attrChild}="${hash}"]`) as HTMLElement;
    
    // if target element exists scroll to element and open it
    if (targetHashElem) {
      // ADD THIS FUNCTION AFTER LIBRARY UPDATE:      
      showParentSections(targetHashElem, options);

      
      setTimeout(() => {
        
        // open accordion
        show(targetHashElem, {
          onAnimationEnd: () => {
            const elemOffsetX = targetHashElem.getBoundingClientRect().top + window.scrollY - navHeight;
            targetHashElem.parentElement.classList.add(`${options.classIsExpanded}`);
            
            // scroll to element which should open then
            window.scrollTo({
              top: elemOffsetX,
              left: 0,
              behavior: 'smooth'
            });
          }
        })
      })
    }
  }
}