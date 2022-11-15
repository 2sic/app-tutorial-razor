export function initRevealers() { 
  const sections = document.querySelectorAll("snippet.reveal-on-h3");
  // console.log('sections', sections, sections.length);

  sections.forEach((sect) => {
    const h3 = sect.querySelector("h3");
    // console.log('t', sect, 'h3', h3);
    if (h3 != null)
      h3.addEventListener("click", () => {
        sect.classList.toggle('reveal');
      });
  });
}