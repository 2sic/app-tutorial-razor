// 2dm 2023-05-04 I believe this is actually not in use
// Will remove the TurnOn for now, and monitor
// probably delete ca. 2023-06
export function initRevealers() { 
  const sections = document.querySelectorAll("snippet.reveal-on-h3");
  // console.log('sections', sections, sections.length);

  sections.forEach ((sect) => {
    const h3 = sect.querySelector("h3");
    // console.log('t', sect, 'h3', h3);
    if (h3 != null)
      h3.addEventListener("click", () => {
        sect.classList.toggle('reveal');
      });
  });
}