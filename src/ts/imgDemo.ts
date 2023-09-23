
export function createImgDemo() {
  var winAny = window as any;
  winAny.imgDemo = {
  /** Start the system with the class name marking all labels */
  start: function(lblClassId: string) {
    var labels = document.querySelectorAll('.' + lblClassId);
    // console.log("labels", labels);
    // return;
    labels.forEach(function (label) {
      // console.log("label", label);
      var parts = winAny.imgDemo.getImgAndLabel(label);
      winAny.imgDemo.showCurrentSrc(parts, 'start');
      parts.img.onload = function () { winAny.imgDemo.showCurrentSrc(parts, 'repeat'); }

    });
  },

  /** Find the label object and find the matching image  */
  getImgAndLabel: function(labelId: string) {
    // find the label
    const label = typeof(labelId) == 'string' 
      ? document.querySelector(labelId)
      : labelId;

    // based on the label, try to find the image
    // console.log("label", label);
    var labelParentDiv = label.parentElement;
    // console.log("labelParentDiv", labelParentDiv);
    // based on the parent, find the next image sibling
    var img = labelParentDiv.nextElementSibling;
    // console.log("img", img);

    // if still null, it's because it's wrapped inside the <hide-silent> tag
    if (img == null)
      img = labelParentDiv.parentElement.nextElementSibling;

    // if the img is actually a picture tag, find the inner img
    if (img.tagName == "PICTURE")
      img = img.getElementsByTagName('img')[0];

    // console.log("img", img != null);
    return { img, label };
  },

  /** Show the currentSrc in the label */
  showCurrentSrc: function(parts: any, note: string) {
    // log for debugging
    // console.log('showCurrentSrc-' + note, parts);
    let src = parts.img.currentSrc;
    if (!src) return;
    const lastSlash = src.lastIndexOf('/');
    src = "..." + src.substring(lastSlash);
    parts.label.innerHTML = 'CurrentSrc:' + src;
  },
};


}