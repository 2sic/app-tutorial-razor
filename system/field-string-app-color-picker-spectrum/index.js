/*
  This example shows a plain JS WebComponent which uses Spectrum Vanilla for color picking.
  Uses Spectrum Vanilla from https://github.com/LeaVerou/spectrum
  This simple picker has a predefined set of colors and doesn't allow field configuration
*/

// always use an IIFE to ensure you don't put variables in the window scope
(() => {
  const tagName = "field-string-app-color-picker-spectrum";
  const spectrumVersion = "1.1.1";
  const spectrumBase = `https://unpkg.com/spectrum-vanilla@${spectrumVersion}/dist`;

  const spectrumJsCdn = `${spectrumBase}/spectrum.min.js`;
  const spectrumCssCdn = `${spectrumBase}/spectrum.min.css`;

  const html = `
    <link rel="stylesheet" href="${spectrumCssCdn}"/>
    <style>
      #color-picker {
        padding: 2px 8px;
        margin-top: 5px;
        font-size: 1.1rem;
        width: 100%;
        outline: none;
        transition: border-color 0.2s;
        border: none;
      }
      .sp-original-input-container {
        display: flex !important;
        cursor: pointer;
      }
    </style>
    <div class="spectrum-container">
      <input id="color-picker" type="text" />
    </div>`;

  class StringColorPicker extends HTMLElement {
    /** connectedCallback() is the standard callback when the component has been attached */
    connectedCallback() {
      this.innerHTML = html;
      this.input = this.querySelector("#color-picker");
      // Set initial value from connector if exists
      this.input.value = this.connector.data.value || "";
      // load Spectrum if not loaded
      this.connector.loadScript("Spectrum", spectrumJsCdn, () => {
        this.initSpectrum();
      });
    }

    /** disconnectedCallback() is a standard callback for clean-up */
    disconnectedCallback() {
      if (this.sp) {
        this.sp.destroy();
        this.sp = null;
      }
      // Remove the click handler if it exists
      if (this.containerClickHandler) {
        const container = this.querySelector(".sp-original-input-container");
        if (container) {
          container.removeEventListener("click", this.containerClickHandler);
        }
      }
    }

    /** This is called when the JS is loaded from loadScript - so Spectrum is ready */
    initSpectrum() {
      this.sp = Spectrum.create(this.input, {
        showInput: true,
        showInitial: true,
        preferredFormat: "hex",
        showPalette: false,
        allowEmpty: true, // enables clear functionality
        change: (color) => this.handleChange(),
        hide: () => this.handleHide(),
      });

      // remember if we're working empty as of now
      this.cleared = !this.connector.data.value;

      // set up click handler for the container to toggle picker
      this.setupContainerClickHandler();
    }

    /** Set up click handler to make the entire picker container clickable */
    setupContainerClickHandler() {
      const container = this.querySelector(".sp-original-input-container");
      if (!container || !this.sp) return;

      // Remove old handler if exists
      if (this.containerClickHandler) {
        container.removeEventListener("click", this.containerClickHandler);
      }

      // Delegate clicks inside the container
      this.containerClickHandler = (event) => {
        // Ignore clicks directly on the input field
        if (event.target === this.input || event.target.tagName === "INPUT") return;

        event.preventDefault();
        event.stopPropagation();

        // Toggle or show the picker
        if (typeof this.sp.toggle === "function") this.sp.toggle();
        else if (typeof this.sp.show === "function") this.sp.show();
      };

      container.addEventListener("click", this.containerClickHandler);
    }

    /** Update the value when color is selected */
    handleChange() {
      this.cleared = false;
      // Spectrum Vanilla updates the input element
      const value = this.input.value;
      this.updateIfChanged(value);
    }

    /** Update the value when picker is closed */
    handleHide() {
      // Only update if value actually changed or cleared
      if (this.cleared) {
        this.updateIfChanged(null);
      }
    }

    /** Only update the value if it really changed, so form isn't dirty if nothing was set */
    updateIfChanged(value) {
      var data = this.connector.data;
      if (!data) return;
      if (data.value === "" && value == null) return;
      if (data.value === value) return;
      data.update(value);
    }
  }

  // Register this web component - if it hasn't been registered yet
  if (!customElements.get(tagName))
    customElements.define(tagName, StringColorPicker);
})();
