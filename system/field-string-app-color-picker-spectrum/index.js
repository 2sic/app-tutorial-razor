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
        margin-top: 2px;
        font-size: 1.1rem;
        border-radius: 0 5px 5px 0;
        border-style: solid;
        border-color: #cbcbcb;
        border-width: 2px 2px 2px 0;
        width: 100px;
        outline: none;
        transition: border-color 0.2s;
      }
      .sp-colorize-container {
        border-radius: 5px !important;
        border: #cbcbcb 2px solid;
      }
      .sp-original-input-container:has(#color-picker:focus) .sp-colorize-container {
        border-color: #a8a8a8 !important;
      }
      #color-picker:focus {
        border-color: #a8a8a8;
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
      // Debug output, see what's being sent
      if (data.value === "" && value == null) return;
      if (data.value === value) return;
      data.update(value);
    }
  }

  // Register this web component - if it hasn't been registered yet
  if (!customElements.get(tagName))
    customElements.define(tagName, StringColorPicker);
})();
