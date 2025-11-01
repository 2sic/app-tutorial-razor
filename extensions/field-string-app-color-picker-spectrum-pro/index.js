/*
  Spectrum Vanilla color picker WebComponent with correct default-palette handling.

  Behaviour:
  - If ShowDefaultPalette setting is true -> show Spectrum's default palette.
    - If Swatches are also provided -> merge swatches into the default palette (so both appear).
  - If ShowDefaultPalette setting is false -> only show Swatches (if any).
  - UseAlphaValues controls showAlpha.
  - Swatches are read from field.settings.Swatches (newline-separated).
*/

(() => {
  const tagName = "field-string-app-color-picker-spectrum-pro";
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

  class StringColorPickerPro extends HTMLElement {
    /** connectedCallback() is the standard callback when the component has been attached */
    connectedCallback() {
      this.innerHTML = html;
      this.input = this.querySelector("#color-picker");
      // Set initial value from connector if exists
      this.input.value =
        this.connector && this.connector.data
          ? this.connector.data.value || ""
          : "";
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

    /** Utility: read UseAlphaValues setting as boolean */
    useAlphaFromSettings() {
      const s =
        this.connector && this.connector.field && this.connector.field.settings
          ? this.connector.field.settings.UseAlphaValues
          : undefined;
      // Accept boolean true, string "true", number 1 as true
      return (
        s === true || String(s).toLowerCase() === "true" || Number(s) === 1
      );
    }

    showDefaultPaletteFromSettings() {
      const s =
        this.connector && this.connector.field && this.connector.field.settings
          ? this.connector.field.settings.ShowDefaultPalette
          : undefined;
      // Accept boolean true, string "true", number 1 as true
      return (
        s === true || String(s).toLowerCase() === "true" || Number(s) === 1
      );
    }

    /** This is called when the JS is loaded from loadScript - so Spectrum is ready */
    initSpectrum() {
      // Read swatches BEFORE creating Spectrum so showPalette/palette can be set from the start
      const extraSwatches = this.getSwatches(); // array of color strings
      const hasSwatches = extraSwatches && extraSwatches.length > 0;

      const showAlpha = this.useAlphaFromSettings();
      const showDefaultPalette = this.showDefaultPaletteFromSettings();

      // Decide whether palette UI should be visible at all
      const shouldShowPalette = showDefaultPalette || hasSwatches;

      // Base options for Spectrum
      const options = {
        showInput: true,
        showInitial: true,
        preferredFormat: "hex",
        showAlpha: showAlpha,
        showPalette: shouldShowPalette,
        allowEmpty: true,
        // pass the color argument through to handler (Spectrum may give different shapes)
        change: (color) => this.handleChange(color),
        hide: () => this.handleHide(),
        show: () => this.setupContainerClickHandler(), // Set up click handler when shown
      };

      // If we should NOT show the default palette but have swatches, set palette to swatches
      if (!showDefaultPalette && hasSwatches) {
        // palette needs to be an array of arrays (rows)
        options.palette = [extraSwatches];
        this.sp = Spectrum.create(this.input, options);
      } else {
        // If we want the default palette (or no palette but maybe will be hidden),
        // create without providing palette so Spectrum's default remains intact.
        this.sp = Spectrum.create(this.input, options);

        // If there are extra swatches, merge them into the existing palette (preserve default)
        if (hasSwatches) {
          try {
            const currentPalette = this.sp.option("palette") || [];
            // Append the extraSwatches as an additional row
            this.sp.option("palette", [...currentPalette, extraSwatches]);
          } catch (err) {
            // If option isn't available or any error occurs, set palette to only extra swatches as fallback
            this.sp.option && this.sp.option("palette", [extraSwatches]);
          }
        }
      }

      this.cleared = !this.connector.data.value;

      // Set up click handler for the entire container after Spectrum is initialized
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
        if (event.target === this.input || event.target.tagName === "INPUT")
          return;

        // Prevent unwanted bubbling
        event.preventDefault();
        event.stopPropagation();

        // Toggle or show the picker
        if (typeof this.sp.toggle === "function") this.sp.toggle();
        else if (typeof this.sp.show === "function") this.sp.show();
      };

      container.addEventListener("click", this.containerClickHandler);
    }

    /** Update the value when color is selected (live changes) */
    handleChange(color) {
      this.cleared = false;
      let value = null;

      if (!color) {
        // If no color object provided, fall back to the input text (or null)
        value = this.input.value || null;
      } else {
        // color is a tinycolor instance (or similar)
        try {
          // If fully opaque, use 6-digit hex; otherwise use 8-digit hex
          if (typeof color.getAlpha === "function") {
            value =
              color.getAlpha() === 1
                ? color.toHexString()
                : color.toHex8String();
          } else if (typeof color.toHexString === "function") {
            value = color.toHexString();
          } else if (color && color.hex) {
            // some spectrum versions might pass { hex: "#rrggbb", a: 1 }-like objects
            value =
              color.a === 1 || color.a == null
                ? color.hex
                : color.hex + Math.round((color.a || 1) * 255).toString(16);
          } else {
            // Fallback to input value
            value = this.input.value || null;
          }
        } catch (err) {
          value = this.input.value || null;
        }
      }

      this.input.value = value || "";
      this.updateIfChanged(value);
    }

    /** Update the value when picker is closed */
    handleHide() {
      // If cleared, we should send null
      if (this.cleared) {
        this.updateIfChanged(null);
        return;
      }

      // Try to get the color object from the Spectrum instance and convert to hex/hex8
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
      if (!data) return;
      if (data.value === "" && value == null) return;
      if (data.value === value) return;
      data.update(value);
    }

    /** Load the settings and convert to swatch-list */
    getSwatches() {
      // the field "Swatches" is the field in the content-type containing the colors
      // it's upper-case, because that's how the field is named
      var swatches =
        this.connector && this.connector.field && this.connector.field.settings
          ? this.connector.field.settings.Swatches
          : null;
      if (!swatches) return [];
      return swatches
        .split("\n")
        .map((colorLine) => colorLine.trim())
        .filter(Boolean)
        .map((colorLine) => {
          var withLabel = colorLine.split(" ");
          return withLabel[0]; // first part is the color
        });
    }
  }

  // Register this web component - if it hasn't been registered yet
  if (!customElements.get(tagName))
    customElements.define(tagName, StringColorPickerPro);
})();
