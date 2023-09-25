interface AccordionData {
  nameId: string;
  isOpen: boolean;
}

export function initToggleAll(data?: AccordionData) {
  var button = document.getElementById(`accordion-toggle-${data?.nameId}`);
  button.innerHTML = data?.isOpen ? "Collapse all" : "Expand all";

  document.querySelectorAll(".btn-collapse").forEach((button) => {
    button.addEventListener("click", (event) => {
      event.preventDefault();
      const nameId = button.getAttribute("data-id");
      toggleAllAccordions(nameId);
    });
  });
}

function toggleAllAccordions(nameId: string) {
  var button = document.getElementById(`accordion-toggle-${nameId}`);
  
  var containerWithNameId = document.getElementById(nameId);
  var accordionHeaders = containerWithNameId.querySelectorAll(
    ".accordion-header button"
  );
  var accordionCollapses = containerWithNameId.querySelectorAll(
    ".accordion-collapse"
  );

  var isCollapsed = Array.from(accordionHeaders).some(function (header) {
    return header.classList.contains("collapsed");
  });

  if (isCollapsed) {
    button.innerHTML = "Collapse all";
    accordionHeaders.forEach((header) => {
      header.classList.remove("collapsed");
    });
    accordionCollapses.forEach((collapse) => {
      collapse.classList.add("show");
    });
  } else {
    button.innerHTML = "Expand all";
    accordionHeaders.forEach((header) => {
      header.classList.add("collapsed");
    });
    accordionCollapses.forEach((collapse) => {
      collapse.classList.remove("show");
    });
  }
}
