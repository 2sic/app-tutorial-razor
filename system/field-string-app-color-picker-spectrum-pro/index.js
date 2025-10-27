/*
  This example shows a plain JS WebComponent which uses Spectrum Vanilla for color picking.
  Uses Spectrum Vanilla from https://github.com/LeaVerou/spectrum
  This simple picker has a predefined set of colors and doesn't allow field configuration
*/

// always use an IIFE to ensure you don't put variables in the window scope
(() => {
  const tagName = "field-string-app-color-picker-spectrum-pro";
  const spectrumJsCdn =
    "https://unpkg.com/spectrum-vanilla/dist/spectrum.min.js";
  const spectrumCssCdn =
    "https://unpkg.com/spectrum-vanilla/dist/spectrum.min.css";
  const html = `
    <link rel="stylesheet" href="${spectrumCssCdn}"/>
    <div class="spectrum-container">
      <input id="color-picker" type="text" />
    </div>`;

  class StringColorPickerPro extends HTMLElement {
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
        showAlpha: true,
        showPalette: true,
        allowEmpty: true,
        // Use the event argument to get the color object
        change: (e) => this.handleChange(e && e.detail ? e.detail.color : null),
        hide: () => this.handleHide(),
      });

      // merge additional custom swatches into default Spectrum palette
      const extraSwatches = this.getSwatches();
      if (extraSwatches.length) {
        const currentPalette = this.sp.option("palette") || [];
        this.sp.option("palette", [...currentPalette, extraSwatches]);
      }

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
      // If cleared, we should send null
      if (this.cleared) {
        this.updateIfChanged(null);
        return;
      }

      // Try to get the color object from the Spectrum instance
      let value = null;
      try {
        const colorObj =
          this.sp && typeof this.sp.get === "function" ? this.sp.get() : null;

        if (colorObj) {
          if (typeof colorObj.getAlpha === "function") {
            value =
              colorObj.getAlpha() === 1
                ? colorObj.toHexString()
                : colorObj.toHex8String();
          } else if (typeof colorObj.toHexString === "function") {
            value = colorObj.toHexString();
          } else {
            // If the color object doesn't provide helpers, fall back to the displayed input
            value = this.input.value || null;
          }
        } else {
          // No color object (empty), use the input value or null
          value = this.input.value || null;
        }
      } catch (err) {
        // On any error, fall back to input value
        value = this.input.value || null;
      }

      // Update input display and propagate if changed
      this.input.value = value || "";
      this.updateIfChanged(value);
    }

    /** Only update the value if it really changed, so form isn't dirty if nothing was set */
    updateIfChanged(value) {
      var data = this.connector.data;
      // Debug output, see what's being sent
      if (data.value === "" && value == null) return;
      if (data.value === value) return;
      data.update(value);
    }

    /** Load the settings and convert to swatch-list */
    getSwatches() {
      // the field "Swatches" is the field in the content-type containing the colors
      // it's upper-case, because that's how the field is named
      var swatches = this.connector.field.settings.Swatches;
      if (!swatches) return [];
      return swatches.split("\n").map((colorLine) => {
        var withLabel = colorLine.trim().split(" ");
        return withLabel[0]; // first part is the color
      });
    }
  }

  // Register this web component - if it hasn't been registered yet
  if (!customElements.get(tagName))
    customElements.define(tagName, StringColorPickerPro);
})();
